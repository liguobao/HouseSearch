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
using HouseMap.Common;

namespace HouseMap.Crawler
{

    public class BeikeCrawler : BaseCrawler,ICrawler
    {

        public BeikeCrawler(HouseDapper houseDapper,  ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = ConstConfigName.Beike;
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            var cityId = config["cityID"].ToString();
            var apiURL = $"https://app.api.ke.com/Rentplat/v1/house/list?city_id={cityId}&offset={page * 30}&limit=30";
            return GetAPIResult(apiURL, cityId);

        }

        public override List<HouseInfo> ParseHouses(JToken config, string data)
        {
            var lstHouseInfo = new List<HouseInfo>();
            if (string.IsNullOrEmpty(data))
            {
                return lstHouseInfo;
            }
            var cityName = config["cityName"]?.ToString();
            var citySortName = config["citySortName"]?.ToString();
            var resultJObject = JsonConvert.DeserializeObject<JObject>(data);
            foreach (var rentHouse in resultJObject["data"]["list"])
            {
                var housePrice = GetPrice(rentHouse);

                var house = new HouseInfo()
                {
                    HouseLocation = rentHouse["house_title"].ToString(),
                    HouseTitle = rentHouse["house_title"].ToString() + rentHouse["desc"].ToString(),
                    HouseOnlineURL = GetHouseURL(rentHouse, citySortName),
                    HouseText = rentHouse.ToString(),
                    HousePrice = housePrice,
                    IsAnalyzed = true,
                    DisPlayPrice = housePrice > 0 ? $"{housePrice}元" : "",
                    Source = ConstConfigName.Beike,
                    LocationCityName = cityName,
                    Status = 1,
                    PicURLs = JsonConvert.SerializeObject(new List<string>()),
                    PubTime = DateTime.Now
                };
                lstHouseInfo.Add(house);
            }
            return lstHouseInfo;
        }




        private static string GetAPIResult(string apiURL, string cityID)
        {
            try
            {
                var client = new RestClient(apiURL);
                var request = new RestRequest(Method.GET);
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
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return response.Content;
                }

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

        private static decimal GetPrice(JToken rentHouse)
        {
            decimal housePrice = 0;
            if (rentHouse["rent_price_listing"].ToString().Contains("-"))
            {
                housePrice = decimal.Parse(rentHouse["rent_price_listing"].ToString().Split("-")[0]);
            }
            else
            {
                housePrice = decimal.Parse(rentHouse["rent_price_listing"].ToString());
            }
            return housePrice;
        }

    }
}