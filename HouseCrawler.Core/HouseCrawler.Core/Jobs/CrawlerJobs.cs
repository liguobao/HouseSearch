using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class CrawlerJobs : Job
    {
        [Invoke(Begin = "2018-04-05 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            PinPaiGongYuHouseCrawler.CapturPinPaiHouseInfo();
            PeopleRentingCrawler.CapturHouseInfo();
            DoubanHouseCrawler.CaptureHouseInfo();
        }
    }
}
