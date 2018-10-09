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

        [JsonIgnore]
        public DateTime? CreateTime { get; set; }

        [JsonIgnore]
        public DateTime? UpdateTime { get; set; } = DateTime.Now;

         public T ToModel<T>()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
        }
    }
}