using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Common
{
    public class ProxyIPItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> authenticationHeaders { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int avgScore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cloud { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int failedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int referCount { get; set; }
    }
}
