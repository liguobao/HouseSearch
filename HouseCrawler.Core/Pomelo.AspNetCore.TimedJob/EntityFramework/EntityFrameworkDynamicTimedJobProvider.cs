using System;
using System.Linq;
using System.Collections.Generic;

namespace Pomelo.AspNetCore.TimedJob.EntityFramework
{
    public class EntityFrameworkDynamicTimedJobProvider<TContext> : IDynamicTimedJobProvider
        where TContext : ITimedJobContext
    {
        private TContext Db { get; }

        public EntityFrameworkDynamicTimedJobProvider(TContext db)
        {
            Db = db;
        }

        public IList<DynamicTimedJob> GetJobs()
        {
            return Db.TimedJobs
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
