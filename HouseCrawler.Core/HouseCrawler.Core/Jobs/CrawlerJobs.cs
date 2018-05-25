using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class CrawlerJobs : Job
    {
        [Invoke(Begin = "2018-05-20 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionNotThrowEx(PinPaiGongYuHouseCrawler.Run);
            LogHelper.RunActionNotThrowEx(PeopleRentingCrawler.Run);
            LogHelper.RunActionNotThrowEx(DoubanHouseCrawler.Run);
            LogHelper.RunActionNotThrowEx(CCBHouesCrawler.Run);
            LogHelper.RunActionNotThrowEx(ZuberHouseCrawler.Run);
            LogHelper.RunActionNotThrowEx(MoGuHouseCrawler.Run);
            LogHelper.RunActionNotThrowEx(HKSpaciousCrawler.Run);
        }
    }
}
