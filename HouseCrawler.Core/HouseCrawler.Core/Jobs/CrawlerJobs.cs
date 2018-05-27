using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class CrawlerJobs : Job
    {
        PinPaiGongYuHouseCrawler pinpai;
        PeopleRentingCrawler people;

        DoubanHouseCrawler douban;

        CCBHouesCrawler ccbHouse;

        ZuberHouseCrawler zuber;

        MoGuHouseCrawler mogu;

        public CrawlerJobs(PinPaiGongYuHouseCrawler pinpai,
        PeopleRentingCrawler people,
        DoubanHouseCrawler douban,
        CCBHouesCrawler ccbHouse,
        ZuberHouseCrawler zuber,
        MoGuHouseCrawler mogu)
        {
            this.pinpai = pinpai;
            this.people = people;
            this.douban = douban;
            this.ccbHouse = ccbHouse;
            this.zuber = zuber;
            this.mogu = mogu;

        }

        [Invoke(Begin = "2018-05-20 00:00", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.RunActionNotThrowEx(pinpai.Run);
            LogHelper.RunActionNotThrowEx(people.Run);
            LogHelper.RunActionNotThrowEx(douban.Run);
            LogHelper.RunActionNotThrowEx(ccbHouse.Run);
            LogHelper.RunActionNotThrowEx(zuber.Run);
            LogHelper.RunActionNotThrowEx(mogu.Run);
        }
    }
}
