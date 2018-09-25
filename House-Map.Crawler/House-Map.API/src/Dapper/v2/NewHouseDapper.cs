using Dapper;
using HouseMapAPI.Common;
using HouseMapAPI.DBEntity;
using HouseMapAPI.Models;
using HouseMapAPI.Service;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMapAPI.Dapper
{
    public class NewHouseDapper : BaseDapper
    {
        public NewHouseDapper(IOptions<AppSettings> options) : base(options)
        {
        }

    }
}