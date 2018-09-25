using Dapper;
using HouseMap.Common;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMap.Dao
{
    public class NewHouseDapper : BaseDapper
    {
        public NewHouseDapper(IOptions<AppSettings> options) : base(options)
        {
        }

    }
}