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

namespace HouseMap.Crawler
{

    public class ZiRoom : BaseCrawler
    {
        private readonly RestClient _restClient;
        public ZiRoom(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic,  RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            _restClient = new RestClient("http://m.ziroom.com/");
            this.Source = SourceEnum.ZiRoom;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var city_code = jsonData["city_code"].ToString();
            var pageIndex = page + 1;
            var resource = $"v7/room/list.json?city_code={city_code}&page={pageIndex}&price=&face=&rface=&hface=&feature=&around=&leasetype=&tag=&version=&area=&subway_code=&subway_station_code=&district_code=&bizcircle_code=&clng=&clat=&suggestion_type=&suggestion_value=&keywords=&sort=";
            var request = new RestRequest(resource, Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("referer", "http://sh.m.ziroom.com/SH/search");
            request.AddHeader("accept", "application/json;version=6");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7,zh-TW;q=0.6");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("pragma", "no-cache");
            IRestResponse response = _restClient.Execute(request);
            return response.IsSuccessful ? response.Content : "";
        }



        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            if (data.Contains("<html"))
            {
                return houses;
            }
            var result = JToken.Parse(data);
            if (result == null || result.Count() == 0 || result["status"]?.ToString() != "success")
            {
                return houses;
            }
            var jsonData = JToken.Parse(config.Json);
            var cityCode = jsonData["city_code"].ToString();

            foreach (var room in result["data"]?["rooms"])
            {
                var id = room["id"]?.ToString();
                var houseDataJson = GetHouseData(cityCode, id);
                if (string.IsNullOrEmpty(houseDataJson) || !houseDataJson.Contains("success"))
                {
                    continue;
                }
                try
                {
                    DBHouse house = ConvertToHouse(config, room, houseDataJson);
                    houses.Add(house);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("ConvertToHouse fail", ex, room);
                }
            }
            return houses;
        }

        private static DBHouse ConvertToHouse(DBConfig config, JToken room, string houseDataJson)
        {
            var house = new DBHouse();
            var houseData = JToken.Parse(houseDataJson)["data"];
            house.Title = houseData["name"].ToString();
            house.City = config.City;
            house.PubTime = DateTime.Now;
            house.Text = houseData["introduction"].ToString();
            if (!string.IsNullOrEmpty(houseData["resblock"]?["around"].ToString()))
            {
                house.Text = house.Text + "<br/>" + houseData["resblock"]?["around"].ToString();
            }
            if (!string.IsNullOrEmpty(houseData["resblock"]?["traffic"].ToString()))
            {
                house.Text = house.Text + "<br/>" + houseData["resblock"]?["traffic"].ToString();
            }

            house.Text = house.Text + $"<br/><br/>上线时间：{houseData["actually_complete_date"]?.ToString()}";
            if (!string.IsNullOrEmpty(houseData["air_report_detail"].ToString()))
            {
                house.Text = house.Text + $" <br/>空气检测信息如下:";
                foreach (var item in houseData["air_report_detail"]["data"])
                {
                    house.Text = house.Text + $"<br/>{item["title"].ToString()}:{item["value"].ToString()}";
                    if (!string.IsNullOrEmpty(item["link"]?.ToString()))
                    {
                        house.Text = house.Text + $"<br/>在线链接:<a href='{item["link"].ToString()}' target='_blank'>{item["link"].ToString()}</a>";
                    }
                }
            }
            house.Location = houseData["location"]?.ToString();
            house.Latitude = houseData["resblock"]?["lat"].ToString();
            house.Longitude = houseData["resblock"]?["lng"].ToString();
            house.Price = 0;
            List<string> allPhotos = GetPhotos(houseData);
            house.PicURLs = JsonConvert.SerializeObject(allPhotos); ;
            house.Tags = string.Join("|", room["tags"].Select(t => t["title"].ToString()));
            house.OnlineURL = houseData["weibo_share"]?["url"]?.ToString();
            house.Id = Tools.GetGuid();
            house.Source = SourceEnum.ZiRoom.GetSourceName();
            house.JsonData = houseData.ToString();
            house.RentType = GetRentType(houseData);
            return house;
        }

        private static List<string> GetPhotos(JToken houseData)
        {
            var allPhotos = new List<string>();
            if (!string.IsNullOrEmpty(houseData["3d_showing"]?.ToString()))
            {
                allPhotos.Add(houseData["3d_showing"]?.ToString());
            }
            var photos = houseData["space"].SelectMany(s => s["photos_big"].Select(p => "https:" + p.ToString())).ToList();
            if (photos != null)
            {
                allPhotos.AddRange(photos);
            }
            allPhotos.AddRange(houseData["hx_photos_big"].Select(i => "https:" + i.ToString()).ToList());


            return allPhotos;
        }

        public string GetHouseData(string cityCode, string id)
        {
            var client = new RestClient($"http://m.ziroom.com/wap/detail/room.json?city_code={cityCode}&id={id}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7,zh-TW;q=0.6");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("connection", "keep-alive");
            IRestResponse response = client.Execute(request);

            return response.IsSuccessful ? response.Content : "";
        }

        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            var roomType = room["version_name"]?.ToString();
            if (string.IsNullOrEmpty(roomType))
            {
                return rentType;
            }
            if (roomType.Contains("友家"))
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (roomType.Contains("整租"))
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            return rentType;
        }
    }
}