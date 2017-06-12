using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;

namespace HouseCrawler.Core
{
    public class DoubanHouseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();

        private static CrawlerDataContent dataContent = new CrawlerDataContent();

        public static void CaptureHouseInfoFromConfig()
        {
            try
            {
                int captrueHouseCount = 0;

                foreach (var doubanConf in dataContent.CrawlerConfigurations
                    .Where(c => c.ConfigurationName == ConstConfigurationName.Douban).ToList())
                {
                    var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);
                    for (var pageIndex = 0; pageIndex < confInfo.pagecount.Value; pageIndex++)
                    {
                        var lstHouseInfo = GetDataFromOnlineWeb(confInfo.groupid.Value, confInfo.cityname.Value, pageIndex);
                        dataContent.AddRange(lstHouseInfo);
                        dataContent.SaveChanges();
                        captrueHouseCount = captrueHouseCount + lstHouseInfo.Count;
                    }
                }
                HouseSourceInfo.RefreshHouseSourceInfo();

                BizCrawlerLog.SaveLog("爬取豆瓣租房数据",$"本次共爬取到{captrueHouseCount}条数据。",1);
            }
            catch (Exception ex)
            {
                LogHelper.Error("DoubanHouseCrawler CrawlerHouseInfo Exception", ex);
            }
        }

        public static void AddDoubanGroupConfig(string groupID, string cityName, int pageIndex = 0)
        {
            var cityInfo = $"{{ 'groupid':'{groupID}','cityname':'{cityName}','pagecount':5}}";

            var doubanConfig = dataContent.CrawlerConfigurations.FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.Douban && c.ConfigurationValue == cityInfo);
            if (doubanConfig != null)
                return;
            var lstHouseInfo = GetDataFromOnlineWeb(groupID, cityName, pageIndex);

            #region add douban group config

            if (lstHouseInfo.Count > 0)
            {
                var config =   new BizCrawlerConfiguration()
                {
                    ConfigurationKey = 0,
                    ConfigurationValue = cityInfo,
                    ConfigurationName = ConstConfigurationName.Douban,
                    DataCreateTime = DateTime.Now,
                    IsEnabled = true,
                };
                dataContent.AddRange(lstHouseInfo);
                dataContent.Add(config);
                dataContent.SaveChanges();

                HouseSourceInfo.RefreshHouseSourceInfo();
            }
            #endregion

        }


        private static List<BizHouseInfo> GetDataFromOnlineWeb(string groupID, string cityName, int pageIndex)
        {
            HashSet<string> hsDoubanHouseURL = new HashSet<string>();

            dataContent.HouseInfos.Where(h=>h.Source== ConstConfigurationName.Douban)
                .Select(h=>h.HouseOnlineURL).Distinct().ToList()
                .ForEach(houseURL=> 
                {
                    if (!hsDoubanHouseURL.Contains(houseURL))
                        hsDoubanHouseURL.Add(houseURL);
                });

            List<BizHouseInfo> lstHouseInfo = new List<BizHouseInfo>();

            var url = $"https://www.douban.com/group/{groupID}/discussion?start={pageIndex * 25}";
            var htmlResult = HTTPHelper.GetHTML(url);
            if (string.IsNullOrEmpty(htmlResult))
                return lstHouseInfo;
            var page = htmlParser.Parse(htmlResult);

            foreach (var trItem in page.QuerySelector("table.olt").QuerySelectorAll("tr"))
            {
                var titleItem = trItem.QuerySelector("td.title");
                if (titleItem == null || hsDoubanHouseURL.Contains(titleItem.QuerySelector("a").GetAttribute("href")))
                    continue;

                var houseInfo = new BizHouseInfo()
                {
                    HouseTitle = titleItem.QuerySelector("a").GetAttribute("title"),
                    HouseOnlineURL = titleItem.QuerySelector("a").GetAttribute("href"),
                    HouseLocation = titleItem.QuerySelector("a").GetAttribute("title"),
                    HouseText = titleItem.QuerySelector("a").GetAttribute("title"),
                    DataCreateTime = DateTime.Now,
                    PubTime = titleItem.QuerySelector("td.time") != null
                    ? DateTime.Parse(DateTime.Now.ToString("yyyy-") + titleItem.QuerySelector("td.time").InnerHtml)
                    : DateTime.Now,
                    DisPlayPrice = "",
                    Source = ConstConfigurationName.Douban,
                    HousePrice = 0,
                    LocationCityName = cityName
                };
                lstHouseInfo.Add(houseInfo);
            }
            return lstHouseInfo;

        }



        public static void AnalyzeDoubanHouseContent()
        {
            var lstHouse = dataContent.HouseInfos.Where(h =>
            h.Source ==ConstConfigurationName.Douban && h.IsAnalyzed == false).Take(100).ToList();

            foreach(var houseInfo in lstHouse)
            {
                var housePrice = JiebaTools.GetHousePrice(houseInfo.HouseText);
                if (housePrice != 0)
                {
                    houseInfo.HousePrice = housePrice;
                }
                else
                {
                    var htmlResult = HTTPHelper.GetHTML(houseInfo.HouseOnlineURL);
                    var page = htmlParser.Parse(htmlResult);
                    var topicContent = page.QuerySelector("div.topic-content");

                }
                
            }
            dataContent.SaveChanges();
        }

    }
}
