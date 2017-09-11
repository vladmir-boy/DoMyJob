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
                .As<ITestOutputHelper>().AsSelf()
                .InstancePerLifetimeScope();
            builder.Register(context => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build()).As<IConfiguration>().SingleInstance();
            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();
                return new SqliteConnectionStringBuilder
                {
                    ConnectionString = $"{config.GetConnectionString("JobsDatabaseConnection")}.{Guid.NewGuid()}"
                };
            }).AsSelf().InstancePerLifetimeScope();
            builder.Register(context => CreateClient(context.Resolve<IConfiguration>(),
                context.Resolve<SqliteConnectionStringBuilder>())).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<JobDsl>().As<IJobDsl>().InstancePerLifetimeScope();

            Container = builder.Build();
        }

        private static HttpClient CreateClient(IConfiguration config, SqliteConnectionStringBuilder connectionStringBuilder)
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