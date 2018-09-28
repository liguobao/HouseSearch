using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Dapper;
using AngleSharp.Dom;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;

using Newtonsoft.Json.Linq;
using HouseMap.Models;

namespace HouseMap.Crawler
{


    public class ChengdufgjCrawler : BaseCrawler,ICrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();
        public ChengdufgjCrawler( HouseDapper houseDapper, ConfigService configService) : base(houseDapper, configService)
        {
            this.Source = ConstConfigName.Chengdufgj;
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            var path = config["path"]?.ToString();
            var url = $"http://zf.cdfgj.gov.cn/{path}page={page}";
            return GetHouseHTML(url);
        }

        public override List<HouseInfo> ParseHouses(JToken config, string data)
        {
            var cityName = config["cityname"]?.ToString();
            List<HouseInfo> houseList = new List<HouseInfo>();
            if (string.IsNullOrEmpty(data))
                return houseList;
            var htmlDoc = htmlParser.Parse(data);
            var houseItems = htmlDoc.QuerySelectorAll("div.pan-item.clearfix");
            if (!houseItems.Any())
                return houseList;
            foreach (var item in houseItems)
            {
                HouseInfo houseInfo = ConvertToHouse(cityName, item);
                houseList.Add(houseInfo);
            }
            return houseList;
        }

        private static HouseInfo ConvertToHouse(string cityName, IElement item)
        {
            int housePrice = GetHousePrice(item);
            string houseLocation = GetLocation(item);
            var titleItem = item.QuerySelector("h2");
            var pubTime = GetPubTime(item);
            string houseURL = GetHouseURL(titleItem);
            var houseInfo = new HouseInfo
            {
                HouseTitle = titleItem.TextContent.Replace("\n", "").Trim() + houseLocation,
                HouseOnlineURL = "http://zf.cdfgj.gov.cn" + houseURL,
                DisPlayPrice = housePrice + "元/月",
                HouseLocation = houseLocation,
                Source = ConstConfigName.Chengdufgj,
                HousePrice = housePrice,
                HouseText = item.InnerHtml,
                LocationCityName = cityName,
                PubTime = pubTime,
                PicURLs = GetPhotos(item)
            };
            return houseInfo;
        }

        private static string GetHouseURL(IElement titleItem)
        {
            var hrefList = titleItem.QuerySelector("a").GetAttribute("href").Split(";");
            return hrefList.Count() > 0 
            ? hrefList[0] 
            : titleItem.QuerySelector("a").GetAttribute("href");

        }


        private static DateTime GetPubTime(IElement item)
        {
            var pubTimeItems = item.QuerySelectorAll("p.p_gx");
            var pubTimetext = "";
            if (pubTimeItems != null && pubTimeItems.Count() > 0)
            {
                pubTimetext = pubTimeItems.Count() > 1
                ? pubTimeItems[1].TextContent.Replace("\n", "").Replace("发布时间：", "").Trim()
                : pubTimeItems[0].TextContent.Replace("\n", "").Replace("发布时间：", "").Trim();
            }
            var pubTime = DateTime.Now;
            DateTime.TryParse(pubTimetext, out pubTime);
            return pubTime;
        }

        private static string GetLocation(IElement item)
        {
            var locationItem = item.QuerySelectorAll("p").Where(p => !p.ClassList.Any()).FirstOrDefault();
            return locationItem?.TextContent.Replace("\n", "").Replace(" ", "").Trim();
        }

        private static int GetHousePrice(IElement item)
        {
            var disPlayPriceItem = item.QuerySelector("strong.rent-price");
            if (disPlayPriceItem == null)
            {
                disPlayPriceItem = item.QuerySelector("strong.current-price");
            }
            var disPlayPrice = disPlayPriceItem?.TextContent.Replace("元/月", "").Replace("\n", "").Replace("当前价：", "").Trim();
            int.TryParse(disPlayPrice, out var housePrice);
            return housePrice;
        }

        private static String GetPhotos(IElement element)
        {
            var photos = new List<String>();
            var imageURL = element.QuerySelector("img")?.GetAttribute("src");
            if (imageURL != null)
            {
                photos.Add(imageURL.Replace(".170x130.jpg", ""));
            }
            return JsonConvert.SerializeObject(photos);
        }
        public static string GetHouseHTML(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}