//============================================================
//http://codelover.link author:李国宝
//============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using HouseCrawler.Web.Common;
using Newtonsoft.Json;

namespace HouseCrawler.Web
{
    [Serializable()]

    /// <summary>
    /// 
    /// </summary>
    public class UserHouse : HouseInfo
    {
        public long UserId { get; set; }

    }
}