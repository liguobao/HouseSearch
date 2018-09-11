using Dapper;
using HouseCrawler.Web.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseCrawler.Web
{
    public class AnnouncementDapper : BaseDapper
    {
        public AnnouncementDapper(IOptions<APPConfiguration> configuration, RedisService redisService)
        : base(configuration, redisService)
        {
        }
    }
}