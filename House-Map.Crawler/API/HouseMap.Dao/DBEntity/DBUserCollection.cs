using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace HouseMap.Dao.DBEntity
{
    [Serializable]
    [Table("UserCollection")]

    public class DBUserCollection : BaseEntity
    {
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }

        [JsonProperty(PropertyName = "userID")]
        public long UserID { get; set; }

        [JsonProperty(PropertyName = "houseID")]
        public string HouseID { get; set; }


        [JsonProperty(PropertyName = "onlineURL")]
        public string OnlineURL { get; set; }


        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "houseJson")]
        public string HouseJson{get;set;}


        [JsonProperty(PropertyName = "deleted")]
        public int Deleted { get; set; }


    }
}
