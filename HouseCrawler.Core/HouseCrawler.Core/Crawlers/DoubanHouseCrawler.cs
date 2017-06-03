using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

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

                foreach (var doubanConf in dataContent.CrawlerConfigurations
                    .Where(c => c.ConfigurationName == ConstConfigurationName.Douban).ToList())
                {
                    var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);
                    for (var pageIndex = 0; pageIndex < confInfo.pagecount.Value; pageIndex++)
                    {
                        var lstHouseInfo = GetDataFromOnlineWeb(confInfo.groupid.Value, confInfo.cityname.Value, pageIndex);
                        dataContent.Add(lstHouseInfo);
                        dataContent.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("DoubanHouseCrawler CrawlerHouseInfo Exception", ex);
            }
        }

        public static void AddDoubanGroupConfig(string groupID, string cityName, int pageIndex = 0)
        {
            var cityInfo = $"{{ 'groupid':'{groupID}','cityname':'{cityName}','pagecount':5}}";

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
                dataContent.Add(config);
                dataContent.SaveChanges();
            }
            #endregion


        }


        public static List<BizHouseInfo> GetDataFromOnlineWeb(string groupID, string cityName, int pageIndex)
        {
            List<BizHouseInfo> lstHouseInfo = new List<BizHouseInfo>();


            var url = $"https://www.douban.com/group/{groupID}/discussion?start={pageIndex * 25}";
            var htmlResult = HTTPHelper.GetHTML(url);
            if (string.IsNullOrEmpty(htmlResult))
                return lstHouseInfo;
            var page = htmlParser.Parse(htmlResult);

            foreach (var trItem in page.QuerySelector("table.olt").QuerySelectorAll("tr"))
            {
                var titleItem = trItem.QuerySelector("td.title");
                if (titleItem == null)
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
                    Sourece = ConstConfigurationName.Douban,
                    HousePrice = 0,
                    LocationCityName = cityName
                };
                lstHouseInfo.Add(houseInfo);
            }
            return lstHouseInfo;

        }

    }
}
