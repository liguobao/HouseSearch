using System;
using HouseMap.Common;
using HouseMap.Dao.DBEntity;
using MongoDB.Bson.Serialization.Attributes;

namespace HouseMapConsumer.Dao.DBEntity
{


    public class MongoHouseEntity
    {


        public MongoHouseEntity(DBHouse dbHouse)
        {
            this.Id = dbHouse.Id;
            this.CreateTime = dbHouse.CreateTime ?? DateTime.Now;
            this.UpdateTime = dbHouse.UpdateTime ?? DateTime.Now;
            this.Title = dbHouse.Title;
            this.Text = dbHouse.Text ?? dbHouse.Title;
            this.PicURLs = dbHouse.PicURLs;
            this.Location = dbHouse.Location;
            this.City = dbHouse.City;
            if (!string.IsNullOrEmpty(dbHouse.Longitude) && double.TryParse(dbHouse.Longitude, out var longitude))
            {
                this.Longitude = longitude;
            }
            if (!string.IsNullOrEmpty(dbHouse.Latitude) && double.TryParse(dbHouse.Latitude, out var latitude))
            {
                this.Latitude = latitude;
            }
            this.RentType = dbHouse.RentType;
            this.Tags = dbHouse.Tags;
            this.Labels = dbHouse.Labels;
            this.PubTime = dbHouse.PubTime;
            this.Timestamp = Tools.GetTimestamp(dbHouse.PubTime);
            this.Status = dbHouse.Status;
            this.OnlineURL = dbHouse.OnlineURL;
            this.Price = dbHouse.Price;
            this.Source = dbHouse.Source;
        }

        [BsonId]
        public string Id { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string PicURLs { get; set; }


        public string Location { get; set; }

        public string City { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }


        public int RentType { get; set; }

        public string Tags { get; set; }

        public string Labels { get; set; }

        public DateTime PubTime { get; set; }

        public long Timestamp { get; set; }

        public int Status { get; set; } = 0;

        public string OnlineURL { get; set; }

        public int Price { get; set; }

        public string Source { get; set; }

    }


    public class MongoHouseQuery
    {
        /// <summary>
        /// 城市名
        /// </summary>
        public string city { get; set; } = "上海";

        /// <summary>
        /// 来源
        /// </summary>
        public string source { get; set; } = "";

        /// <summary>
        /// 起始价格
        /// </summary>
        public int? fromPrice { get; set; }

        /// <summary>
        /// 结束价格
        /// </summary>
        public int? toPrice { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; } = 0;

        /// <summary>
        /// 页数
        /// </summary>
        public int pageSize { get; set; } = 20;

        /// <summary>
        /// 房间类型 0 未知, 1 合租, 2 单间, 3 整套出租, 4 公寓
        /// </summary>
        public int? rentType { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? longitude { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public double? latitude { get; set; }

        /// <summary>
        /// 距离半径（公里），根据一个给定经纬度的点和距离，进行附近地点查询
        /// </summary>
        public double distance { get; set; } = 5;

    }
}