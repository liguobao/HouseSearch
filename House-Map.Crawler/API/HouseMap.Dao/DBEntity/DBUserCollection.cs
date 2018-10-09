using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HouseMap.Dao.DBEntity
{
    [Serializable]
    [Table("UserCollection")]
    public class DBUserCollection : BaseEntity
    {
        public string City { get; set; }

        public string Source { get; set; }

        public long UserID { get; set; }

        public string HouseID { get; set; }

        public string OnlineURL {get;set;}

        public string Title { get; set; }

        public int Deleted { get; set; }


    }
}
