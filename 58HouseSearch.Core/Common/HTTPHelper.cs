using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace _58HouseSearch.Core
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
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }


        public static string GetHTML(string url)
        {
            try
            {
                Client.DefaultRequestHeaders.ExpectContinue = true;
                var task = Client.GetStringAsync(url);
                return task.Result; 
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }
       
    }


}