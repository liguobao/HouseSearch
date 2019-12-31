using System;
using System.Collections.Generic;
using HouseMap.Dao.DBEntity;

namespace HouseMapAPI.Models
{
    public class SourceDashboard
    {
        public int id { get; set; }

        public string city { get; set; }

        public List<DBConfig> sources { get; set; }
    }

    public class DoubanSource
    {
        public string groupId { get; set; }

        public string city { get; set; }
    }
}