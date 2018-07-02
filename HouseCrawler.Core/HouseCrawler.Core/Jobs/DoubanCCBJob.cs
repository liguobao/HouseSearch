using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class DoubanCCBJob : Job
    {
        PinPaiGongYuHouseCrawler pinpai;
        PeopleRentingCrawler people;

        DoubanHouseCrawler douban;

        CCBHouesCrawler ccbHouse;

        ZuberHouseCrawler zuber;

        MoGuHouseCrawler mogu;

        public DoubanCCBJob(PinPaiGongYuHouseCrawler pinpai,
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

        [Invoke(Begin = "2018-07-01 00:10", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("DoubanJob start");
            LogHelper.RunActionNotThrowEx(douban.Run);
            LogHelper.RunActionNotThrowEx(ccbHouse.Run);
            LogHelper.Info("DoubanJob finish");
        }
    }
}
