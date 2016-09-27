using System;
using System.Collections.Concurrent;
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

        public ConcurrentBag<PVInfo> LstPVInfo { get; set; }
    }
}