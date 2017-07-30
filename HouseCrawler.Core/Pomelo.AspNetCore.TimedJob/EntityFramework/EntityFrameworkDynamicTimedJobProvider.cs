using System;
using System.Linq;
using System.Collections.Generic;

namespace Pomelo.AspNetCore.TimedJob.EntityFramework
{
    public class EntityFrameworkDynamicTimedJobProvider<TContext> : IDynamicTimedJobProvider
        where TContext : ITimedJobContext
    {
        private TContext DB { get; set; }

        public EntityFrameworkDynamicTimedJobProvider(TContext db)
        {
            DB = db;
        }

        public IList<DynamicTimedJob> GetJobs()
        {
            return DB.TimedJobs
                .Where(x => x.IsEnabled)
                .Select(x => new DynamicTimedJob
                {
                    Id = x.Id,
                    Begin = x.Begin,
                    Interval = x.Interval,
                    IsEnabled = x.IsEnabled
                })
                .ToList();
        }
    }
}
