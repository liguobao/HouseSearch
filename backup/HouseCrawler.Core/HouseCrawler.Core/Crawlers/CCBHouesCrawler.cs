using HouseCrawler.Core.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;


namespace HouseCrawler.Core
{
    public class CCBHouesCrawler
    {

        private HouseDapper houseDapper;

        private APPConfiguration config;

        private ConfigDapper configDapper;

        public CCBHouesCrawler(HouseDapper houseDapper, IOptions<APPConfiguration> configuration,
        ConfigDapper configDapper)
        {
            this.houseDapper = houseDapper;
            this.config = configuration.Value;
            this.configDapper = configDapper;
        }

        public void Run()
        {
            int captrueHouseCount = 0;
            DateTime startTime = DateTime.Now;
            foreach (var crawlerConfiguration in configDapper.GetList(ConstConfigName.CCBHouse).ToList())
            {
                LogHelper.RunActionNotThrowEx(() =>
                {
                    captrueHouseCount = captrueHouseCount + CaptureHouse(crawlerConfiguration);
                }, "CapturPinPaiHouseInfo", crawlerConfiguration);
            }
            LogHelper.Info($"CCBHouesCrawler finish.本次共爬取到{captrueHouseCount}条数据，耗时{ (DateTime.Now - startTime).TotalSeconds}秒。");
        }

        private int CaptureHouse(CrawlerConfiguration crawlerConfiguration)
        {
            var confInfo = JsonConvert.DeserializeObject<dynamic>(crawlerConfiguration.ConfigurationValue);
            if (confInfo.shortcutname == null || string.IsNullOrEmpty(confInfo.shortcutname.Value))
            {
                return 0;
            }
            int captrueHouseCount = 0;
            string cityShortCutName = confInfo.shortcutname.Value;
            List<BaseHouseInfo> houses = new List<BaseHouseInfo>();
            for (var pageNum = 1; pageNum < confInfo.pagecount.Value; pageNum++)
            {
                var result = GetResultByAPI(cityShortCutName, pageNum);
                houses.AddRange(GetHouseData(cityShortCutName, result));
            }
            captrueHouseCount = captrueHouseCount + houses.Count;
            houseDapper.BulkInsertHouses(houses);
            return captrueHouseCount;
        }

        private static List<BaseHouseInfo> GetHouseData(string cityShortCutName, string result)
        {
            var houseList = new List<BaseHouseInfo>();
            if (string.IsNullOrEmpty(result))
            {
                return houseList;
            }

            var resultJObject = JsonConvert.DeserializeObject<JObject>(result);
            foreach (var item in resultJObject["items"])
            {
                BaseHouseInfo houseInfo = new BaseHouseInfo();
                string houseURL = GetHouseOnlineURL(cityShortCutName, item);
                houseInfo.HouseOnlineURL = houseURL;
                houseInfo.HouseLocation = item["headline"].ToObject<string>();
                houseInfo.HouseTitle = item["headline"].ToObject<string>();
                houseInfo.LocationCityName = item["cityName"].ToObject<string>();
                houseInfo.HouseText = item.ToString();
                houseInfo.HousePrice = item["totalPrice"].ToObject<Int32>();
                houseInfo.DisPlayPrice = item["totalPrice"].ToString();
                houseInfo.PubTime = item["publishTime"].ToObject<DateTime>();
                houseInfo.Source = ConstConfigName.CCBHouse;
                houseList.Add(houseInfo);
            }

            return houseList;
        }

        private static string GetHouseOnlineURL(string cityShortCutName, JToken item)
        {
            var houseURL = "";
            if (!string.IsNullOrEmpty(item["web_url"].ToString()))
            {
                houseURL = item["web_url"].ToString();
            }
            else if (!string.IsNullOrEmpty(item["app_url"].ToString()))
            {
                houseURL = item["app_url"].ToString();
            }
            else
            {
                houseURL = $"http://{cityShortCutName}.jiayuan.home.ccb.com/lease/{ item["dealCode"].ToString()}.html";
            }

            return houseURL;
        }

        private string GetResultByAPI(string cityShortCutName, int page)
        {
            string formBody = $"_reqParams=apiKey%3D{config.CCBHomeAPIKey}%26city%3D{cityShortCutName}%26saleOrLease%3Dlease%26pageSize%3D50%26page%3D{page}%26propType%3D11%26tmflags%3D3&_interfaceUrl=%2Fhlsp%2Fcityhouse%2Fdeal%2Fsearch&_reqMethod=GET";
            var client = new RestClient("http://bankservice.home.ccb.com/LHECISM/LanHaiHttpResfulReqServlet");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("cookie2", "$Version=1");
            request.AddHeader("cookie", "UDC_Ser2018_ON=1; FAVOR=||||||||||||||||||||||||||||||||||||||||||||||||||; CCBIBS1=Qief3XbaeklLa3FZfwVHnEaGnFVRS3EOeEVxmbDcfhlWbc2zwKnabcKNraFiWdHUel1baPIKeD1K2yHHeSVJmyFMf0lvqqR9Vw3Xli; TC=249277366_1613198604_1362849648; UDC_ON=1; _BOA_mf_txcode_=HT0205");
            request.AddHeader("host", "bankservice.home.ccb.com");
            request.AddParameter("application/x-www-form-urlencoded", formBody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";
        }
    }
}
