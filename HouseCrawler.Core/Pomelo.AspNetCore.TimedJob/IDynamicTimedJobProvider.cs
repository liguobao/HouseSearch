using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.TimedJob
{
    public interface IDynamicTimedJobProvider
    {
        IList<DynamicTimedJob> GetJobs();
    }
}
