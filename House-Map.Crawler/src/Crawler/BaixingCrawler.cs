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

    public class BaixingCrawler : BaseCrawler,ICrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();
        public BaixingCrawler(HouseDapper houseDapper, ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = ConstConfigName.BaiXing;
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            var url = $"http://{config["shortcutname"].ToString()}.baixing.com/zhengzu/?grfy=1&page={page}";
            return GetHouseHTML(url);
        }

        public static string GetHouseHTML(string houseURL)
        {
            var client = new RestClient(houseURL);
            var request = new RestRequest(Method.GET);
            //request.AddHeader("if-modified-since", "Wed, 30 May 2018 09:14:14 GMT");
            //request.AddHeader("cookie", "__trackId=152758449742434; __city=maoming; _ga=GA1.2.1113154012.1527584498; _gid=GA1.2.576443783.1527584498; __admx_track_id=nARqVnEeXTO1SAwEFpGgPQ; __admx_track_id.sig=Ib7MLyENlAb0yN8oWCgNFfR7-68; appVersion=5.7.1; __smartphone=1527584790; __location=maoming; Hm_lvt_767685c7b1f25e1d49aa5a5f9555dc7d=1527584792,1527666095; Hm_lvt_5a727f1b4acc5725516637e03b07d3d2=1527584502,1527666102; __s=pia04bfj6o8ffd6gtmo3vppau1; Hm_lpvt_767685c7b1f25e1d49aa5a5f9555dc7d=1527670231; _gat=1; __sense_session_pv=12; Hm_lpvt_5a727f1b4acc5725516637e03b07d3d2=1527671656");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        public override List<HouseInfo> ParseHouses(JToken config, string jsonOrHTML)
        {
            var houseList = new List<HouseInfo>();
            if (string.IsNullOrEmpty(jsonOrHTML))
                return houseList;
            var htmlDoc = htmlParser.Parse(jsonOrHTML);

            var houseItems = htmlDoc.QuerySelectorAll("li.listing-ad.item-regular");

            if (!houseItems.Any())
                return houseList;
            var cityName = config["cityname"]?.ToString();
            foreach (var item in houseItems)
            {
                var element = item.QuerySelector("div.media-body");
                if (element == null)
                {
                    continue;
                }
                var disPlayPrice = element.QuerySelector("span.highlight").TextContent;
                int.TryParse(disPlayPrice.Replace("元", ""), out var housePrice);
                var adTitle = element.QuerySelector("a.ad-title");
                var detailList = element.QuerySelectorAll("div.ad-item-detail");
                var descTitle = detailList[0];
                var localTitle = detailList[1];
                if (localTitle.QuerySelector("a.source") != null)
                {
                    localTitle.QuerySelector("a.source").Remove();
                }
                if (localTitle.QuerySelector("time") != null)
                {
                    localTitle.QuerySelector("time").Remove();
                }
                var houseLocation = localTitle.TextContent.Replace("来自：", "").Trim();
                var houseInfo = new HouseInfo
                {
                    HouseTitle = adTitle.TextContent + houseLocation,
                    HouseOnlineURL = adTitle.GetAttribute("href"),
                    DisPlayPrice = disPlayPrice,
                    HouseLocation = houseLocation,
                    Source = ConstConfigName.BaiXing,
                    HousePrice = housePrice,
                    HouseText = element.InnerHtml,
                    LocationCityName = cityName,
                    PubTime = DateTime.Now,
                    PicURLs = GetPhotos(item)
                };
                houseList.Add(houseInfo);
            }
            return houseList;
        }

        private static String GetPhotos(IElement element)
        {
            var photos = new List<String>();
            var imageURL = element.QuerySelector("img")?.GetAttribute("src");
            if (imageURL != null)
            {
                photos.Add("https:" + imageURL);
            }
            return JsonConvert.SerializeObject(photos);
        }

    }
}