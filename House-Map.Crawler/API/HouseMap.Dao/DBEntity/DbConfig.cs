using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace HouseMap.Dao.DBEntity
{
    [Serializable]
    [Table("Config")]
    public class DBConfig : BaseEntity
    {
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "displaySource")]
        [NotMapped]
        public string DisplaySource
        {
            get
            {
                return SourceTool.GetDisplayName(this.Source);
            }
        }

        [JsonProperty(PropertyName = "pageCount")]
        public int PageCount { get; set; }

        [JsonProperty(PropertyName = "json")]
        public string Json { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        [NotMapped]
        public int HouseCount { get; set; }
    }
}
