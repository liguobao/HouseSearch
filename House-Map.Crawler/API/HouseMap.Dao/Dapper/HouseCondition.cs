using HouseMap.Dao.DBEntity;
using System;

namespace HouseMap.Dao
{
    public class DBHouseQuery
    {
        /// <summary>
        /// 城市名
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; } = "";

        /// <summary>
        /// 分页数量
        /// </summary>
        public int Size { get; set; } = 0;

        /// <summary>
        /// 关键字搜索
        /// </summary>
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 是否刷新数据
        /// </summary>
        public bool Refresh { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 0;

        /// <summary>
        /// 房源类型
        /// </summary>
        public int? RentType { get; set; }

        /// <summary>
        /// 起始价格
        /// </summary>
        public int FromPrice { get; set; } = 0;

        /// <summary>
        /// 结束价格
        /// </summary>
        public int ToPrice { get; set; } = 0;


        public string Query { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? PubTime { get; set; }

        public string RedisKey
        {
            get
            {

                var key = $"{this.City}-{this.Source}-p{this.Page}-s{this.Size}-{this.Keyword}";
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
                var tableName = SourceTool.GetHouseTableName(this.Source);
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
                                   + $" and City = @City ";
                if (!string.IsNullOrEmpty(this.Keyword))
                {
                    queryText = queryText + " and (Text like @LikeKeyWord or Title like @LikeKeyWord) ";
                }
                if (this.PubTime == null)
                {
                    // 默认查询60天内的数据
                    this.PubTime = DateTime.Now.Date.AddDays(-60);
                    queryText = queryText + " and PubTime >= @PubTime  ";
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
