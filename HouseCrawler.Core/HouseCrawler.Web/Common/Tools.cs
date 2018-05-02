using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HouseCrawler.Web
{
    public static class Tools
    {
        
        public static string GetMD5(string observedText)
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

        public static string GetSearchTimeStamp()
        {
           return DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss");
        }
    }
}