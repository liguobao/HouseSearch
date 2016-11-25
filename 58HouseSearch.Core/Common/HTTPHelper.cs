using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace _58HouseSearch.Core
{
    public class HTTPHelper
    {

        public static HttpClient Client { get; } = new HttpClient();

        public static string GetHTMLByURL(string url)
        {
            try
            {
                return Client.GetStringAsync(url).Result;          
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
                Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                Client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "4.0"));
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