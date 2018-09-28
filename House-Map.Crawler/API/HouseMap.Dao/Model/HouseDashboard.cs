
using System;
using System.Collections.Generic;
using System.Linq;
using HouseMap.Dao.DBEntity;
using Newtonsoft.Json;

namespace HouseMap.Models
{
    public class HouseDashboard
    {
        [JsonProperty(PropertyName = "cityName")]
        public string CityName { get; set; }

        [JsonProperty(PropertyName = "houseSum")]
        public int HouseSum { get; set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; set; }


        [JsonProperty(PropertyName = "displaySource")]
        public string DisplaySource
        {
            get
            {
                return ConstConfigName.ConvertToDisPlayName(this.Source);
            }
        }

    }



}
