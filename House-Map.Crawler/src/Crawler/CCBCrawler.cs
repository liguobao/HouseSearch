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
using Microsoft.Extensions.Options;
using HouseMap.Models;
using HouseMap.Common;

namespace HouseMap.Crawler
{

    public class CCBCrawler : BaseCrawler,ICrawler
    {
        private readonly AppSettings _configuration;
        public CCBCrawler(IOptions<AppSettings> configuration, HouseDapper houseDapper, ConfigDapper configDapper)
         : base(houseDapper, configDapper)
        {
            this.Source = ConstConfigName.CCBHouse;
            _configuration = configuration.Value;
        }

        public override string GetJsonOrHTML(JToken config, int page)
        {
            string cityShortCutName = config["shortcutname"]?.ToString();
            return GetResultByAPI(cityShortCutName, page);
        }

        public override List<HouseInfo> ParseHouses(JToken config, string data)
        {
            throw new NotImplementedException();
        }


        private string GetResultByAPI(string cityShortCutName, int page)
        {
            string formBody = $"_reqParams=apiKey%3D{_configuration.CCBHomeAPIKey}%26city%3D{cityShortCutName}%26saleOrLease%3Dlease%26pageSize%3D50%26page%3D{page}%26propType%3D11%26tmflags%3D3&_interfaceUrl=%2Fhlsp%2Fcityhouse%2Fdeal%2Fsearch&_reqMethod=GET";
            var client = new RestClient("http://bankservice.home.ccb.com/LHECISM/LanHaiHttpResfulReqServlet");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("cookie2", "$Version=1");
            request.AddHeader("cookie", "UDC_Ser2018_ON=1; FAVOR=||||||||||||||||||||||||||||||||||||||||||||||||||; CCBIBS1=Qief3XbaeklLa3FZfwVHnEaGnFVRS3EOeEVxmbDcfhlWbc2zwKnabcKNraFiWdHUel1baPIKeD1K2yHHeSVJmyFMf0lvqqR9Vw3Xli; TC=249277366_1613198604_1362849648; UDC_ON=1; _BOA_mf_txcode_=HT0205");
            request.AddHeader("host", "bankservice.home.ccb.com");
            request.AddParameter("application/x-www-form-urlencoded", formBody, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            return "";
        }
    }
}