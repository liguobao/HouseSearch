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

    public class CJia : BaseCrawler
    {
        private readonly RestClient _restClient;
        public CJia(HouseDapper houseDapper, ConfigDapper configDapper, ElasticService elastic, RedisTool redisTool)
        : base(houseDapper, configDapper, elastic, redisTool)
        {
            _restClient = new RestClient("https://m.cjia.com/restapi/svr/productsearch/product/searchListUnitEx/0");
            this.Source = SourceEnum.CJia;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            var jsonData = JToken.Parse(config.Json);
            var targetDestinationId = jsonData["targetDestinationId"].ToString();
            var pageIndex = page + 1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("referer", "https://m.cjia.com/longMixedUnitList/104/93?");
            request.AddHeader("authority", "m.cjia.com");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("header", "{\"applicationCode\":\"PORTAL-H5\",\"clientId\":\"\",\"sourceId\":\"M0000009\",\"exSourceId\":\"CJIA\",\"channel\":\"CJIA\",\"subChannel\":\"H5\",\"version\":\"2.0.0\",\"userToken\":\"\"}");
            request.AddHeader("accept", "*/*");
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7,zh-TW;q=0.6");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("origin", "https://m.cjia.com");
            request.AddParameter("application/json;charset=UTF-8",
            "{\"productChannel\":2,\"querys\":[],\"filters\":{},\"page\":{\"pageIndex\":"
            + pageIndex + ",\"pageSize\":10,\"sortBy\":0},\"targetDestinationId\":"
            + targetDestinationId + ",\"targetPoint\":{},\"queryStrategy\":2}", ParameterType.RequestBody);
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
            if (result == null || result.Count() == 0 || result["unitList"] == null)
            {
                return houses;
            }

            foreach (var room in result["unitList"])
            {
                var house = new DBHouse();
                house.Title = room["basicInfo"]?["unitName"]?.ToString();
                house.City = config.City;
                house.PubTime = DateTime.Now.Date;
                house.Text = GetHouseText(room);
                house.Location = room["basicInfo"]?["unitName"]?.ToString();
                house.Longitude = room["coordinate"]["longitude"]?.ToString();
                house.Latitude = room["coordinate"]["latitude"]?.ToString(); ;
                house.Price = !string.IsNullOrEmpty(room["priceInfo"]["lowestSalePriceValue"]?.ToString()) ? room["priceInfo"]["lowestSalePriceValue"].ToObject<int>() / 100 : 0;
                house.PicURLs = Tools.GetPicURLs(room["basicInfo"]["logoURL"]?.ToString()
                .Replace("?x-oss-process=image/resize,m_fill,h_469,w_750,limit_0", ""));
                house.Tags = string.Join("|", room["tagList"].Select(t => t["tagName"]?.ToString()));
                house.OnlineURL = "https://m.cjia.com/apartment/detail/" + room["basicInfo"]["unitId"]?.ToString();
                house.Id = Tools.GetGuid();
                house.Source = SourceEnum.CJia.GetSourceName();
                house.JsonData = room.ToString();
                house.RentType = (int)RentTypeEnum.Shared;
                houses.Add(house);
            }
            return houses;
        }

        private static string GetHouseText(JToken room)
        {
            var minBookingDays = !string.IsNullOrEmpty(room["minBookingDays"]?.ToString()) ? $"最少预定{room["minBookingDays"]?.ToString()}天" : "";
            return room["nearByInfo"]["zoneName"]?.ToString() + "，"
            + room["nearByInfo"]["nearFacilities"].FirstOrDefault()?.ToString() + "，"
            + string.Join("/", room["tagList"].Select(t => t["tagName"]?.ToString())) + "，"
            + minBookingDays;
        }

        private static int GetRentType(JToken room)
        {
            var rentType = 0;
            var roomType = room["rent_type_name"]?.ToString();
            if (roomType == "合租")
            {
                rentType = (int)RentTypeEnum.Shared;
            }
            else if (roomType == "整租")
            {
                rentType = (int)RentTypeEnum.AllInOne;
            }
            return rentType;
        }
    }
}