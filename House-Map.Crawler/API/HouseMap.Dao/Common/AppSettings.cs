
using System.Runtime.Serialization;

namespace HouseMap.Common
{
    public class AppSettings
    {

        public string QCloudMySQL { get; set; }

        public string RedisConnectionString { get; set; }

        public string CCBHomeAPIKey { get; set; }

        public string SenderAddress { get; set; }

        public string EmailAccount { get; set; }

        public string EmailPassword { get; set; }

        public string ReceiverAddress { get; set; }

        public string ReceiverName { get; set; }


        public string EmailSMTPDomain { get; set; }

        public int EmailSMTPPort { get; set; }

        public string EncryptionConfigCIV { get; set; }

        public string EncryptionConfigCKEY { get; set; }

        public string QQAPPID { get; set; }

        public string QQAPPKey { get; set; }

        public string QQAuthReturnURL { get; set; }



        public string QQAPIID { get; set; }

        public string QQAPIKey { get; set; }

        public string QQAPIAuthReturnURL { get; set; }

        public string WechatAppId { get; set; }

        public string WechatAppSecret { get; set; }
        public string ESUserName { get; set; }
        public string ESPassword { get; set; }
        public string ESURL { get; set; }


        public string NodeProxyHost { get; set; }
    }
}
