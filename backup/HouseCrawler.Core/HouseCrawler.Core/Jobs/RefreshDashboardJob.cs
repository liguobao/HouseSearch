using HouseCrawler.Core.Models;
using HouseCrawler.Core.Service;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public class RefreshDashboardJob : Job
    {
        HouseDashboardService houseDashboardService;
        public RefreshDashboardJob(HouseDashboardService houseDashboardService)
        {
            this.houseDashboardService = houseDashboardService;
        }
        //半个小时刷一次
        [Invoke(Begin = "2018-07-01 00:00", Interval = 1000 * 1800 , SkipWhileExecuting = true)]
        public void Run()
        {
            houseDashboardService.LoadDashboard();
        }
    }
}
