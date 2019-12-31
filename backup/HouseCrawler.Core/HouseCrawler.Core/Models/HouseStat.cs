
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Models
{
    public class HouseStat
    {

        public int HouseSum { get; set; }

        public string Source { get; set; }

        public DateTime LastPubTime { get; set; }

        public DateTime LastCreateTime { get; set; }
    }



}
