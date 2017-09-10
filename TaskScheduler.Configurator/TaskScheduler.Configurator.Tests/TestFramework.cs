using System;
using System.Net.Http;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;
using Xunit.Ioc.Autofac;
using Xunit.Sdk;

namespace TaskScheduler.Configurator.Tests
{
    public class IntegrationTestFramework : AutofacTestFramework
    {
        private const string TestSuffixConvention = "Tests";

        public IntegrationTestFramework(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith(TestSuffixConvention));

            builder.Register(context => new TestOutputHelper())
                .AsSelf()
                .As<ITestOutputHelper>()
                .InstancePerLifetimeScope();
            builder.Register(context =>
            {
                return new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json").Build();
            }).As<IConfigurationRoot>().SingleInstance();
            builder.Register(context =>
            {
                var config = context.Resolve<IConfigurationRoot>();
                return new SqliteConnectionStringBuilder
                {
                    ConnectionString = $"{config.GetConnectionString("JobsDatabaseConnection")}.{Guid.NewGuid()}"
                };
            }).AsSelf().InstancePerLifetimeScope();
            builder.Register(context =>
            {
                return CreateClient(context.Resolve<IConfigurationRoot>(),
                    context.Resolve<SqliteConnectionStringBuilder>());
            }).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<JobDsl>().As<IJobDsl>().InstancePerLifetimeScope();
            // configure your container
            // e.g. builder.RegisterModule<TestOverrideModule>();

            Container = builder.Build();
        }

        private static HttpClient CreateClient(IConfigurationRoot config, SqliteConnectionStringBuilder connectionStringBuilder)
        {
            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseSetting("ConnectionStrings:JobsDatabaseConnection", connectionStringBuilder.ConnectionString)
                .UseStartup<Startup>();
            var server = new TestServer(host);
            return server.CreateClient();
        }
    }
}