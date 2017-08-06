using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Common
{
    public class DomainProxyInfo
    {
        public List<ProxyIPItem> lstDoubanProxyIP { get; set; }


        private static DomainProxyInfo domainProxyInfo;

        public static void InitDomainProxyInfo(string path)
        {
            LogHelper.RunActionNotThrowEx(() =>
            {
                if (domainProxyInfo != null)
                    return;
                using (var stream = new FileStream(path, FileMode.Open))
                {

                    StreamReader sr = new StreamReader(stream);
                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new JavaScriptDateTimeConverter() }
                    };
                    //构建Json.net的读取流  
                    using (var reader = new JsonTextReader(sr))
                    {
                        domainProxyInfo = serializer.Deserialize<DomainProxyInfo>(reader);
                    }
                }
            }, "InitDomainProxyInfo", path);

        }

        public static DomainProxyInfo GetDomainProxyInfo()
        {
            return domainProxyInfo;
        }


        public static ProxyIPItem GetRandomProxyIPItem()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            var index = random.Next(domainProxyInfo.lstDoubanProxyIP.Count);
            if (index == 0)
                return null;
            return domainProxyInfo.lstDoubanProxyIP[index];
        }
    }


    /// <summary>
    /// 实现WebProxy接口
    /// </summary>
    public class CrawlerProxyInfo : IWebProxy
    {
        public CrawlerProxyInfo(string proxyUri)
            : this(new Uri(proxyUri))
        {
        }

        public CrawlerProxyInfo(Uri proxyUri)
        {
            ProxyUri = proxyUri;
        }

        public Uri ProxyUri { get; set; }

        public ICredentials Credentials { get; set; }

        public Uri GetProxy(Uri destination) => ProxyUri;

        public bool IsBypassed(Uri host) => false;/* Proxy all requests */
    }

}
