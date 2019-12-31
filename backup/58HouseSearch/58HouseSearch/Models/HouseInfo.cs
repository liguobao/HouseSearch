using _58HouseSearch.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _58HouseSearch.Models
{
    public class HouseInfo
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string HouseTitle { get; set; }

        /// <summary>
        /// 房间URL
        /// </summary>
        public string HouseURL { get; set; }

        /// <summary>
        /// 地理位置（一般用于定位）
        /// </summary>
        public string HouseLocation { get; set; }

        /// <summary>
        /// 价钱（可能非纯数字）
        /// </summary>
        public string Money { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string HouseTime { get; set; }

        /// <summary>
        /// 价格（纯数字）
        /// </summary>
        public decimal HousePrice { get; set; }

        /// <summary>
        /// 定位图标
        /// </summary>
        public string LocationMarkBG { get; set; }
    }
}