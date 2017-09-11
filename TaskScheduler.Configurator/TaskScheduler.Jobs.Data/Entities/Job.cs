using System;

namespace TaskScheduler.Jobs.Data.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Container { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset LastExecutionDate { get; set; }
        public bool IsActive { get; set; }
        public string Cron { get; set; }
    }
}