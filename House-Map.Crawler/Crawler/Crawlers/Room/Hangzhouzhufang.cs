using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using Dapper;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;
using HouseMap.Common;
using Newtonsoft.Json.Linq;
using System.Net;
using AngleSharp.Parser.Html;
using System.Text.RegularExpressions;

namespace HouseMap.Crawler
{

    public class Hangzhouzhufang : BaseCrawler
    {

        private static HtmlParser htmlParser = new HtmlParser();

        private readonly RestClient _restClient;

        public Hangzhouzhufang(HouseDapper houseDapper, ConfigDapper configDapper,
         ElasticService elastic, RedisTool redisTool) : base(houseDapper, configDapper, elastic, redisTool)
        {
            this.Source = SourceEnum.Hangzhouzhufang;
            _restClient = new RestClient();
            _restClient.BaseUrl = new Uri("http://zl.hzfc.gov.cn");

        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var request = new RestRequest("/webrent/gphouselist.htm", Method.POST);
            var p = page + 1;
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7,zh-TW;q=0.6");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("referer", "http://zl.hzfc.gov.cn/webrent/gphouselist.htm");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("origin", "http://zl.hzfc.gov.cn");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            request.AddParameter("application/x-www-form-urlencoded", $"begintime=&endtime=&keywords=&page={p}&areaid=&hxs=&jzmjs=&zjjes=&czfs=&sblb=1&paixu=21&sfcz=0&myj=", ParameterType.RequestBody);
            IRestResponse response = _restClient.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var jsonData = JToken.Parse(config.Json);
            var city = config.City;
            var htmlDoc = htmlParser.Parse(data);
            var xiaoquShow = htmlDoc?.QuerySelector("div.xiaoquShow");
            if (xiaoquShow == null)
            {
                return houses;
            }
            foreach (var item in xiaoquShow.QuerySelectorAll("a"))
            {
                var title = item.QuerySelector("div.xiaoquRight")?.QuerySelector("div.title")?.TextContent;
                if (string.IsNullOrEmpty(title))
                {
                    continue;
                }
                var house = new DBHouse();
                house.Location = title;
                house.Title = title;
                var resource = item.GetAttribute("href");
                house.OnlineURL = $"http://zl.hzfc.gov.cn{resource}";
                house.JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(new { html = item.OuterHtml });
                house.Source = SourceEnum.Hangzhouzhufang.GetSourceName();
                if (!string.IsNullOrEmpty(item.QuerySelector("font.f16")?.TextContent))
                {
                    house.Price = int.Parse(item.QuerySelector("font.f16")?.TextContent);
                }
                house.Id = Tools.GetGuid();
                house.City = city;
                house.RentType = (int)RentTypeEnum.AllInOne;
                house.PubTime = string.IsNullOrEmpty(item.QuerySelector("div.time")?.TextContent) ?
                 DateTime.Now : DateTime.Parse(item.QuerySelector("div.time")?.TextContent.Replace("", ""));
                house.Text = item.TextContent.Replace("\t", "").Replace("\n\n", "\n").Replace("   ", " ");
                FillTextAndPicURLs(house, resource);
                houses.Add(house);
            }
            return houses;
        }

        private void FillTextAndPicURLs(DBHouse house, string resource)
        {
            var roomDetailHTML = GetOne(resource);
            if (!string.IsNullOrEmpty(roomDetailHTML))
            {
                var roomDetail = htmlParser.Parse(roomDetailHTML);
                house.Text = house.Text + roomDetail.QuerySelectorAll("div.contentCenter").Select(d => d.TextContent)
                .FirstOrDefault(div => div.Contains("挂牌人信息")).Replace("\t", "").Replace("\n\n", "\n").Replace("   ", " ");
                var imgSource = "/rent/WebRentAction_selectFwzpList.jspx?ztcode=" + GetZtcode(roomDetailHTML);
                var imgJsonResult = GetOne(imgSource);
                if (!string.IsNullOrEmpty(imgJsonResult))
                {
                    var imageList = JToken.Parse(imgJsonResult)["list"].Select(i => "http://zl.hzfc.gov.cn/" + i["wllj"]?.ToString()).ToList();
                    house.PicURLs = JsonConvert.SerializeObject(imageList);
                }

            }
        }

        public static string GetZtcode(string roomDetailHTML)
        {
            var ztcodeRegex = new Regex(@"ztcode=([0-9]+)");
            var m = ztcodeRegex.Match(roomDetailHTML);
            if (!m.Success)
            {
                return string.Empty;
            }
            return m.Groups[1].Value;
        }


        public string GetOne(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7,zh-TW;q=0.6");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            IRestResponse response = _restClient.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }
    }
}