using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HouseCrawler.Core.Models;
using HouseCrawler.Core.Common;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HouseCrawler.Core
{
    public class BeikeHouseCrawler
    {
        private HouseDapper houseDapper;

        private ConfigDapper configDapper;

        public BeikeHouseCrawler(HouseDapper houseDapper, ConfigDapper configDapper)
        {
            this.houseDapper = houseDapper;
            this.configDapper = configDapper;
        }

        public void Run()
        {
            try
            {
                int captrueHouseCount = 0;
                DateTime startTime = DateTime.Now;
                foreach (var config in configDapper.GetList(ConstConfigName.Beike))
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


        public void InitCityData()
        {
            var client = new RestClient("https://app.api.ke.com/config/city/selectlist");
            var request = new RestRequest(Method.GET);
            request.AddHeader("connection", "Keep-Alive");
            request.AddHeader("host", "app.api.ke.com");
            request.AddHeader("lianjia-im-version", "2.13.0-SNAPSHOT");
            request.AddHeader("authorization", "MjAxODAxMTFfYW5kcm9pZDo0OGI4Mjg4M2NmNDdhYzAyZWQzMmFlZjI0ZmVhNjg0ODUxYzdkYWI5");
            request.AddHeader("lianjia-version", "1.2.4");
            request.AddHeader("lianjia-device-id", "865371037947909");
            request.AddHeader("lianjia-channel", "Android_ke_chuizi");
            request.AddHeader("user-agent", "Beike1.2.4;SMARTISAN OD103; Android 7.1.1");
            request.AddHeader("extension", "lj_device_id_android=865371037947909&mac_id=B4:0B:44:D0:2E:D1&lj_imei=865371037947909&lj_android_id=a9adb10848897a64");
            request.AddHeader("referer", "homepage%3Fcity_id%3D310000%26latitude%3D31.328682%26longitude%3D121.397956%26distance%3D5000%26limit_offset%3D0%26limit_count%3D10");
            request.AddHeader("page-schema", "SelectCityActivity");
            IRestResponse response = client.Execute(request);
            var resultJObject = JsonConvert.DeserializeObject<JObject>(response.Content);
            var configs = new List<CrawlerConfiguration>();
            foreach (var tab in resultJObject["data"]["tab_list"])
            {
                if (tab["title"] != null && tab["title"].ToString() == "海外城市")
                {
                    continue;
                }
                foreach (var city in tab["list"].SelectMany(item =>item["cities"]))
                {
                    var cityId = city["id"].ToString();
                    var name = city["name"].ToString();
                    var abbr = city["abbr"].ToString();

                    var configValue = "{'citySortName':'" + abbr + "','cityName':'" + name + "','cityID':'" + cityId + "','pagecount':10}";
                    var config = new CrawlerConfiguration()
                    {
                        ConfigurationValue = configValue,
                        ConfigurationName = "beike",
                    };
                    configs.Add(config);
                }


            }
            configDapper.BulkInsertConfig(configs);

        }

    }
}
