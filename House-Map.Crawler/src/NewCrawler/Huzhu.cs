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
using HouseMap.Models;

namespace HouseMap.Crawler
{

    public class Huzhu : NewBaseCrawler
    {

        public Huzhu(NewHouseDapper houseDapper, ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = SourceEnum.HuZhuZuFang;
        }

        public override string GetJsonOrHTML(DBConfig config, int page)
        {
            return getResultFromAPI(page);
        }

        public override List<DBHouse> ParseHouses(DBConfig config, string data)
        {
            var houseList = new List<DBHouse>();
            var resultJObject = JObject.Parse(data);
            if (resultJObject == null || resultJObject["head"] == null || !resultJObject["head"]["success"].ToObject<Boolean>())
            {
                return houseList;
            }
            foreach (var item in resultJObject["houseList"])
            {
                DBHouse house = new DBHouse();
                house.Id = Tools.GetUUId();
                var houseDesc = item["houseDescript"].ToObject<string>().Replace("😄", "");
                var houseURL = $"http://www.huzhumaifang.com/Renting/house_detail/id/{item["houseId"]}.html";
                house.OnlineURL = houseURL;
                house.Title = houseDesc;
                house.Location = houseDesc;
                house.Text = houseDesc;
                house.JsonData = item.ToString();
                house.Price = item["houseRentPrice"].ToObject<Int32>();
                house.City = config.City;
                house.RentType = GetRentType(houseDesc);
                house.PubTime = item["houseCreateTime"].ToObject<DateTime>();
                house.PicURLs = item["bigPicUrls"].ToString();
                house.Source = SourceEnum.HuZhuZuFang.GetSourceName();
                houseList.Add(house);
            }
            return houseList;
        }

        private int GetRentType(string houseDesc)
        {
            if (houseDesc.Contains("一室一厅") || houseDesc.Contains("1室1厅") || houseDesc.Contains("一室户") || houseDesc.Contains("1室户"))
            {
                return (int)RentTypeEnum.OneRoom;
            }
            else if (houseDesc.Contains("整租"))
            {
                return (int)RentTypeEnum.AllInOne;
            }
            else if (houseDesc.Contains("合租"))
            {
                return (int)RentTypeEnum.Shared;
            }
            return (int)RentTypeEnum.AllInOne;
        }

        private static string getResultFromAPI(int pageNum)
        {
            var dicParameter = new JObject()
            {
                {"uid","" },
                {"pageNum",$"{pageNum}" },
                {"sortType","1" },
                {"sellRentType","2" },
                {"searchCondition","{}" }
            };
            var client = new RestClient("http://www.huzhumaifang.com:8080/hzmf-integration/getHouseList.action");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("user-agent", "Apache-HttpClient/UNAVAILABLE (java 1.4)");
            request.AddHeader("host", "www.huzhumaifang.com:8080");
            request.AddParameter("application/x-www-form-urlencoded", $"content={JsonConvert.SerializeObject(dicParameter)}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string result = response.Content;
            return result;
        }
    }
}