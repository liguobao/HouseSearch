using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMapAPI.Common;
using Newtonsoft.Json;

namespace HouseMapAPI.DBEntity
{

    [Serializable]
    public class BaseEntity
    {
        public long ID { get; set; }

        public DateTime DataCreateTime { get; set; }

        public DateTime DataChangeLastTime { get; set; } = DateTime.Now;
    }
}