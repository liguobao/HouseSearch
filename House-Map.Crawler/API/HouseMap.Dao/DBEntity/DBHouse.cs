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

    [ElasticsearchType(IdProperty = "Id")]
    public class DBHouse : BaseEntity
    {
        public string Title { get; set; }



        public string Text { get; set; }



        public string PicURLs { get; set; }


        public string Location { get; set; }



        public string City { get; set; }



        public string Longitude { get; set; }



        public string Latitude { get; set; }



        public int RentType { get; set; }


        public string Tags { get; set; }


        public string Labels { get; set; }



        public DateTime PubTime { get; set; }

        public int Status { get; set; } = 0;

        public string OnlineURL { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string JsonData { get; set; }



        [NotMapped]
        public string Icon
        {
            get
            {
                if (this.Price > 0)
                {
                    return LocationMarkBGType.SelectColor(this.Price / 1000);
                }
                return "";
            }
        }


        [NotMapped]
        public List<string> Pictures
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(PicURLs))
                    {
                        return new List<string>();
                    }
                    return JsonConvert.DeserializeObject<List<String>>(PicURLs);
                }
                catch
                {
                    return new List<string>();
                }
            }
        }


        public int Price { get; set; }


        public string Source { get; set; }

        [NotMapped]
        public string DisplaySource
        {
            get
            {
                return SourceTool.GetDisplayName(this.Source);
            }
        }
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

    public enum HouseStatusEnum
    {
        [DescriptionAttribute("已创建")]
        Created = 0,

        [DescriptionAttribute("已删除/废弃")]
        Deleted = 1,

        [DescriptionAttribute("已分析")]
        Analyzed = 2,

        [DescriptionAttribute("高品质")]
        HighGrade = 3,


        [DescriptionAttribute("一般")]
        General = 4,

        [DescriptionAttribute("低品质")]
        LowGrade = 5
    }



}