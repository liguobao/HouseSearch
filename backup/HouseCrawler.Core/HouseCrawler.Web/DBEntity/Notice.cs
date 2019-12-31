using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Web
{
    public class Notice
    {
        public long Id { get; set; }


        public string Content { get; set; }
        public DateTime DataCreateTime { get; set; }

        public DateTime EndShowDate { get; set; }


        public DateTime DataChange_LastTime { get; set; }

    }
}
