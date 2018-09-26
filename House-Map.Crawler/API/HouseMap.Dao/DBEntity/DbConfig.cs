using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HouseMap.Dao.DBEntity
{
    [Serializable]
    [Table("Config")]
    public class DbConfig : BaseEntity
    {
        public string City { get; set; }

        public string Source { get; set; }

        public int PageCount { get; set; }

        public string Json { get; set; }
    }
}
