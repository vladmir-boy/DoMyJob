using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TaskScheduler.Configurator.Tests.Utils;
using Xunit;

namespace TaskScheduler.Configurator.Tests
{
    public class JobControllerTests : IDisposable
    {
        private readonly string _tempDatabasePath;
        private readonly HttpClient _client;

        public JobControllerTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json").Build();

            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                ConnectionString = $"{config.GetConnectionString("JobsDatabaseConnection")}.{Guid.NewGuid()}"
            };

            _tempDatabasePath = connectionStringBuilder.DataSource;

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseSetting("ConnectionStrings:JobsDatabaseConnection", connectionStringBuilder.ConnectionString)
                .UseStartup<Startup>();
            var server = new TestServer(host);
            _client = server.CreateClient();
        }

        [Fact]
        public async Task CreateJob()
        {
            var job = await new JobDsl(_client).GenerateNonExistingJob();
            var requestMessage =
                new HttpRequestMessage(HttpMethod.Put, ApiPaths.JobApiPath)
                {
                    Content = JsonContent.Create(job)
                };
            var responseMessage = await _client.SendAsync(requestMessage);

            Assert.Equal(HttpStatusCode.Created, responseMessage.StatusCode);

            var createdJob = await new JobDsl(_client).GetExistingJob(UrlParser.ExtractIdFromLocationHeader(responseMessage, ApiPaths.JobApiPath));

            Assert.Equal(job.Name, createdJob.Name);
            Assert.Equal(job.Container, createdJob.Container);
            Assert.Equal(job.CreateDate, createdJob.CreateDate);
            Assert.Equal(job.LastExecutionDate, createdJob.LastExecutionDate);
            Assert.Equal(job.IsActive, createdJob.IsActive);
        }

        [Fact]
        public async Task UpdateExistingJobShouldUpdateSpecifiedFields()
        {
            var existingJob = await new JobDsl(_client).GenerateExistingJob();
            existingJob.Name = "updated";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiPaths.JobApiPath)
            {
                Content = JsonContent.Create(existingJob)
            };

            var responseMessage = await _client.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.NoContent, responseMessage.StatusCode);

            var updatedJob = await new JobDsl(_client).GetExistingJob(existingJob.Id);
            Assert.Equal(updatedJob.Name, existingJob.Name);
        }

        [Fact]
        public async Task GetNonExistingJobShouldReturnNotFound()
        {
            var nonExistingJobId = Builder<int>.CreateNew();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{ApiPaths.JobApiPath}/{nonExistingJobId}");
            var responseMessage = await _client.SendAsync(requestMessage);

            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [Fact]
        public async Task UpdateNonExistingJobShouldReturnNotFound()
        {
            var nonExistingJob = new JobDsl(_client).GenerateNonExistingJob();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiPaths.JobApiPath)
                {
                    Content = JsonContent.Create(nonExistingJob)
                };
            var responseMessage = await _client.SendAsync(requestMessage);

            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        public void Dispose()
        {
            _client.Dispose();
            File.Delete(_tempDatabasePath);
        }
    }
}
