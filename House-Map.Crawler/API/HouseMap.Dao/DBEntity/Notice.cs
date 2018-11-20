using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HouseMap.Dao.DBEntity
{
    [Table("notice")]
    public class Notice
    {
        public long Id { get; set; }


        public string Content { get; set; }
        public DateTime DataCreateTime { get; set; }

        public DateTime EndShowDate { get; set; }


        public DateTime DataChange_LastTime { get; set; }

    }
}
