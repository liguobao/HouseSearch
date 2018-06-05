using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using RestSharp;

namespace HouseCrawler.Web
{
    public class PinPaiGongYuHouseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();

        public static List<HouseInfo> GetHouseData(string shortCutName, string cityName, string houseURL)
        {
            var html = GetHouseHTML(houseURL);
            return GetDataFromHMTL(shortCutName, cityName, html);
        }

        private static List<HouseInfo> GetDataFromHMTL(string shortCutName, string cityName, string houseHTML)
        {
            List<HouseInfo> houseList = new List<HouseInfo>();
            if (string.IsNullOrEmpty(houseHTML))
                return houseList;
            var htmlDoc = htmlParser.Parse(houseHTML);
            var logrList = htmlDoc.QuerySelectorAll("li").Where(element => element.HasAttribute("logr"));
            if (!logrList.Any())
                return houseList;
            foreach (var element in logrList)
            {
                var houseTitle = element.QuerySelector("h2").TextContent;
                var houseInfoList = houseTitle.Split(' ');
                int.TryParse(element.QuerySelector("b").TextContent, out var housePrice);
                var onlineUrl = $"http://{shortCutName}.58.com" + element.QuerySelector("a").GetAttribute("href");
                var houseInfo = new HouseInfo
                {
                    HouseTitle = houseTitle,
                    HouseOnlineURL = onlineUrl,
                    DisPlayPrice = element.QuerySelector("b").TextContent,
                    HouseLocation = new[] { "公寓", "青年社区" }.All(s => houseInfoList.Contains(s)) ? houseInfoList[0] : houseInfoList[1],
                    DataCreateTime = DateTime.Now,
                    HousePrice = housePrice,
                    HouseText = houseTitle,
                    LocationCityName = cityName,
                    PubTime = DateTime.Now
                };
                houseList.Add(houseInfo);
            }
            return houseList;
        }

        public static string GetHouseHTML(string houseURL)
        {
            var client = new RestClient(houseURL);
            var request = new RestRequest(Method.GET);
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("cookie", "f=n; f=n; f=n; id58=c5/njVqyV8E3W36xBFjjAg==; 58tj_uuid=c94df52b-df72-4d50-bddd-1afda5827d63; new_uv=1; utm_source=; spm=; init_refer=https%253A%252F%252Fwww.bing.com%252F; als=0; new_session=0; commontopbar_new_city_info=2%7C%E4%B8%8A%E6%B5%B7%7Csh; Hm_lvt_dcee4f66df28844222ef0479976aabf1=1521637318; f=n; ppStore_fingerprint=C57B6AC37AFB7EDD553E5DB2C335F4FC3E03018F34D46D58%EF%BC%BF1521637343698; Hm_lpvt_dcee4f66df28844222ef0479976aabf1=1521637344");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.186 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
            request.AddHeader("accept-encoding", "gzip, deflate");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}
