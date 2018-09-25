using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using HouseMapAPI.Common;
using Newtonsoft.Json;

namespace HouseMapAPI.DBEntity
{

    [Serializable]
    [Table("UserHouses")]
    public class UserHouse : DBHouse
    {
        public long UserId { get; set; }
    }
}