using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Collections.Concurrent;
using _58HouseSearch.Core.Models;

namespace _58HouseSearch.Core
{
    public class HTTPHelper
    {

        public static string GetHTMLByURL(string Url, string type = "UTF-8")
        {
            try
            {
                Url = Url.ToLower();

                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponseAsync().Result;
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }

        }
 }

    
}