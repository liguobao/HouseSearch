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

        public static void CaptureHouseInfo()
        {
            try
            {

                foreach (var doubanConf in dataContent.CrawlerConfigurations.Where(c => c.ConfigurationName
                == ConstConfigurationName.Douban).ToList())
                {
                    var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);
                    for (var index = 0; index < confInfo.pagecount.Value; index++)
                    {
                        GetDataFromOnlineWeb(confInfo.groupid.Value, index,
                            confInfo.cityname.Value);
                    }
                }

            }
            catch(Exception ex)
            {
                LogHelper.Error("DoubanHouseCrawler CrawlerHouseInfo Exception", ex);
            }
        }

        public static void GetDataFromOnlineWeb(string groupID,int index,string cityName)
        {
            var url = $"https://www.douban.com/group/{groupID}/discussion?start={index * 25}";
            var htmlResult = HTTPHelper.GetHTML(url);
            if (string.IsNullOrEmpty(htmlResult))
                return;
            var page = htmlParser.Parse(htmlResult);

            foreach(var trItem in page.QuerySelector("table.olt").QuerySelectorAll("tr"))
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
                    SoureceDaminURL = "www.douban.com",
                    HousePrice = 0,
                    LocationCityName = cityName
                };
                dataContent.Add(houseInfo);

            }
            dataContent.SaveChanges();
        }

    }
}
