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
using HouseMap.Common;

namespace HouseMap.Crawler
{

    public class BaixingWechat : BaseCrawler
    {
        private static HtmlParser htmlParser = new HtmlParser();
        public BaixingWechat(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService, RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            this.Source = SourceEnum.BaixingWechat;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var configJson = JToken.Parse(config.Json);
            var baixingPage = page + 1;
            return GetHouseListResult(configJson["areaId"].ToString(), baixingPage, configJson["session"].ToString());
        }

        private static string GetHouseListResult(string areaId, int page, string session)
        {
            var client = new RestClient("https://mpapi.baixing.com/v1.2.10/");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "ecffafaf-842f-3d09-1f03-f68ed0e47be5");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("host", "mpapi.baixing.com");
            request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36 MicroMessenger/6.7.3.1360(0x26070333) NetType/WIFI Language/zh_CN Process/toolsmp");
            request.AddHeader("env_version", "6.7.3");
            request.AddHeader("network_type", "wifi");
            request.AddHeader("source_path", "pages/index");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("model", "MIX");
            request.AddHeader("source", "1001");
            //request.AddHeader("baixing-session", "$2y$10$Cz9H5ib/ZKh0UOZxVp2rCOeiBjK7Y7/ZmOuUipdZ65QPhms7DpGD2");
            request.AddHeader("baixing-session", session);
            request.AddHeader("os_version", "8.0.0");
            request.AddHeader("os", "Android");
            request.AddHeader("template_version", "Ver1.2.10");
            request.AddHeader("referer", "https://servicewechat.com/wxd9808e2433a403ab/34/page-frame.html");
            request.AddHeader("charset", "utf-8");
            request.AddParameter("application/json", "{\"listing.getAds\":{\"areaId\":\"" + areaId + "\",\"categoryId\":\"zhengzu\",\"page\":" + page + ",\"size\":500,\"notAllowChatOnly\":1}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        private static string GetHouseResult(string houseId, string session)
        {
            var client = new RestClient("https://mpapi.baixing.com/v1.2.10/");
            var request = new RestRequest(Method.POST);
            request.AddHeader("host", "mpapi.baixing.com");
            request.AddHeader("user-agent", "Mozilla/5.0 (Linux; Android 8.0.0; MIX Build/OPR1.170623.032; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/69.0.3497.100 Mobile Safari/537.36 MicroMessenger/6.7.3.1360(0x26070333) NetType/WIFI Language/zh_CN Process/toolsmp");
            request.AddHeader("env_version", "6.7.3");
            request.AddHeader("network_type", "wifi");
            request.AddHeader("source_path", "pages/index");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("model", "MIX");
            request.AddHeader("baixing-session", session);
            request.AddHeader("os_version", "8.0.0");
            request.AddHeader("os", "Android");
            request.AddHeader("template_version", "Ver1.2.10");
            request.AddHeader("referer", "https://servicewechat.com/wxd9808e2433a403ab/34/page-frame.html");
            request.AddHeader("charset", "utf-8");
            request.AddParameter("application/json", "{\"viewAd.getVad\":{\"adId\":\"" + houseId + "\"}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            return response.Content;

        }

        public override List<DBHouse> ParseHouses(DBConfig config, string jsonOrHTML)
        {
            var houses = new List<DBHouse>();
            if (jsonOrHTML.Contains("html>") || jsonOrHTML.Contains("<script>"))
            {
                return houses;
            }
            var result = JToken.Parse(jsonOrHTML);
            if (result?["status"].ToString() != "success" || result?["info"]["listing"]["getAds"]["data"] == null)
            {
                return houses;
            }
            var configJson = JToken.Parse(config.Json);
            foreach (var room in result?["info"]["listing"]["getAds"]["data"])
            {
                var roomId = room["id"].ToString();
                var roomDetailResult = GetHouseResult(roomId, configJson["session"].ToString());
                if (string.IsNullOrEmpty(roomDetailResult))
                {
                    continue;
                }
                //Console.WriteLine(room.ToString());
                var roomDetail = JToken.Parse(roomDetailResult)["info"]["viewAd"]["getVad"];
                //Console.WriteLine(roomDetail["updateTimeString"].ToString());
                if (CheckRoom(roomDetail))
                {
                    continue;
                }
                DBHouse house = ConvertToHouse(room, roomDetailResult, roomDetail);
                if (house == null)
                    continue;
                houses.Add(house);
            }
            return houses;
        }

        private static DBHouse ConvertToHouse(JToken room, string roomDetailResult, JToken roomDetail)
        {
            try
            {
                var house = new DBHouse();
                house.Id = Tools.GetGuid();
                house.Title = roomDetail["title"]?.ToString();
                house.Location = roomDetail["address"]?.ToString();
                house.Text = roomDetail["description"]?.ToString();
                house.PicURLs = roomDetail["bigImages"].ToString();
                house.Price = int.Parse(roomDetail["highlightMeta"]?["value"].ToString().Replace("元", ""));
                house.Source = SourceEnum.BaixingWechat.GetSourceName();
                house.PubTime = GetPubTime(roomDetail["updateTimeString"].ToString());
                house.JsonData = roomDetailResult.ToString();
                house.Labels = string.Join("|", roomDetail["labelMetas"].Select(l => l["value".ToString()]));
                house.Tags = string.Join("|", roomDetail["metas"].Select(l => l["value"]?.ToString()).Where(v => !string.IsNullOrEmpty(v)));
                house.City = roomDetail["cityCName"]?.ToString();
                house.OnlineURL = $"http://{roomDetail["cityEnglishName"].ToString()}.baixing.com/zhengzu/a{room["id"].ToString()}.html";
                return house;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ConvertToHouse", ex, roomDetailResult);
                return null;
            }

        }

        private static DateTime GetPubTime(string updateTimeString)
        {
            var pubTime = DateTime.Now;
            if (updateTimeString.Contains("秒"))
            {
                pubTime = pubTime.AddSeconds(-int.Parse(updateTimeString.Replace("秒前", "")));
            }
            else if (updateTimeString.Contains("分钟"))
            {
                pubTime = pubTime.AddMinutes(-int.Parse(updateTimeString.Replace("分钟前", "")));
            }
            else if (updateTimeString.Contains("小时"))
            {
                pubTime = pubTime.AddHours(-int.Parse(updateTimeString.Replace("小时前", "")));
            }
            else if (updateTimeString.Contains("天"))
            {
                pubTime = pubTime.AddDays(-int.Parse(updateTimeString.Replace("天前", "")));
            }

            return pubTime;
        }

        private static bool CheckRoom(JToken roomDetail)
        {
            return roomDetail["userName"].ToString().Contains("经纪人") || roomDetail["userName"].ToString().Contains("地产") || roomDetail["certifications"] == null
                            || !roomDetail["certifications"].Any(i => i["type"].ToString() == "idcard" && i["isBind"].ToObject<bool>());
        }

    }
}