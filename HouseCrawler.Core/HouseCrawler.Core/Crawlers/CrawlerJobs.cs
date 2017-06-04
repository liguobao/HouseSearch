using HouseCrawler.Core.Models;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Crawlers
{
    public class CrawlerJobs : Job
    {
        [Invoke(Begin = "2017-06-01 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
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
