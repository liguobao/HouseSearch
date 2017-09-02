using Microsoft.EntityFrameworkCore;

namespace Pomelo.AspNetCore.TimedJob.EntityFramework
{
    public static class TimedJobDbContextExtensions
    {
        public static ModelBuilder SetupTimedJobs(this ModelBuilder self)
        {
            return self.Entity<DynamicTimedJob>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.IsEnabled);
            });
        }
    }
}
