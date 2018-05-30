using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class HKSpaciousJob : Job
    {
        private HKSpaciousCrawler crawler;
        public HKSpaciousJob
        (HKSpaciousCrawler crawler)
        {
            this.crawler = crawler;
        }
        
        [Invoke(Begin = "2018-05-20 00:20", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionNotThrowEx(crawler.Run);
        }
    }
}
