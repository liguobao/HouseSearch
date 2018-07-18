using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HouseCrawler.Core
{
    public class ZuberHouseCrawler
    {
        static string API_VERSION = "v3/";
        static string SCENE = "2567a5ec9705eb7ac2c984033e06189d";


        private HouseDapper houseDapper;
        private ConfigDapper configDapper;
        public ZuberHouseCrawler(HouseDapper houseDapper, ConfigDapper configDapper)
        {
            this.houseDapper = houseDapper;
            this.configDapper = configDapper;
        }

        public void Run()
        {
            foreach (var doubanConf in configDapper.GetList(ConstConfigName.Zuber))
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
                    var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(doubanConf.ConfigurationValue);
                    var cityName = confInfo.cityname.Value;
                    var sequence = "";
                    for (var i = 0; i <= confInfo.pagecount.Value; i++)
                    {
                        var tupleResult = GetHouseData(cityName, sequence);
                        sequence = tupleResult.Item2;
                        houses.AddRange(tupleResult.Item1);
                    }
                    houseDapper.BulkInsertHouses(houses);
                }, "DoubanHouseCrawler CaptureHouseInfo ", doubanConf);
            }
        }


        private static Tuple<List<BaseHouseInfo>, string> GetHouseData(string cityName, string sequence)
        {
            List<BaseHouseInfo> lstHouse = new List<BaseHouseInfo>();
            LogHelper.Debug($"city:{cityName},sequence:{sequence}");
            var result = GetAPIResult(cityName, sequence);
            if (string.IsNullOrEmpty(result))
            {
                return Tuple.Create<List<BaseHouseInfo>, string>(lstHouse, "");
            }
            var nextSequence = "";
            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);
            if (resultJObject["code"].ToString() == "0" && resultJObject["result"] != null)
            {
                if (resultJObject["result"]["has_next_page"].ToObject<bool>())
                {
                    nextSequence = resultJObject["result"]["sequence"].ToObject<string>();
                }
                foreach (var item in resultJObject["result"]["items"])
                {
                    var room = item["room"];
                    var housePrice = room["cost1"].ToObject<decimal>();
                    var house = new BaseHouseInfo()
                    {
                        HouseLocation = room["address"].ToString(),
                        HouseTitle = room["summary"].ToString(),
                        HouseOnlineURL = $"http://www.zuber.im/app/room/{room["id"].ToString()}",
                        HouseText = item.ToString(),
                        HousePrice = housePrice,
                        IsAnalyzed = true,
                        DisPlayPrice = housePrice > 0 ? $"{housePrice}元" : "",
                        Source = ConstConfigName.Zuber,
                        LocationCityName = cityName,
                        Status = 1,
                        PicURLs = GetPhotos(room),
                        PubTime = room["last_modify_time"].ToObject<DateTime>()
                    };
                    lstHouse.Add(house);
                }
            }

            return Tuple.Create<List<BaseHouseInfo>, string>(lstHouse, nextSequence);
        }

        private static string GetPhotos(JToken room)
        {
            var photos = new List<String>();
            if (room["photo"] != null)
            {
                photos.Add(room["photo"].ToString().Replace("?imageView2/0/w/300", ""));
            }
            return JsonConvert.SerializeObject(photos);
        }
        private static string GetAPIResult(string cityName, string sequence)
        {
            try
            {
                var referer = "http://www.zuber.im/app/?house=1";
                string url = $"https://api.zuber.im/v3/rooms/search?city={cityName}&house=1&sex=&cost1=&cost2=&address=&start_time=&end_time=&longitude=&latitude=&subway_line=&subway_station=&short_rent=&type=&bed=true&room_type_affirm=&coords_type=gaode&sequence={sequence}";
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);
                request.AddHeader("connection", "keep-alive");
                request.AddHeader("referer", referer);
                request.AddHeader("accept", "application/json, text/plain, */*");
                request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("origin", "http://www.zuber.im");
                var _stamp = GetTimestamp().ToString();
                var _secret = "";
                var _token = md5(_stamp);
                var _scene = SCENE;
                var _method = "get";
                var _param = "{}";//(_method === 'get' || _method === 'delete') ? '{}' : JSON.stringify(param)
                var api = "rooms/search";
                var _source = "request_url=" + API_VERSION + api + "&" +
                    "content=" + _param + "&" +
                    "request_method=" + _method + '&' +
                    "timestamp=" + _stamp + '&' +
                    "secret=" + _secret;
                var _signature = md5(_source);
                var _auth = "timestamp=" + _stamp + ";" +
                    "oauth2=" + _token + ";"
                    + "signature=" + _signature + ";"
                    + "scene=" + _scene;
                request.AddHeader("authorization", _auth);
                LogHelper.Debug($"_source:{_source},_auth:{_auth}");
                IRestResponse response = client.Execute(request);
                return response.Content;

            }
            catch (Exception ex)
            {
                LogHelper.Error("ZuberHouseCrawler", ex);
                return string.Empty;
            }

        }

        private static string md5(string observedText)
        {
            string result;
            using (MD5 hash = MD5.Create())
            {
                result = String.Join
                (
                    "",
                    from ba in hash.ComputeHash
                    (
                        Encoding.UTF8.GetBytes(observedText)
                    )
                    select ba.ToString("x2")
                );
            }
            return result;
        }

        private static int GetTimestamp()
        {
            return (int)(DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }
    }
}
