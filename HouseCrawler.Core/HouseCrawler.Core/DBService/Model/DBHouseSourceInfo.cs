using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Web.DBService.Model
{
    public class DBHouseSourceInfo
    {
        public string CityName { get; set; }

        public long HouseSum { get; set; }

        public string Source { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
