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

        public string DisplaySource
        {
            get
            {
                return ConstConfigName.ConvertToDisPlayName(this.Source);
            }
        }

        public int PageCount { get; set; }

        public string Json { get; set; }

        public int Score { get; set; }
    }
}
