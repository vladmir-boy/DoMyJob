using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TaskScheduler.Jobs.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<JobDatabase>
{
    public JobDatabase CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<JobDatabase>();
        builder.UseSqlite("Filename=taskscheduler.design.sqlite");
        return new JobDatabase(builder.Options);
    }
}