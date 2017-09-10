using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using TaskScheduler.Configurator.Tests.Utils;
using TaskScheduler.Jobs.Data.Entities;

namespace TaskScheduler.Configurator.Tests
{
    public interface IJobDsl
    {
        Task<int> CreateJob(Job job);
        Task<Job> GetJob(int jobId);
        Task<Job> GenerateExistingJob();
        Task<Job> GetExistingJob(int jobId);
        Task<Job> GenerateNonExistingJob();
    }

    public class JobDsl : IJobDsl
    {
        private readonly HttpClient _httpClient;

        public JobDsl(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> CreateJob(Job job)
        {
            using (var json = JsonContent.Create(job))
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, ApiPaths.JobApiPath) { Content = json };
                var responseMessage = await _httpClient.SendAsync(requestMessage);
                return UrlParser.ExtractIdFromLocationHeader(responseMessage, ApiPaths.JobApiPath);
            }
        }

        public async Task<Job> GetJob(int jobId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{ApiPaths.JobApiPath}/{jobId}");
            var responseMessage = await _httpClient.SendAsync(requestMessage);
            return JsonConvert.DeserializeObject<Job>(await responseMessage.Content.ReadAsStringAsync());
        }

        public async Task<Job> GenerateExistingJob()
        {
            var job = Builder<Job>.CreateNew().Build();
            var createdJobId = await CreateJob(job);
            return await GetJob(createdJobId);
        }

        public async Task<Job> GetExistingJob(int jobId)
        {
            return await GetJob(jobId);
        }

        public async Task<Job> GenerateNonExistingJob()
        {
            return await Task.FromResult(Builder<Job>.CreateNew().Build());
        }
    }
}