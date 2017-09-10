using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Microsoft.Data.Sqlite;
using TaskScheduler.Configurator.Tests.Utils;
using Xunit;
using Xunit.Ioc.Autofac;

namespace TaskScheduler.Configurator.Tests
{
    [UseAutofacTestFramework]
    public class JobControllerTests : IDisposable
    {
        private readonly IJobDsl _jobDsl;
        private readonly HttpClient _client;
        private readonly string _tempDatabasePath;

        public JobControllerTests()
        {
            
        }

        public JobControllerTests(IJobDsl jobDsl, HttpClient client, SqliteConnectionStringBuilder connectionStringBuilder)
        {
            _jobDsl = jobDsl;
            _client = client;
            _tempDatabasePath = connectionStringBuilder.DataSource;
        }

        [Fact]
        public async Task CreateJob()
        {
            var job = await _jobDsl.GenerateNonExistingJob();
            var requestMessage =
                new HttpRequestMessage(HttpMethod.Put, ApiPaths.JobApiPath)
                {
                    Content = JsonContent.Create(job)
                };
            var responseMessage = await _client.SendAsync(requestMessage);

            Assert.Equal(HttpStatusCode.Created, responseMessage.StatusCode);

            var createdJob = await _jobDsl.GetExistingJob(UrlParser.ExtractIdFromLocationHeader(responseMessage, ApiPaths.JobApiPath));

            Assert.Equal(job.Name, createdJob.Name);
            Assert.Equal(job.Container, createdJob.Container);
            Assert.Equal(job.CreateDate, createdJob.CreateDate);
            Assert.Equal(job.LastExecutionDate, createdJob.LastExecutionDate);
            Assert.Equal(job.IsActive, createdJob.IsActive);
        }

        [Fact]
        public async Task UpdateExistingJobShouldUpdateSpecifiedFields()
        {
            var existingJob = await _jobDsl.GenerateExistingJob();
            existingJob.Name = "updated";

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiPaths.JobApiPath)
            {
                Content = JsonContent.Create(existingJob)
            };

            var responseMessage = await _client.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.NoContent, responseMessage.StatusCode);

            var updatedJob = await _jobDsl.GetExistingJob(existingJob.Id);
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
            var nonExistingJob = _jobDsl.GenerateNonExistingJob();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiPaths.JobApiPath)
            {
                Content = JsonContent.Create(nonExistingJob)
            };
            var responseMessage = await _client.SendAsync(requestMessage);

            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        public void Dispose()
        {
            File.Delete(_tempDatabasePath);
        }
    }
}
