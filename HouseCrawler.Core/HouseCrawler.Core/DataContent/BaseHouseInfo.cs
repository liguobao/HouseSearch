using System;
using System.ComponentModel.DataAnnotations;


namespace HouseCrawler.Core
{
 
    public class BaseHouseInfo
    {

        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(2048)]
        public string HouseTitle { get; set; }

        [MaxLength(8000)]
        public string HouseText { get; set; }

        /// <summary>
        /// 房间URL
        /// </summary>
        [MaxLength(512)]
        [Key]
        public string HouseOnlineURL { get; set; }

        /// <summary>
        /// 地理位置（一般用于定位）
        /// </summary>
        [MaxLength(2048)]
        public string HouseLocation { get; set; }

        /// <summary>
        /// 价钱（可能非纯数字）
        /// </summary>
        [MaxLength(64)]
        public string DisPlayPrice { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [MaxLength(2048)]
        public DateTime ?PubTime { get; set; }

        /// <summary>
        /// 价格（纯数字）
        /// </summary>
        public decimal HousePrice { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        [MaxLength(64)]
  
        public string LocationCityName { get; set; }

        /// <summary>
        /// 来源网站
        /// </summary>
        [MaxLength(512)]
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
        [MaxLength(1024)]
        public string PicURLs { get; set; }

    }
}
