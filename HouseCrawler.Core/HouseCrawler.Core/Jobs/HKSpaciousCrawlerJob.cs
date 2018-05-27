using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class HKSpaciousCrawlerJob : Job
    {
        private HKSpaciousCrawler crawler;
        public HKSpaciousCrawlerJob(HKSpaciousCrawler crawler)
        {
            this.crawler = crawler;
        }
        
        [Invoke(Begin = "2018-05-20 00:00", Interval = 1000 * 1800, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionNotThrowEx(crawler.Run);
        }
    }
}
