using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class HKSpaciousCrawlerJob : Job
    {
        [Invoke(Begin = "2018-05-20 00:00", Interval = 1000 * 1800, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionNotThrowEx(HKSpaciousCrawler.Run);
        }
    }
}
