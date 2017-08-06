using HouseCrawler.Core.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HouseCrawler.Core
{
    public class HTTPHelper
    {

        public static HttpClient Client { get; } = new HttpClient
        {
             DefaultRequestHeaders =
            {
                { "Accept-Encoding", "gzip, deflate" },
                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393" }
            }
        };

        public static string GetHTMLByURL(string url)
        {
            try
            {
                var html =  Client.GetStringAsync(url).Result;
                return html;         
            }
            catch (Exception ex)
            {
                LogHelper.Error("GetHTMLByURL Exception", ex, new { URL = url });
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }


        public static HttpClient AndroidHttpClient { get; } = new HttpClient
        {
            DefaultRequestHeaders =
            {
                {"User-Agent","Android" }
            }
        };

        public static string GetJsonResultByURL(string postUrl)
        {
            var result = AndroidHttpClient.PostAsync(postUrl, new StringContent("")).Result;
            return result.Content.ReadAsStringAsync().Result;
        }




        public static string GetHTML(string url,List<string> LstProxyID=null)
        {
            try
            {
                Client.DefaultRequestHeaders.ExpectContinue = true;
                var task = Client.GetStringAsync(url);
                return task.Result; 
            }
            catch (System.Exception ex)
            {
                LogHelper.Info(url);
                LogHelper.Error("GetHTMLByURL Exception", ex, new { URL = url });
                Console.WriteLine(ex.ToString());
                return string.Empty;
             
            }
        }

        public static string GetHTMLForDouban(string url)
        {
            var proxyIPItem = DomainProxyInfo.GetRandomProxyIPItem();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            if (proxyIPItem != null)
            {
                httpClientHandler = new HttpClientHandler
                {
                    Proxy = new CrawlerProxyInfo($"http://{proxyIPItem.ip}:{proxyIPItem.port}"),
                    UseProxy = true
                };
                LogHelper.Info("URL:" + url + ";ProxyIP:" + Newtonsoft.Json.JsonConvert.SerializeObject(proxyIPItem));
                Console.WriteLine("URL:" + url + ";ProxyIP:" + Newtonsoft.Json.JsonConvert.SerializeObject(proxyIPItem));
            }

            try
            {
              
                
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.Timeout = new TimeSpan(0,3,0);
                httpClient.DefaultRequestHeaders.ExpectContinue = true;
                var task = httpClient.GetStringAsync(url);
                var htmlDoc= task.Result;
                if (htmlDoc != null)
                {
                    LogHelper.Info("GetStringAsync Success:" + url + ";ProxyIP:" + Newtonsoft.Json.JsonConvert.SerializeObject(proxyIPItem));
                }
                return htmlDoc;
            }
            catch (System.Exception ex)
            {
                if (proxyIPItem != null)
                {
                    DomainProxyInfo.GetDomainProxyInfo().lstDoubanProxyIP.Remove(proxyIPItem);
                    Console.WriteLine("Remove ProxyIP:" + Newtonsoft.Json.JsonConvert.SerializeObject(proxyIPItem));
                }
                LogHelper.Error("GetHTMLByURL Exception", ex.InnerException, new { URL = url });
                Console.WriteLine(ex.InnerException.ToString());
                return string.Empty;

            }
        }



    }



    

}