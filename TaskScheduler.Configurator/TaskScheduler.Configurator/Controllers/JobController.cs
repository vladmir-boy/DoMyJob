using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using TaskScheduler.Jobs.Data.Entities;
using TaskScheduler.Jobs.Data.Repositories;

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
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<Job>))]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _jobRepository.All(cancellationToken));
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetJob")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Job))]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.Get(j => j.Id == id, cancellationToken);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }

        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody]Job job, CancellationToken cancellationToken)
        {
            var created = await _jobRepository.Add(job, cancellationToken);
            return CreatedAtRoute("GetJob", new {id = created.Id}, null);
        }

        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromBody]Job job, CancellationToken cancellationToken)
        {
            var found = await _jobRepository.Update(job, cancellationToken);
            if (!found)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int jobId, CancellationToken cancellationToken)
        {
            var found = await _jobRepository.Delete(jobId, cancellationToken);
            if (!found)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
