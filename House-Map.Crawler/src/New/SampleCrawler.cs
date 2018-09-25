using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Dapper;
using AngleSharp.Dom;
using HouseMap.Crawler.Dapper;
using HouseMap.Crawler.Common;
using HouseMap.Crawler.DBEntity;
using Newtonsoft.Json.Linq;

namespace HouseMap.Crawler
{

    public class SampleCrawler : BaseCrawler
    {

        public SampleCrawler(HouseDapper houseDapper, ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = ConstConfigName.Beike;
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            throw new NotImplementedException();
        }

        public override List<BaseHouseInfo> ParseHouses(JToken config, string data)
        {
            throw new NotImplementedException();
        }
    }
}