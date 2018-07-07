using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;
using HouseCrawler.Core.DataContent;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HouseCrawler.Core
{
    public class BeikeHouseCrawler
    {
        private HouseDapper houseDapper;
        public BeikeHouseCrawler(HouseDapper houseDapper)
        {
            this.houseDapper = houseDapper;
        }

        public void Run()
        {
            try
            {
                int captrueHouseCount = 0;
                DateTime startTime = DateTime.Now;
                foreach (var config in houseDapper.GetConfigurationList(ConstConfigurationName.Beike))
                {
                    LogHelper.RunActionNotThrowEx(() =>
                    {
                        List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
                        var confInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(config.ConfigurationValue);
                        for (var pageIndex = 0; pageIndex < confInfo.pagecount.Value; pageIndex++)
                        {
                            var lstHouseInfo = GetHouseData(confInfo.citySortName.Value, confInfo.cityID.Value,
                                confInfo.cityName.Value, pageIndex);
                            houses.AddRange(lstHouseInfo);
                        }
                        captrueHouseCount = captrueHouseCount + houses.Count;
                        houseDapper.BulkInsertHouses(houses);
                    }, "BeikeHouseCrawler CaptureHouseInfo ", config);
                }
                LogHelper.Info($"BeikeHouseCrawler finish.本次共爬取到{captrueHouseCount}条数据，耗时{ (DateTime.Now - startTime).TotalSeconds}秒。");
            }
            catch (Exception ex)
            {
                LogHelper.Error("BeikeHouseCrawler CrawlerHouseInfo Exception", ex);
            }
        }

        public static List<BaseHouseInfo> GetHouseData(string citySortName, string cityID, string cityName, int pageIndex)
        {
            List<BaseHouseInfo> lstHouseInfo = new List<BaseHouseInfo>();
            var apiURL = $"https://app.api.ke.com/Rentplat/v1/house/list?city_id={cityID}&offset={pageIndex * 30}&limit=30";
            var result = GetAPIResult(apiURL, cityID);
            if (string.IsNullOrEmpty(result))
            {
                return lstHouseInfo;
            }
            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);
            foreach (var rentHouse in resultJObject["data"]["list"])
            {
                var housePrice = GetPrice(rentHouse);

                var house = new BaseHouseInfo()
                {
                    HouseLocation = rentHouse["house_title"].ToString(),
                    HouseTitle = rentHouse["house_title"].ToString() + rentHouse["desc"].ToString(),
                    HouseOnlineURL = GetHouseURL(rentHouse, citySortName),
                    HouseText = rentHouse.ToString(),
                    HousePrice = housePrice,
                    IsAnalyzed = true,
                    DisPlayPrice = housePrice > 0 ? $"{housePrice}元" : "",
                    Source = ConstConfigurationName.Beike,
                    LocationCityName = cityName,
                    Status = 1,
                    PicURLs = JsonConvert.SerializeObject(new List<string>()),
                    PubTime = DateTime.Now
                };
                lstHouseInfo.Add(house);
            }

            return lstHouseInfo;
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

    }
}
