//============================================================
//http://codelover.link author:李国宝
//============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMap.Common;
using Newtonsoft.Json;

namespace HouseMap.Dao.DBEntity
{

    public class DBHouse : BaseEntity
    {


        public string Title { get; set; }


        public string Text { get; set; }

        public string PicURLs { get; set; }

        public string Location { get; set; }

        public string City { get; set; }

        public decimal Longitude { get; set; }


        public decimal Latitude { get; set; }

        public int RentType { get; set; }


        public string Tags { get; set; }

        public DateTime PubTime { get; set; }


        public string OnlineURL { get; set; }

        [JsonIgnore]
        public string JsonData { get; set; }


        public int Price { get; set; }

        /// <summary>
        /// 来源网站
        /// </summary>
        public string Source { get; set; }
    }


    public enum RentTypeEnum
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