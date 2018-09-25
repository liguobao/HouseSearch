using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMap.Common;
using Newtonsoft.Json;

namespace HouseMap.Dao.DBEntity
{

    [Serializable]
    public class BaseEntity
    {
        public string Id { get; set; }

        public DateTime DataCreateTime { get; set; }

        public DateTime DataChangeLastTime { get; set; } = DateTime.Now;
    }
}