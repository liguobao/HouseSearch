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
using System.Net;

namespace HouseMap.Crawler
{

    public class Xianyu : BaseCrawler
    {

        private readonly string APP_KEY = "12574478";
        public Xianyu(HouseDapper houseDapper, ConfigDapper configDapper,
         ElasticService elastic,  RedisTool redisTool) : base(houseDapper, configDapper, elastic, redisTool)
        {
            this.Source = SourceEnum.Xianyu;

        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            return GetDataFromAPI(config, page);
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houses = new List<DBHouse>();
            var jsonData = JToken.Parse(data);
            var token = jsonData["c"]?.ToString().Split(";")?[0];
            if (!string.IsNullOrEmpty(token))
            {

            }
            if (jsonData?["data"] == null || jsonData?["data"]?["cardList"] == null)
            {
                return houses;
            }
            foreach (var card in jsonData?["data"]?["cardList"])
            {
                var house = new DBHouse();
                house.JsonData = card.ToString();
                house.City = card["cardData"]["city"].ToString();
                house.Title = card["cardData"]["title"].ToString();
                house.Text = card["cardData"]["description"].ToString();
                if (!string.IsNullOrEmpty(card["cardData"]?["attributesMap"]?["dstPos"]?.ToString()))
                {
                    house.Location = card["cardData"]?["attributesMap"]?["location"]?.ToString();
                    var dstPos = card["cardData"]?["attributesMap"]?["dstPos"]?.ToString();
                    house.Latitude = dstPos.Split(",")[0];
                    house.Longitude = dstPos.Split(",")[1];
                }
                else
                {
                    house.Location = card["cardData"]?["title"]?.ToString();
                }
                house.Id = Tools.GetGuid();
                var price = 0;
                int.TryParse(card["cardData"]["price"]?.ToString(), out price);
                house.Price = price;
                house.OnlineURL = card["cardData"]?["shortUrl"].ToString();
                house.PubTime = card["cardData"]["firstModified"].ToObject<DateTime>();
                house.PicURLs = card["cardData"]?["imageUrls"].ToString(); ;
                house.Labels = $"{ card["cardData"]?["area"].ToString()}";
                house.Source = SourceEnum.Xianyu.GetSourceName();
                houses.Add(house);
            }
            return houses;
        }


        private string GetDataFromAPI(DBConfig config, int page, string token, string cookie)
        {
            var fishpoolId = "624597";
            var data = "{\"topicSeq\":\"2\",\"topicCreateType\":\"0\",\"fishpoolTopicName\":\"出租\",\"fishpoolTopicId\":\"2479350\",\"fishpoolId\":\"" + fishpoolId + "\",\"topicRule\":\"{\\\"fishpoolId\\\":" + fishpoolId + ",\\\"keyWords\\\":[\\\"出租\\\"]}\",\"pageNumber\":" + page + "}";
            var time = Tools.GetMillisecondTimestamp();
            var sign = Tools.GetMD5($"{token}&{time}&{APP_KEY}&" + data);
            var client = new RestClient($"http://h5api.m.taobao.com/h5/mtop.taobao.idle.fishpool.item.list/6.0/?jsv=2.4.2&appKey={APP_KEY}&t={time}&sign={sign}&api=mtop.taobao.idle.fishpool.item.list&v=6.0&dataType=json&AniCreep=true&AntiFlool=true"
             + $"&jsonpIncPrefix=weexcb&ttid=2018%40weex_h5_0.12.11&type=originaljson&c={WebUtility.UrlEncode(cookie)}&data=" + WebUtility.UrlEncode(data));
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("origin", "http://g.tbcdn.cn");
            //request.AddHeader("referer", "http://g.tbcdn.cn/idleFish-F2e/app-basic/fishpool.html?id=624597&ut_sk=1.W3D9dDwZ1BoDAHWNnmC0v4dS_21407387_1540215181688.Copy.fishpool.624597.557644361&forceFlush=1");
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            return response.Content;


        }


        private string GetDataFromAPI(DBConfig config, int page)
        {
            //var data = "{\"topicSeq\":\"2\",\"topicCreateType\":\"0\",\"fishpoolTopicName\":\"出租\",\"fishpoolTopicId\":\"2479350\",\"fishpoolId\":\"624597\",\"topicRule\":\"{\\\"fishpoolId\\\":624597,\\\"keyWords\\\":[\\\"出租\\\"]}\",\"pageNumber\":2}";
            var dataCode = WebUtility.UrlEncode(config.Json);
            var client = new RestClient("http://h5api.m.taobao.com/h5/mtop.taobao.idle.fishpool.item.list/6.0/?jsv=2.4.2&appKey=12574478&t=1540215670275&sign=c39580d53207fcc1fe2238b7d0497dff&api=mtop.taobao.idle.fishpool.item.list&v=6.0&dataType=json&AniCreep=true&AntiFlool=true&jsonpIncPrefix=weexcb&ttid=2018%40weex_h5_0.12.11&type=originaljson&c=6ef4a222b0b1ecac5f0a5dbcd9a72bfe_1540223174832%3B87c44162f3e1c16a778cd67c2752b29b&data=" +
               dataCode);
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("origin", "http://g.tbcdn.cn");
            request.AddHeader("referer", "http://g.tbcdn.cn/idleFish-F2e/app-basic/fishpool.html?id=624597&ut_sk=1.W3D9dDwZ1BoDAHWNnmC0v4dS_21407387_1540215181688.Copy.fishpool.624597.557644361&forceFlush=1");
            request.AddHeader("accept", "application/json");
            IRestResponse response = client.Execute(request);
            if (string.IsNullOrEmpty(response.Content))
            {
                LogHelper.Info("response:" + response.ErrorMessage);
                return "{}";
            }
            if (JToken.Parse(response.Content)["c"]?.ToString() != null)
            {
                return GetDataFromAPI(config, page, GetToken(response), JToken.Parse(response.Content)["c"]?.ToString());
            }
            return response.Content;

        }

        private static string GetToken(IRestResponse response)
        {
            return JToken.Parse(response.Content)["c"]?.ToString().Split(";")[0].Split("_")[0];
        }
    }
}