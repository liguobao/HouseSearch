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
    public class MoGuHouseCrawler
    {
        private HouseDapper houseDapper;

        private ConfigDapper configDapper;
        public MoGuHouseCrawler(HouseDapper houseDapper, ConfigDapper configDapper)
        {
            this.houseDapper = houseDapper;
            this.configDapper = configDapper;
        }

        public void Run()
        {
            foreach (var conf in configDapper.GetList(ConstConfigName.MoguHouse))
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
                    var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(conf.ConfigurationValue);
                    var cityName = confInfo.cityname.Value;
                    var cityId = (int)confInfo.cityid.Value;
                    // 2:合租 3:整租 5:业主房源
                    var rentTypes = new List<int>() { 2, 3, 5 };
                    foreach (var rentType in rentTypes)
                    {
                        for (var pageIndex = 1; pageIndex <= confInfo.pagecount.Value; pageIndex++)
                        {
                            var list = GetHouseData(cityName, cityId, pageIndex, rentType);
                            houses.AddRange(list);
                        }
                    }
                    houseDapper.BulkInsertHouses(houses);
                }, "MoGuHouseCrawler Run ", conf);
            }
        }

        public static List<BaseHouseInfo> GetHouseData(string cityName, int cityID, int currentPage, int rentTypes = 2)
        {
            List<BaseHouseInfo> lstHouse = new List<BaseHouseInfo>();
            var result = GetAPIResult(cityID, currentPage, rentTypes);
            if (string.IsNullOrEmpty(result))
            {
                return lstHouse;
            }
            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);

            if (resultJObject["message"].ToString() != "success"
                        || resultJObject["content"] == null
                        || resultJObject["content"]["roomInfos"] == null)
            {
                return lstHouse;
            }
            foreach (var room in resultJObject["content"]["roomInfos"])
            {
                try
                {
                    var housePrice = room["maxShowPrice"].ToObject<decimal>();

                    var lastPublishTime = GetPublishTime(room);
                    string location = room["address"] != null ? room["address"].ToString() : room["title"].ToString();
                    var house = new BaseHouseInfo()
                    {
                        HouseLocation = location,
                        HouseTitle = $"{room["title"].ToString()}【{room["subtitleNew"].ToString()}】",
                        HouseOnlineURL = $"https://h5.mgzf.com/houseDetail/{room["roomId"].ToString()}",
                        HouseText = room.ToString(),
                        HousePrice = housePrice,
                        DisPlayPrice = housePrice > 0 ? $"{housePrice}元" : "",
                        Source = ConstConfigName.MoguHouse,
                        LocationCityName = cityName,
                        PicURLs = GetPhotos(room),
                        PubTime = lastPublishTime,
                        Status = 1,
                        IsAnalyzed = true
                    };
                    lstHouse.Add(house);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("Convert to House error", ex, room);
                }
            }

            return lstHouse;
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
                request.AddHeader("referer", "https://h5.mgzf.com/list?rentTypes%5B1%5D=2&currentPage=1");
                request.AddHeader("accept", "application/json, text/plain, */*");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
                request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
                request.AddHeader("accept-encoding", "gzip, deflate, br");
                request.AddHeader("origin", "https://h5.mgzf.com");
                string bodyParameters = $"payTypes=&houseType=&flatsTags=&rentTypes={rentTypes}&currentPage={currentPage}&cityId={cityID}&searchTimeStamp={Tools.GetSearchTimeStamp()}";
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