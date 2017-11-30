using HouseCrawler.Core.Models;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public class CrawlerJobs : Job
    {
        [Invoke(BeginS = "2017-11-30 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            //Job要执行的逻辑代码
            PinPaiGongYuHouseCrawler.CapturPinPaiHouseInfo();

            PeopleRentingCrawler.CapturHouseInfo();

            DoubanHouseCrawler.CaptureHouseInfoFromConfig();

            HouseSourceInfo.RefreshHouseSourceInfo();
        }
    }
}
