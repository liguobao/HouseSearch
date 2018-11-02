using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;

using HouseMap.Common;

namespace HouseMapAPI.Service
{
    public class DoubanService
    {

        public static bool CheckDBGroupData (string groupID, string cityName, int pageIndex)
        {
            List<DBHouse> lstHouseInfo = new List<DBHouse>();
            var apiURL = $"https://api.douban.com/v2/group/{groupID}/topics?start={pageIndex * 50}";
            //LogHelper.Info($"url:{apiURL},groupID:{groupID}, city:{cityName}");
            var result = GetAPIResult(apiURL);
            if (string.IsNullOrEmpty(result))
            {
                return false;
            }
            return !string.IsNullOrEmpty(JToken.Parse(result)?["topics"]?.ToString());
        }

        private static string GetAPIResult(string apiURL)
        {

            var client = new RestClient(apiURL);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("referer", apiURL);
            request.AddHeader("accept", "*/*");
            request.AddHeader("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("cookie", "bid=qLvbOle-G58; ps=y; ue=^\\^codelover^@qq.com^^; push_noty_num=0; push_doumail_num=0; __utmz=30149280.1521636704.1.1.utmcsr=(direct)^|utmccn=(direct)^|utmcmd=(none); __utmv=30149280.15460; ll=^\\^108296^^; _vwo_uuid_v2=D87414308A33790472DBB4D2B1DC0DE7B^|6a9fc300e5ea8c7485f9a922d87e820b; __utma=30149280.2046746446.1521636704.1522567163.1522675073.3");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";

        }

    }
}
