//============================================================
//http://codelover.link author:李国宝
//============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using HouseMapAPI.Common;
using Newtonsoft.Json;

namespace HouseMapAPI.DBEntity
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