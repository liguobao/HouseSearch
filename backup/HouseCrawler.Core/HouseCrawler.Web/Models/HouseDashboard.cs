
using System;
using System.Collections.Generic;
using System.Linq;


namespace HouseCrawler.Web.Models
{
    public class HouseDashboard
    {
        public string CityName { get; set; }

        public int HouseSum { get; set; }

        public string Source { get; set; }

        public string DisplaySource
        {
            get
            {
                return ConstConfigName.ConvertToDisPlayName(this.Source);
            }
        }

        public DateTime LastRecordPubTime { get; set; }

    }



}
