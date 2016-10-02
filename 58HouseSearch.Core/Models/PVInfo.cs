
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
namespace _58HouseSearch.Core.Models
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

        [JsonIgnoreAttribute]
        public ConcurrentBag<PVInfo> SalesLstPVInfo;
    }
}