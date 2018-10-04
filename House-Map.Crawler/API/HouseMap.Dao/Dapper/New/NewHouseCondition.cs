using Dapper;
using HouseMap.Dao.DBEntity;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HouseMap.Dao
{
    public class NewHouseCondition
    {
        public string City { get; set; }
        public string Source { get; set; } = "";
        public int Size { get; set; } = 600;
        public int IntervalDay { get; set; } = 14;
        public string Keyword { get; set; } = "";
        public bool Refresh { get; set; }
        public int Page { get; set; } = 0;

        public int? RentType { get; set; }
        public int FromPrice { get; set; } = 0;
        public int ToPrice { get; set; } = 0;

        public string RedisKey
        {
            get
            {

                var key = $"{this.City}-{this.Source}-{this.IntervalDay}-{this.Page}-{this.Size}-{this.Keyword}";
                if (this.FromPrice > 0 && this.ToPrice > 0 && this.FromPrice <= this.ToPrice)
                {
                    key = key + $"-{this.FromPrice}-{this.ToPrice}";
                }
                if (RentType != null)
                {
                    key = key + $"-{this.RentType}";
                }
                return key;
            }
        }

        public DateTime PubTime
        {
            get
            {
                return DateTime.Now.Date.AddDays(-IntervalDay);
            }
        }

        public string LikeKeyWord
        {
            get { return $"%{Keyword}%"; }
        }

        public string QueryText
        {
            get
            {
                if (string.IsNullOrEmpty(this.Source))
                {
                    return "";
                }
                var tableName = SourceTool.GetHouseTableNameDic()[this.Source];
                string queryText = @"SELECT Id,
                                            OnlineURL,
                                            Title,
                                            Location,
                                            Price,
                                            PubTime,
                                            City,
                                            Source,
                                            PicURLs,
                                            Labels,
                                            Tags,
                                            RentType,
                                            Latitude,
                                            Longitude,
                                            Text"
                                   + $" from { tableName } where 1=1 "
                                   + $" and City = @City and  PubTime >= @PubTime ";
                if (!string.IsNullOrEmpty(this.Keyword))
                {
                    queryText = queryText + " and (Text like @LikeKeyWord or Title like @LikeKeyWord) ";
                }
                if (this.FromPrice > 0 && this.ToPrice >= 0 && this.FromPrice <= this.ToPrice)
                {
                    queryText = queryText + $" and (Price >= {this.FromPrice} and Price <={this.ToPrice}) "
                    + $" order by Price, PubTime limit {this.Size * this.Page}, {this.Size}";
                }
                else
                {
                    queryText = queryText + $" order by PubTime desc limit {this.Size * this.Page}, {this.Size} ";
                }
                return queryText;

            }
        }
    }




}
