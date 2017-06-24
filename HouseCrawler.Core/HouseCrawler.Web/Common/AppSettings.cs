using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HouseCrawler.Web
{
    public class AppSettings
    {
        public static string CityJsonFilePath = "";
    }

    public class ConnectionStrings
    {
        public static string MySQLConnectionString { get; set; }


    }
}
