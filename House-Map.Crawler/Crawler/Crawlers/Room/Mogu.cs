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

    public class Mogu : BaseCrawler
    {


        public Mogu(HouseDapper houseDapper, ConfigDapper configDapper,ElasticService elastic,  RedisTool redisTool) 
        : base(houseDapper, configDapper,elastic, redisTool)
        {
            this.Source = SourceEnum.Mogu;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var cityId = jsonData["cityid"].ToObject<int>();
            var rentType = jsonData["rentTypes"] != null ? jsonData["rentTypes"].ToObject<int>() : 2;
            return GetAPIResult(cityId, page, rentType);
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var resultJObject = JObject.Parse(data);

            if (resultJObject["content"] == null
                        || resultJObject["content"]["roomInfos"] == null)
            {
                return houses;
            }
            foreach (var room in resultJObject["content"]["roomInfos"])
            {
                try
                {
                    DBHouse house = ConvertHouse(config, room);
                    houses.Add(house);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Convert to House error", ex, room);
                }
            }
            return houses;
        }

        private static DBHouse ConvertHouse(DBConfig config, JToken room)
        {
            var housePrice = room["maxShowPrice"].ToObject<int>();
            var lastPublishTime = GetPublishTime(room);
            string location = room["address"] != null ? room["address"].ToString() : room["title"].ToString();
            var house = GetHouse(config.City, room, housePrice, lastPublishTime, location);
            return house;
        }

        private static DBHouse GetHouse(string city, JToken room, int housePrice, DateTime lastPublishTime, string location)
        {
            return new DBHouse()
            {
                Id = Tools.GetGuid(),
                Location = location,
                Title = $"{room["title"].ToString()}【{room["subtitleNew"].ToString()}】",
                OnlineURL = $"https://h5.mgzf.com/{GetRoomPath(room)}/{room["roomId"].ToString()}",
                JsonData = room.ToString(),
                Price = housePrice,
                Source = SourceEnum.Mogu.GetSourceName(),
                City = city,
                PicURLs = GetPhotos(room),
                PubTime = lastPublishTime,
                //Labels = string.Join("|", room["specials"] != null ? room["specials"] : new List<JToken>()),
                Tags = $"{room["districtName"]?.ToString()}|{room["comName"]?.ToString()}|{room["subtitle"]?.ToString()}",
                RentType = ConvertToRentType(room),
                Latitude = room["lat"].ToString(),
                Longitude = room["lng"].ToString()
            };
        }

        private static string GetRoomPath(JToken room)
        {
            return room["rentType"]["key"].ToObject<int>() == 2 ? "roomDetail" : "houseDetail";
        }

        private static int ConvertToRentType(JToken room)
        {
            if (room["rentType"]["key"].ToObject<int>() == 2)
            {
                return (int)RentTypeEnum.AllInOne;
            }
            else if (room["rentType"]["key"].ToObject<int>() == 3)
            {
                return (int)RentTypeEnum.Shared;
            }

            return (int)RentTypeEnum.Undefined;
        }

        private static string GetPhotos(JToken room)
        {
            var photos = new List<String>();
            if (room["imageNew"] != null)
            {
                photos.Add(room["imageNew"].ToString().Replace("!mobile.list2", ""));
            }
            return JsonConvert.SerializeObject(photos);
        }
        public static DateTime GetPublishTime(JToken room)
        {
            try
            {
                if (room["lastPublishTime"] != null && !string.IsNullOrEmpty(room["lastPublishTime"].ToString()))
                {
                    return room["lastPublishTime"].ToObject<DateTime>();
                }
                else
                {
                    return DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetPublishTime", ex, room);
                return DateTime.Now;
            }

        }

        public static string GetAPIResult(int cityID, int currentPage, int rentTypes = 2)
        {
            try
            {
                var client = new RestClient("https://renterfind.mgzf.com/mogoroom-find/v2/find/getRoomListByCriteria");
                var request = new RestRequest(Method.POST);
                request.AddHeader("uuid", "92EFC512-9566-4D73-80D6-9A497B6F");
                request.AddHeader("channel", "9");
                request.AddHeader("connection", "keep-alive");
                request.AddHeader("userid", "0");
                var prePage = currentPage > 0 ? currentPage - 1 : 0;
                request.AddHeader("referer", $"https://h5.mgzf.com/list?rentTypes%5B1%5D=2&currentPage={prePage}");
                request.AddHeader("accept", "application/json, text/plain, */*");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("origin", "https://h5.mgzf.com");
                string bodyParameters = $"payTypes=&houseType=&flatsTags=&rentTypes={rentTypes}&currentPage={currentPage}&cityId={cityID}&pageSize=50&searchTimeStamp={Tools.GetSearchTimeStamp()}";
                request.AddParameter("application/x-www-form-urlencoded", bodyParameters, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAPIResult", ex);
                return string.Empty;
            }

        }
    }
}