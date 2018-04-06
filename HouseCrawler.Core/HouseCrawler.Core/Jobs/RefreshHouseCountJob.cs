using HouseCrawler.Core.Models;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Jobs
{
    public class RefreshHouseCountJob : Job
    {
        [Invoke(Begin = "2018-04-05 00:00", Interval = 1000 * 3600 * 12, SkipWhileExecuting = true)]
        public void Run()
        {
            HouseSourceInfo.RefreshHouseSourceInfo();
        }
    }
}
