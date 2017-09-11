using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Xunit.Extensions.Microsoft.DI;
using Xunit.Sdk;

namespace TaskScheduler.Configurator.Tests
{
    public class IntegrationTestFramework : ServiceProviderTestFramework
    {
        private const string TestSuffixConvention = "Tests";

        public IntegrationTestFramework(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
            var builder = new ServiceCollection();
            foreach (var testClassType in GetType().Assembly.GetTypes().Where(t=>t.FullName.EndsWith(TestSuffixConvention)))
            {
                builder.AddTransient(testClassType);
            }

            builder.AddTransient<ITestOutputHelper>(context => new TestOutputHelper());
            builder.AddSingleton<IConfiguration>(context => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build());
            builder.AddScoped(context =>
            {
                var config = context.GetService<IConfiguration>();
                return new SqliteConnectionStringBuilder
                {
                    ConnectionString = $"{config.GetConnectionString("JobsDatabaseConnection")}.{Guid.NewGuid()}"
                };
            });
            builder.AddSingleton(context => CreateClient(context.GetService<IConfiguration>(),
                context.GetService<SqliteConnectionStringBuilder>()));
            builder.AddSingleton<IJobDsl, JobDsl>();

            Container = builder.BuildServiceProvider();
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