using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HouseMap.Dao.DBEntity
{
    [Table("CrawlerConfigurations")]
    public class CrawlerConfig
    {
        public long Id { get; set; }

        [MaxLength(255)]
        public string ConfigurationName { get; set; }

        [MaxLength(4096)]
        public string ConfigurationValue { get; set; }

        public DateTime DataCreateTime { get; set; }

        public bool IsEnabled { get; set; }

        public int ConfigurationKey { get; set; }
    }
}
