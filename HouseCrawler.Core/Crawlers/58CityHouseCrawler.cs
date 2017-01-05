using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

namespace HouseCrawler.Core
{
    public class _58CityHouseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();

        private static CrawlerDataContent dataContent = new CrawlerDataContent();

        public static void CapturPinPaiHouseInfo()
        {
            foreach (var doubanConf in dataContent.CrawlerConfigurations.Where(c => c.ConfigurationName
               == ConstConfigurationName.PinPaiGongYu && c.IsEnabled).ToList())
            {
                var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);
                for (var index = 0; index < confInfo.pagecount.Value; index++)
                {
                    var url = $"http://{confInfo.shortcut.Value}.58.com/pinpaigongyu/pn/{index}";
                    var htmlResult = HTTPHelper.GetHTMLByURL(url);
                    var page = new HtmlParser().Parse(htmlResult);
                    var lstLi = page.QuerySelectorAll("li").Where(element => element.HasAttribute("logr"));
                    if ((lstLi == null || lstLi.Count() == 0) && index == 0)
                    {
                        doubanConf.IsEnabled = false;
                        break;
                    }

                    GetDataOnPageDoc(confInfo, page);

                    dataContent.SaveChanges();

                }
            }
        }

        private static void GetDataOnPageDoc(dynamic confInfo, AngleSharp.Dom.Html.IHtmlDocument page)
        {
            foreach (var element in page.QuerySelectorAll("li").Where(element => element.HasAttribute("logr")))
            {
                var houseTitle = element.QuerySelector("h2").TextContent;
                var houseInfoList = houseTitle.Split(' ');
                int housePrice = 0;
                int.TryParse(element.QuerySelector("b").TextContent, out housePrice);
                var onlineURL = $"http://{confInfo.shortcut.Value}.58.com" + element.QuerySelector("a").GetAttribute("href");
                if (dataContent.HouseInfos.Any(h => h.HouseOnlineURL == onlineURL))
                    continue;
                var houseInfo = new BizHouseInfo
                {
                    HouseTitle = houseTitle,
                    HouseOnlineURL = onlineURL,
                    DisPlayPrice = element.QuerySelector("b").TextContent,
                    HouseLocation = new[] { "公寓", "青年社区" }.All(s => houseInfoList.Contains(s)) ? houseInfoList[0] : houseInfoList[1],
                    DataCreateTime = DateTime.Now,
                    SoureceDaminURL = ConstConfigurationName.PinPaiGongYu,
                    HousePrice = housePrice,
                    HouseText = houseTitle,
                    LocationCityName = confInfo.cityname.Value,
                    PubTime = DateTime.Now
                };
                dataContent.Add(houseInfo);
            }
        }
    }
}
