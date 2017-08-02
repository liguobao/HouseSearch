using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;
using HouseCrawler.Web.DAL;
using HouseCrawler.Core.DBService.DAL;

namespace HouseCrawler.Core
{
    public class DoubanHouseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();

        private static CrawlerDataContent dataContent = new CrawlerDataContent();

        private static CrawlerDAL crawlerDAL = new CrawlerDAL();

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

            List<BizHouseInfo> lstHouseInfo = new List<BizHouseInfo>();

            var url = $"https://www.douban.com/group/{groupID}/discussion?start={pageIndex * 25}";
            var htmlResult = HTTPHelper.GetHTML(url);
            if (string.IsNullOrEmpty(htmlResult))
                return lstHouseInfo;
            var page = htmlParser.Parse(htmlResult);

            foreach (var trItem in page.QuerySelector("table.olt").QuerySelectorAll("tr"))
            {
                var titleItem = trItem.QuerySelector("td.title");
                if (titleItem == null || crawlerDAL.ContainsHouseOnlineURL(titleItem.QuerySelector("a").GetAttribute("href")))
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

        public static void AnalyzeDoubanHouseContentAll()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Console.WriteLine("AnalyzeDoubanHouseContent Start...");
            int index = 0;
            try
            {
                var dal = new DBHouseInfoDAL();

                var lstHouse = dal.LoadUnAnalyzeList();

                foreach (var houseInfo in lstHouse)
                {
                    var housePrice = JiebaTools.GetHousePrice(houseInfo.HouseText);
                    string houseTextContent = string.Empty;
                    if (housePrice == 0)
                    {
                        AnalyzeFromWebPage(houseInfo, ref housePrice, ref houseTextContent);
                    }
                    else
                    {
                        houseInfo.Status = 1;
                        houseInfo.DisPlayPrice = housePrice.ToString();
                        houseInfo.HousePrice = housePrice;
                    }

                    houseInfo.IsAnalyzed = true;
                    dal.UpdateHouseInfo(houseInfo);
                    index++;

                     Console.WriteLine("HouseInfo:" + Newtonsoft.Json.JsonConvert.SerializeObject(houseInfo));
                }
               
            }
            catch (Exception ex)
            {
                LogHelper.Error("AnalyzeDoubanHouseContent Exception", ex);
            }

            sw.Stop();

            string copyTime = sw.Elapsed.TotalSeconds.ToString();

            Console.WriteLine("AnalyzeDoubanHouseContent Finish,Update Count:" + index +";花费时间：" + copyTime);

        }

        private static void AnalyzeFromWebPage(Web.Model.DBHouseInfo houseInfo,
            ref decimal housePrice, ref string houseTextContent)
        {
            var htmlResult = HTTPHelper.GetHTML(houseInfo.HouseOnlineURL);
            //没有页面信息
            if (string.IsNullOrEmpty(htmlResult))
            {
                //404页面
                houseInfo.Status = 2;
            }
            else
            {
                var page = htmlParser.Parse(htmlResult);
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
                    houseInfo.DisPlayPrice = housePrice.ToString();
                    houseInfo.HousePrice = housePrice;
                    houseInfo.HouseText = houseTextContent;
                }
            }
        }
    }
}
