using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMap.Common;
using Nest;
using Newtonsoft.Json;

namespace HouseMap.Dao.DBEntity
{

    [ElasticsearchType(IdProperty = "OnlineURL")]
    public class DBPeopleIntention
    {
        public string Title { get; set; }

        public string Text { get; set; }


         public string Source { get; set; }



        public string Location { get; set; }



        public string City { get; set; }



        public string Longitude { get; set; }

        public string DateDetail { get; set; }



        public string Latitude { get; set; }



        public int RentType { get; set; }


        public string Tags { get; set; }

        public string PriceSection { get; set; }

        public int Price { get; set; }

        public DateTime PubTime { get; set; }


        public string OnlineURL { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string JsonData { get; set; }
    }


    public enum IntentionTypeEnum
    {
        [DescriptionAttribute("未知")]
        Undefined = 0,

        [DescriptionAttribute("合租")]
        Shared = 1,

        [DescriptionAttribute("单间")]
        OneRoom = 2,

        [DescriptionAttribute("整套出租")]
        AllInOne = 3,

        [DescriptionAttribute("公寓")]
        Apartment = 4
    }

}