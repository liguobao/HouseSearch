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
using System.Security.Cryptography;
using System.Text;
using HouseMap.Common;
using System.IO;
using Microsoft.Extensions.Options;

namespace HouseMap.Crawler
{

    public class Beike : BaseCrawler
    {
        private readonly RestClient _restClient;
        public Beike(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elasticService,RedisTool redisTool)
        : base(houseDapper, configDapper, elasticService, redisTool)
        {
            _restClient = new RestClient("https://app.api.ke.com/Rentplat/v1");
            this.Source = SourceEnum.Beike;
        }


        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var cityId = JToken.Parse(config.Json)["cityID"].ToString();
            var resource = $"/house/list?city_id={cityId}&offset={page * 30}&limit=30";
            return GetHouseResult(resource, cityId);
        }
        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var configJToken = JToken.Parse(config.Json);
            var cityName = configJToken["cityName"]?.ToString();
            var citySortName = configJToken["citySortName"]?.ToString();
            var houseList = JToken.Parse(data)["data"]["list"];
            foreach (var rentHouse in houseList)
            {
                DBHouse house = new DBHouse();
                var onlineURL = GetHouseURL(rentHouse, citySortName);
                house.Id = Tools.GetGuid();
                house.OnlineURL = onlineURL;
                house.Title = rentHouse["house_title"].ToString();
                house.Price = GetPrice(rentHouse);
                house.Location = !string.IsNullOrEmpty(rentHouse["resblock_name"]?.ToString()) ? rentHouse["resblock_name"].ToString() : rentHouse["desc"].ToString();
                house.PubTime = DateTime.Now;
                house.RentType = ConvertRentType(rentHouse["layout"].ToString());
                house.Text = $"朝{rentHouse["frame_orientation"].ToString()},{rentHouse["layout"].ToString()}" + rentHouse["desc"].ToString();
                house.Labels = string.Join("|", rentHouse["house_tags"].Select(i => i["name"].ToString()));
                house.Source = SourceEnum.Beike.GetSourceName();
                house.JsonData = rentHouse.ToString();
                house.City = config.City;
                house.CreateTime = DateTime.Now;
                house.Tags = $"{rentHouse["layout"]?.ToString()}|{rentHouse["bizcircle_name"]?.ToString()}";
                var picURL = rentHouse["list_picture"]?.ToString().Replace("280x210", "1280x960"); ;
                house.PicURLs = JsonConvert.SerializeObject(new List<string>() { picURL });
                houses.Add(house);
            }
            return houses;

        }

        private int ConvertRentType(string houseTypeDisplay)
        {
            if (houseTypeDisplay == "1室")
            {
                return (int)RentTypeEnum.OneRoom;
            }
            return (int)RentTypeEnum.AllInOne;
        }


        private string GetHouseResult(string resource, string cityID)
        {
            try
            {
                var request = new RestRequest(resource, Method.GET);
                request.AddHeader("connection", "Keep-Alive");
                request.AddHeader("host", "app.api.ke.com");
                request.AddHeader("lianjia-im-version", "2.8.0");
                request.AddHeader("lianjia-version", "1.2.4");
                request.AddHeader("lianjia-device-id", "865371037947909");
                request.AddHeader("lianjia-channel", "Android_ke_chuizi");
                request.AddHeader("user-agent", "Beike1.2.4;SMARTISAN OD103; Android 7.1.1");
                request.AddHeader("extension", "lj_device_id_android=865371037947909&mac_id=B4:0B:44:D0:2E:D1&lj_imei=865371037947909&lj_android_id=a9adb10848897a64");
                request.AddHeader("lianjia-city-id", cityID);
                request.AddHeader("page-schema", "matrix%2Fhomepage");
                IRestResponse response = _restClient.Execute(request);
                return response.IsSuccessful ? response.Content : "";
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetAPIResult", ex);
            }
            return "";

        }

        private static string GetHouseURL(JToken rentHouse, string citySortName)
        {
            var houseURL = "";
            var detailScheme = rentHouse["detail_scheme"].ToString();
            if (detailScheme.Contains("lianjiabeike"))
            {
                houseURL = $"https://{citySortName}.zu.ke.com/zufang/{rentHouse["house_code"].ToString()}.html";
            }
            else
            {
                houseURL = detailScheme;
            }
            return houseURL;
        }

        private static int GetPrice(JToken rentHouse)
        {
            int housePrice = 0;
            if (rentHouse["rent_price_listing"].ToString().Contains("-"))
            {
                housePrice = int.Parse(rentHouse["rent_price_listing"].ToString().Split("-")[0]);
            }
            else
            {
                housePrice = int.Parse(rentHouse["rent_price_listing"].ToString());
            }
            return housePrice;
        }









    }


}