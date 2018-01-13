using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;
using HouseCrawler.Web.DAL;
using HouseCrawler.Core.DBService.DAL;
using HouseCrawler.Core.DataContent;

namespace HouseCrawler.Core
{
    public class DoubanHouseCrawler
    {
        private static readonly HtmlParser HtmlParser = new HtmlParser();

        private static readonly CrawlerDataContent DataContent = new CrawlerDataContent();

        private static CrawlerDAL crawlerDAL = new CrawlerDAL();

        public static void CaptureHouseInfoFromConfig()
        {
            try
            {
                int captrueHouseCount = 0;
                foreach (var doubanConf in DataContent.CrawlerConfigurations
                    .Where(c => c.ConfigurationName == ConstConfigurationName.Douban).ToList())
                {
                    LogHelper.RunActionNotThrowEx(() =>
                    {
                        var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);

                        for (var pageIndex = 0; pageIndex < confInfo.pagecount.Value; pageIndex++)
                        {
                            var lstHouseInfo = GetDataFromAPI(confInfo.groupid.Value,
                                confInfo.cityname.Value, pageIndex);
                            DataContent.DoubanHouseInfos.AddRange(lstHouseInfo);
                            DataContent.SaveChanges();
                            captrueHouseCount = captrueHouseCount + lstHouseInfo.Count;
                        }
                    }, "CaptureHouseInfoFromConfig", doubanConf);

                }
                HouseSourceInfo.RefreshHouseSourceInfo();

                BizCrawlerLog.SaveLog("爬取豆瓣租房数据", $"本次共爬取到{captrueHouseCount}条数据。", 1);
            }
            catch (Exception ex)
            {
                LogHelper.Error("DoubanHouseCrawler CrawlerHouseInfo Exception", ex);
            }
        }

        public static void AddDoubanGroupConfig(string groupID, string cityName, int pageIndex = 0)
        {
            var cityInfo = $"{{ 'groupid':'{groupID}','cityname':'{cityName}','pagecount':5}}";

            var doubanConfig = DataContent.CrawlerConfigurations
                .FirstOrDefault(c => c.ConfigurationName == ConstConfigurationName.Douban && c.ConfigurationValue == cityInfo);
            if (doubanConfig != null)
                return;
            var lstHouseInfo = GetDataFromAPI(groupID, cityName, pageIndex);
            #region add douban group config

            if (lstHouseInfo.Count > 0)
            {
                var config = new BizCrawlerConfiguration()
                {
                    ConfigurationKey = 0,
                    ConfigurationValue = cityInfo,
                    ConfigurationName = ConstConfigurationName.Douban,
                    DataCreateTime = DateTime.Now,
                    IsEnabled = true,
                };
                DataContent.AddRange(lstHouseInfo);
                DataContent.Add(config);
                DataContent.SaveChanges();

                HouseSourceInfo.RefreshHouseSourceInfo();
            }
            #endregion

        }


        public static List<DoubanHouseInfo> GetDataFromAPI(string groupID, string cityName, int pageIndex)
        {
            List<DoubanHouseInfo> lstHouseInfo = new List<DoubanHouseInfo>();
            var apiURL = $"https://api.douban.com/v2/group/{groupID}/topics?start={pageIndex * 50}";
            var doubanTopic = WebAPIHelper.GetAPIResult<DoubanTopic>(apiURL);
            if (doubanTopic != null && doubanTopic.topics != null)
            {
                foreach (var topic in doubanTopic.topics)
                {
                    if (DataContent.DoubanHouseInfos.Any(h => h.HouseOnlineURL == topic.share_url))
                        continue;
                    var housePrice = JiebaTools.GetHousePrice(topic.content);
                    var house = new DoubanHouseInfo()
                    {
                        HouseLocation = topic.title,
                        HouseTitle = topic.title,
                        HouseOnlineURL = topic.share_url,
                        HouseText = topic.content,
                        HousePrice = JiebaTools.GetHousePrice(topic.content),
                        IsAnalyzed = true,
                        DisPlayPrice = housePrice > 0 ? $"{housePrice}元" : "",
                        Source = ConstConfigurationName.Douban,
                        LocationCityName = cityName,
                        Status = 1,
                        PubTime = DateTime.Parse(topic.created),
                        DataCreateTime = DateTime.Now,
                    };
                    lstHouseInfo.Add(house);
                }
             }
            return lstHouseInfo;
        }


        private static void AnalyzeFromWebPage(Web.Model.DBHouseInfo houseInfo,
            ref decimal housePrice, ref string houseTextContent)
        {
            var htmlResult = DoubanHTTPHelper.GetHTMLForDouban(houseInfo.HouseOnlineURL);
            //没有页面信息
            if (string.IsNullOrEmpty(htmlResult))
            {
                //404页面
                houseInfo.Status = 2;
            }
            else
            {
                var page = HtmlParser.Parse(htmlResult);
                var topicContent = page.QuerySelector("div.topic-content");
                //没有帖子内容
                if (topicContent == null || topicContent.QuerySelector("p") == null || topicContent.QuerySelector("p") == null)
                {
                    houseInfo.Status = 3;
                }
                else
                {
                    //获取帖子内容
                    houseTextContent = topicContent.QuerySelector("p").TextContent;
                    //获取价格信息
                    housePrice = JiebaTools.GetHousePrice(houseTextContent);
                    if (housePrice != 0 || !string.IsNullOrEmpty(houseTextContent))
                    {
                        houseInfo.Status = 1;
                    }
                    houseInfo.DisPlayPrice = housePrice.ToString(CultureInfo.InvariantCulture);
                    houseInfo.HousePrice = housePrice;
                    houseInfo.HouseText = houseTextContent;
                }
            }
        }
    }
}
