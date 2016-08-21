using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _58HouseSearch.Models
{
    public class PVInfo
    {
        public string PVIP { get; set; }

        public string PVTime { get; set; }

        public string PVActionAddress { get; set; }

    }

    public class WebPVInfo
    {
        public long PVCount { get; set; }

        public List<PVInfo> LstPVInfo { get; set; }
    }
}