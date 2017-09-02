using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.TimedJob.EntityFramework
{
    public interface ITimedJobContext
    {
        DbSet<TimedJob> TimedJobs { get; set; }

        int SaveChanges();
    }
}
