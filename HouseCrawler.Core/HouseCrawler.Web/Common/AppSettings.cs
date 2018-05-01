
namespace HouseCrawler.Web
{

    public class ConnectionStrings
    {
        internal static string RedisConnectionString { get; set; }

        public static string MySQLConnectionString { get; set; }

    }


    public class EncryptionConfig
    {
        public static string CIV { get; set; } //初始化向量

        public static string CKEY { get; set; }
    }

    public class EmailConfig
    {
        public static string Account{get;set;}

        public static string Password{get;set;}
    }


}
