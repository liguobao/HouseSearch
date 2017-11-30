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


        public static string GetHTML(string url, List<string> LstProxyID = null)
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
            try
            {
                myClient.DefaultRequestHeaders.ExpectContinue = true;
                return myClient.GetStringAsync(url).Result;
            }
            catch (System.Exception ex)
            {
                LogHelper.Info(url);
                LogHelper.Error("GetHTMLForDouban Exception", ex, new { URL = url });
                Console.WriteLine(ex.ToString());
                return string.Empty;

            }
        }
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

}