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
using System.Security.Cryptography;
using System.Text;

namespace HouseCrawler.Core.Crawlers
{
    public class ZuberHouseCrawler
    {
        static string API_VERSION = "v3/";
        static string SCENE = "2567a5ec9705eb7ac2c984033e06189d";

        public static void Crawler()
        {
            var cityName = "上海";
            var sequence = "";
            var referer = "http://www.zuber.im/app/?house=1";
            string url = $"https://api.zuber.im/v3/rooms/search?city={cityName}&house=1&sex=&cost1=&cost2=&address=&start_time=&end_time=&longitude=&latitude=&subway_line=&subway_station=&short_rent=&type=&bed=true&room_type_affirm=&coords_type=gaode&sequence={sequence}";
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);

            request.AddHeader("connection", "keep-alive");
            request.AddHeader("referer", referer);
            request.AddHeader("accept", "application/json, text/plain, */*");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("origin", "http://www.zuber.im");
            var _stamp = GetTimestamp().ToString();
            var _secret = "";
            var _token = md5(_stamp);
            var _scene = SCENE;
            var _method = "get";
            var _param = "{}";//(_method === 'get' || _method === 'delete') ? '{}' : JSON.stringify(param)
            var api = "rooms/search";
            var _source = "request_url=" + API_VERSION + api + "&" +
                "content=" + _param + "&" +
                "request_method=" + _method + '&' +
                "timestamp=" + _stamp + '&' +
                "secret=" + _secret;

            var _signature = md5(_source);


            var _auth = "timestamp=" + _stamp + ";" +
                "oauth2=" + _token + ";"
                + "signature=" + _signature + ";"
                + "scene=" + _scene;

            request.AddHeader("authorization", _auth);
            IRestResponse response = client.Execute(request);
        }



        public static string md5(string observedText)
        {
            string result;
            using (MD5 hash = MD5.Create())
            {
                result = String.Join
                (
                    "",
                    from ba in hash.ComputeHash
                    (
                        Encoding.UTF8.GetBytes(observedText)
                    )
                    select ba.ToString("x2")
                );
            }
            return result;
        }

        public static int GetTimestamp()
        {
            return (int)(DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }
    }
}
