using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HouseCrawler.Core.Common;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HouseCrawler.Core
{
    public class AppSettings
    {
        public static string CityJsonFilePath = "";

        public static string DoubanAccount { get; set; }

        public static string DoubanPassword { get; set; }

    }

    public class ConnectionStrings
    {
        public static string MySQLConnectionString { get; set; }

       
    }
}
