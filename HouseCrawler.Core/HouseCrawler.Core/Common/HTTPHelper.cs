using HouseCrawler.Core.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HouseCrawler.Core
{
    public class HTTPHelper
    {

        public static HttpClient Client { get; } = new HttpClient();

        public static string GetHTMLByURL(string url)
        {
            try
            {
                var html = Client.GetStringAsync(url).Result;
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

        public static HttpClient myClient { get; } = new HttpClient(new HtmlTextHandler());

    }



    class HtmlTextHandler : HttpClientHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var contentType = response.Content.Headers.ContentType;
            contentType.CharSet = await getCharSetAsync(response.Content);

            return response;
        }

        private async Task<string> getCharSetAsync(HttpContent httpContent)
        {
            var charset = httpContent.Headers.ContentType.CharSet;
            if (!string.IsNullOrEmpty(charset))
                return charset;

            var content = await httpContent.ReadAsStringAsync();
            var match = Regex.Match(content, @"charset=(?<charset>.+?)""", RegexOptions.IgnoreCase);
            if (!match.Success)
                return charset;

            return match.Groups["charset"].Value;
        }
    }



    public class DoubanHTTPHelper

    {
        private static HttpClient doubanHttpClient;

        private static CookieCollection cookieCollection;

        private static HttpClientHandler handler = new HttpClientHandler();

        public static void InitCookieCollection()
        {
            handler.UseCookies = true;

            doubanHttpClient = new HttpClient(handler) {
                DefaultRequestHeaders =
                {
                    { "Cache-Control", "max-age=0" },
                    { "Origin", "https://www.douban.com" },
                    { "Upgrade-Insecure-Requests", "1" },
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36" },
                    { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" },
                }
            };
          
            #region 获取登录页Cookie
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://accounts.douban.com/login?source=group"),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("Host", "accounts.douban.com");
            request.Headers.Add("Connection", "Keep-Alive");
            request.Headers.Add("Referer", "https://www.douban.com/group");
            var response = doubanHttpClient.SendAsync(request).Result;
            var setCookieValue = response.Headers.GetValues("Set-Cookie");
            #endregion

            #region 登陆formContent

            var formContent = new FormUrlEncodedContent(new[]
           {
                new KeyValuePair<string, string>("source", "source"),
                new KeyValuePair<string, string>("redir", "https://www.douban.com/group/"),
                new KeyValuePair<string, string>("form_email", AppSettings.DoubanAccount),
                new KeyValuePair<string, string>("form_password", AppSettings.DoubanPassword),
                new KeyValuePair<string, string>("login", "登录"),
            });

            #endregion


            #region 登陆请求
            var loginRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://accounts.douban.com/login"),
                Method = HttpMethod.Post,
            };
            loginRequest.Headers.Add("Set-Cookie", setCookieValue);
            loginRequest.Content = formContent;
            loginRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var loginResult = doubanHttpClient.SendAsync(loginRequest).Result;
            cookieCollection = handler.CookieContainer.GetCookies(new Uri("https://douban.com")); // Retrieving a Cookie
            LogHelper.Info($"豆瓣登陆结果，result：{Newtonsoft.Json.JsonConvert.SerializeObject(loginResult)}," +
                $"cookieCollection:{Newtonsoft.Json.JsonConvert.SerializeObject(cookieCollection)}");

            #endregion
        }


        public static string GetHTMLForDouban(string url)
        {
            try
            {
                doubanHttpClient.DefaultRequestHeaders.ExpectContinue = true;
                var rsp = doubanHttpClient.GetAsync(url).Result;
                return rsp.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"{url}", ex);
                return string.Empty;

            }
        }
    }

}