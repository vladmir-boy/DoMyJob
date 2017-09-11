using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskScheduler.Jobs.Data.Entities;

namespace TaskScheduler.Jobs.Data.Repositories
{
    public interface IJobRepository
    {
        Task<Job> Add(Job job, CancellationToken cancellationToken);
        Task<bool> Delete(int jobId, CancellationToken cancellationToken);
        Task<IEnumerable<Job>> All(CancellationToken cancellationToken);
        Task<Job> Get(Expression<Func<Job, bool>> predicate, CancellationToken cancellationToken);
        Task<bool> Update(Job job, CancellationToken cancellationToken);
    }

    public class JobRepository : IJobRepository
    {
        private readonly JobDatabase _db;

        public JobRepository(JobDatabase db)
        {
            _db = db;
        }
        public async Task<Job> Add(Job job, CancellationToken cancellationToken)
        {
            var created = await _db.AddAsync(job, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return created.Entity;
        }

        public async Task<bool> Delete(int jobId, CancellationToken cancellationToken)
        {
            var attachedJob = await _db.FindAsync<Job>(jobId);
            if (attachedJob == null)
            {
                return false;
            }

            _db.Remove(attachedJob);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<Job>> All(CancellationToken cancellationToken)
        {
            // https://youtrack.jetbrains.com/issue/RSRP-464676
            return await _db.Jobs.ToListAsync(cancellationToken);
        }

        public async Task<Job> Get(Expression<Func<Job, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _db.Jobs.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<bool> Update(Job job, CancellationToken cancellationToken)
        {

            try
            {
                _db.Jobs.Update(job);
                await _db.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                return false;
            }
        }
    }
}