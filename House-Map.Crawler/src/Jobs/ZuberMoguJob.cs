using HouseMap.Common;
using HouseMap.Crawler.Common;
using Pomelo.AspNetCore.TimedJob;

namespace HouseMap.Crawler.Jobs
{
    public class ZuberMoguJob : Job
    {
        PinPaiGongYuHouseCrawler pinpai;
        PeopleRentingCrawler people;

        DoubanHouseCrawler douban;

        CCBHouseCrawler ccbHouse;

        ZuberHouseCrawler zuber;

        MoGuHouseCrawler mogu;

        public ZuberMoguJob(PinPaiGongYuHouseCrawler pinpai,
        PeopleRentingCrawler people,
        DoubanHouseCrawler douban,
        CCBHouseCrawler ccbHouse,
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

        [Invoke(Begin = "2018-07-01 00:35", Interval = 1000 * 3600, SkipWhileExecuting = true)]
        public void Run()
        {
            LogHelper.Info("ZuberMoguJob start");
            LogHelper.RunActionNotThrowEx(zuber.Run);
            LogHelper.RunActionNotThrowEx(mogu.Run);

            LogHelper.Info("ZuberMoguJob finish");
        }
    }
}
