using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Common;
using Newtonsoft.Json.Linq;

namespace HouseMap.Crawler
{

    public class Xhj : BaseCrawler
    {
        public Xhj(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic, RedisTool redisTool)
              : base(houseDapper, configDapper, elastic, redisTool)
        {
            this.Source = SourceEnum.Xhj;

        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            return GetDataFromAPI(config, page);
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var jsonData = JToken.Parse(data);
            if (jsonData?["content"] == null)
            {
                return houses;
            }
            foreach (var houseItem in jsonData?["content"])
            {
                var house = new DBHouse();
                house.JsonData = houseItem.ToString();
                house.City = config.City;
                house.Title = houseItem["title"]?.ToString() + " " + houseItem["stressName"].ToString();
                house.Location = houseItem["address"]?.ToString();
                house.Text = houseItem["description"]?.ToString();
                if (!string.IsNullOrEmpty(houseItem["location"]?.ToString()))
                {
                    var dstPos = houseItem["location"].ToObject<List<double>>();
                    house.Longitude = dstPos[0].ToString();
                    house.Latitude = dstPos[1].ToString();
                }
                house.Id = Tools.GetGuid();
                house.Price = houseItem["price"].ToObject<int>();
                house.OnlineURL = $"http://m.xhj.com/zufangdetails/{houseItem["hsmid"]}/000.html";
                house.PubTime = UnixTimeStampToDateTime(houseItem["refreshTime"].ToObject<long>());
                house.PicURLs = JsonConvert.SerializeObject(houseItem["imgs"].ToList().Select(j => j["imgPath"]?.ToString()).ToList());
                house.Labels = string.Join("|", houseItem["features"].ToObject<List<string>>());
                house.RentType = GetRentType(houseItem["rentType"].ToString());
                house.Source = SourceEnum.Xhj.GetSourceName();
                houses.Add(house);
            }
            return houses;
        }

        private static int GetRentType(string title)
        {
            var rentType = 0;
            if (title == null)
            {
                return rentType;
            }
            if (title.Contains("整租"))
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            else if (title.Contains("合租"))
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (title.Contains("1室"))
            {
                rentType = (int)RentTypeEnum.OneRoom;
            }
            return rentType;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
        }

        private string GetDataFromAPI(DBConfig config, int page)
        {
            var client = new RestClient("http://m.xhj.com/wap/search/findByCondition");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Origin", "http://m.xhj.com");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            request.AddHeader("Referer", "http://m.xhj.com/zufanglist/");
            request.AddParameter("application/json; charset=UTF-8", "{\"houseType\":12,\"cityId\":\"1\",\"cityName\":\"长沙\",\"sort\":\"desc\",\"pageFrom\":" + page + ",\"pageSize\":30,\"currPage\":0,\"isRepeat\":false,\"statusArray\":[20,10],\"isThumb\":true,\"locationR\":0}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return string.Empty;
        }
    }
}