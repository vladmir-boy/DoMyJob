using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskScheduler.Jobs.Data.Entities;

namespace TaskScheduler.Jobs.Data.Repositories
{
    public interface IJobRepository
    {
        Task Add(Job job, CancellationToken cancellationToken);
        Task Delete(int jobId, CancellationToken cancellationToken);
        Task<IEnumerable<Job>> All(CancellationToken cancellationToken);
    }

    public class JobRepository : IJobRepository
    {
        private readonly JobDatabase _db;

        public JobRepository(JobDatabase db)
        {
            _db = db;
        }
        public async Task Add(Job job, CancellationToken cancellationToken)
        {
            await _db.AddAsync(job, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task Delete(int jobId, CancellationToken cancellationToken)
        {
            var job = _db.Entry(await _db.FindAsync<Job>(jobId));
            _db.Remove(job);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Job>> All(CancellationToken cancellationToken)
        {
            // https://youtrack.jetbrains.com/issue/RSRP-464676
            return await _db.Jobs.ToListAsync(cancellationToken);
        }
    }
}