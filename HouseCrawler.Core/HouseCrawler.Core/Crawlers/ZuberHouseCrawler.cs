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

namespace HouseCrawler.Core.Crawlers
{
    public class ZuberHouseCrawler
    {
        public static void Crawler()
        {
            var client = new RestClient("https://api.zuber.im/v3/search/room?city=%E4%B8%8A%E6%B5%B7&has_short_rent=0&has_video=&subway_line=&sex=&type=&room_type_affirm=&sequence=1523771866&longitude=&latitude=");
            var request = new RestRequest(Method.GET);
            request.AddHeader("postman-token", "5b3f498e-11b2-1286-1ed8-03a713e79af0");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("connection", "keep-alive");
            request.AddHeader("referer", "https://mobile.zuber.im/search/rent?city=%E4%B8%8A%E6%B5%B7&has_short_rent=0&has_video=&subway_line=&sex=&type=&room_type_affirm=&sequence=&longitude=&latitude=");
            request.AddHeader("accept", "application/json, text/plain, */*");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("origin", "https://mobile.zuber.im");
            request.AddHeader("authorization", "timestamp=1523773613;oauth2=dceb50a2e47c7f83cf63bdc609250d33;signature=f086fd4ca8842315f343a0f0c9169dfa;scene=2567a5ec9705eb7ac2c984033e06189d;deploykey=d626ca3232d521d65f234512");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
