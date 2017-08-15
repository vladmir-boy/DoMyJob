using System;
using Microsoft.EntityFrameworkCore;
using TaskScheduler.Jobs.Data.Entities;
using Microsoft.Extensions.Configuration;
namespace TaskScheduler.Jobs.Data
{
    public class JobDatabase : DbContext
    {
        public JobDatabase(DbContextOptions dbContextOptions) : base(dbContextOptions)
        { 
            
        }
        public DbSet<Job> Jobs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Filename=taskscheduler.design.sqlite");
            }
        }
    }
}
