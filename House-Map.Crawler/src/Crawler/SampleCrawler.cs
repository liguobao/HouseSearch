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
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMap.Crawler.Common;

using Newtonsoft.Json.Linq;
using HouseMap.Models;

namespace HouseMap.Crawler
{

    public class SampleCrawler : BaseCrawler,ICrawler
    {

        public SampleCrawler(HouseDapper houseDapper,ConfigDapper configDapper) : base(houseDapper, configDapper)
        {
            this.Source = "Test";
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            throw new NotImplementedException();
        }

        public override List<HouseInfo> ParseHouses(JToken config, string data)
        {
            throw new NotImplementedException();
        }
    }
}