using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class BaiXingCrawlerJob : Job
    {
        private BaiXingHouseCrawler crawler;
        public BaiXingCrawlerJob(BaiXingHouseCrawler crawler)
        {
            this.crawler = crawler;
        }
        
        [Invoke(Begin = "2018-05-20 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionNotThrowEx(crawler.Run);
        }
    }
}
