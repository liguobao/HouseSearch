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

        public static void CrawlerHouseInfo()
        {
            var doubanConf = dataContent.CrawlerConfigurations.FirstOrDefault();
            if(doubanConf!=null)
            {
                var doubanConfInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigconfigurationValue);
                for(var index =0;index< doubanConfInfo.pagecount.Value;index++)
                {
                    GetDataFromOnlineWeb(doubanConfInfo.groupid.Value,index,
                  doubanConfInfo.cityname.Value);
                }

              
            }

        }


        public static void GetDataFromOnlineWeb(string groupID,int index,string cityName)
        {
            var url = $"https://www.douban.com/group/{groupID}/discussion?start={index * 25}";
            var htmlResult = HTTPHelper.GetHTML(url);
            var page = htmlParser.Parse(htmlResult);
            page.QuerySelectorAll("td.title").ToList().ForEach(item => {

                if (item.QuerySelector("a").GetAttribute("title").Contains("\xE3\x80\x90\xE5\xAE\x8B"))
                {
                    string vu = item.QuerySelector("a").GetAttribute("title");
                }
               var houseInfo = new BizHouseInfo()
                {
                    HouseTitle = item.QuerySelector("a").GetAttribute("title"),
                    HouseOnlineURL = item.QuerySelector("a").GetAttribute("href"),
                    HouseLocation = item.QuerySelector("a").GetAttribute("title"),
                    HouseText = item.QuerySelector("a").GetAttribute("title"),
                    DataCreateTime = DateTime.Now,
                    PubTime= item.QuerySelector("td.time")!=null
                    ? DateTime.Parse(DateTime.Now.ToString("yyyy-") + item.QuerySelector("td.time").InnerHtml) 
                    :DateTime.Now,
                    DisPlayPrice = "",
                    SoureceDaminURL= "www.douban.com",
                    HousePrice=0,
                    LocationCityName= cityName
               };
               dataContent.Add(houseInfo);
            });

            dataContent.SaveChanges();
        }

    }
}
