using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Jobs.Data.Entities;
using TaskScheduler.Jobs.Data.Repositories;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskScheduler.Configurator.Controllers
{
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private readonly IJobRepository _jobRepository;

        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }
        [HttpGet]
        public async Task<JsonResult> All(CancellationToken cancellationToken)
        {
            return await Task.FromResult(new JsonResult(await _jobRepository.All(cancellationToken)) { StatusCode = (int)HttpStatusCode.OK });
        }
        [HttpPost]
        public async Task<JsonResult> Register([FromBody]Job job, CancellationToken cancellationToken)
        {
            await _jobRepository.Add(job, cancellationToken);
            return await Task.FromResult(new JsonResult(null) {StatusCode = (int)HttpStatusCode.NoContent});
        }

        [HttpDelete]
        public async Task<JsonResult> UnRegister(int jobId, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new JsonResult(null) { StatusCode = (int)HttpStatusCode.NoContent });
        }
    }
}
