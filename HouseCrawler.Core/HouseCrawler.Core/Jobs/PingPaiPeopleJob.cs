using Pomelo.AspNetCore.TimedJob;

namespace HouseCrawler.Core
{
    public class PingPaiPeopleJob : Job
    {
        PinPaiGongYuHouseCrawler pinpai;
        PeopleRentingCrawler people;

        DoubanHouseCrawler douban;

        CCBHouesCrawler ccbHouse;

        ZuberHouseCrawler zuber;

        MoGuHouseCrawler mogu;

        public PingPaiPeopleJob(PinPaiGongYuHouseCrawler pinpai,
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

        [Invoke(Begin = "2018-07-01 00:25", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("PingPaiPeopleJob start");
            LogHelper.RunActionNotThrowEx(pinpai.Run);
            LogHelper.RunActionNotThrowEx(people.Run);

            LogHelper.Info("PingPaiPeopleJob finish");
        }
    }
}
