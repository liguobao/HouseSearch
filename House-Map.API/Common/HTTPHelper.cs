using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HouseCrawler.Web
{
    public class HTTPHelper
    {

        public static HttpClient ClientWithHtmlText { get; } = new HttpClient(new HtmlTextHandler()) ;


        public static string GetHTMLByURL(string url)
        {
            try
            {
                var html = ClientWithHtmlText.GetStringAsync(url).Result;
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

        public static string GetHTML(string url)
        {
            try
            {
                ClientWithHtmlText.DefaultRequestHeaders.ExpectContinue = true;
                var task = ClientWithHtmlText.GetStringAsync(url);
                return task.Result; 
            }
            catch (System.Exception ex)
            {
                LogHelper.Error("GetHTMLByURL Exception", ex, new { URL = url });
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