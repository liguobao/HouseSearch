using HouseMap.Crawler.Common;
using Pomelo.AspNetCore.TimedJob;

namespace HouseMap.Crawler.Jobs
{
    public class BaiXingJob : Job
    {
        private BaiXingHouseCrawler crawler;
        public BaiXingJob(BaiXingHouseCrawler crawler)
        {
            this.crawler = crawler;
        }
        
        [Invoke(Begin = "2018-07-01 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionNotThrowEx(crawler.Run);
        }
    }
}
