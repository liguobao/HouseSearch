using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Collections.Concurrent;
using _58HouseSearch.Core.Models;
using System.Net.Http;

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
 }

    
}