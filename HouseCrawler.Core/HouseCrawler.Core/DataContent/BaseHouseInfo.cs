using System;
using System.ComponentModel.DataAnnotations;
using Nest;

namespace HouseCrawler.Core
{

    [ElasticsearchType(IdProperty = "HouseOnlineURL")]
    public class BaseHouseInfo
    {

        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string HouseTitle { get; set; }

        public string HouseText { get; set; }

        /// <summary>
        /// 房间URL
        /// </summary>
        [Key]
        public string HouseOnlineURL { get; set; }

        /// <summary>
        /// 地理位置（一般用于定位）
        /// </summary>
        public string HouseLocation { get; set; }

        /// <summary>
        /// 价钱（可能非纯数字）
        /// </summary>
        public string DisPlayPrice { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PubTime { get; set; }


        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PubDate { get { return this.PubTime.Date; } }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime DataCreateDate { get { return this.DataCreateTime.Date; } }
        public DateTime DataCreateTime { get; set; }

        /// <summary>
        /// 价格（纯数字）
        /// </summary>
        public decimal HousePrice { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string LocationCityName { get; set; }

        /// <summary>
        /// 来源网站
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 是否已分析
        /// </summary>
        public bool IsAnalyzed { get; set; }

        /// <summary>
        /// 状态（0:未处理 1：有效 2:已作废）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string PicURLs { get; set; }

    }
}
