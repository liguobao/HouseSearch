using HouseCrawler.Web.DataContent;
using StackExchange.Redis;
using System.Collections.Generic;

namespace HouseCrawler.Web
{
    public class RedisService
    {
        public static List<DBHouseInfo> ReadSearchCache(string key)
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(ConnectionStrings.RedisConnectionString);
            options.SyncTimeout =10 * 1000;
            ConnectionMultiplexer redis = ConnectionMultiplexer .Connect(options);
            IDatabase db = redis.GetDatabase();
            if (db.KeyExists(key))
            {
                string houseJson = db.StringGet(key);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DBHouseInfo>>(houseJson);
            }
            else
            {
                return null;
            }
        }

        public static void WriteSearchCache(string key, List<DBHouseInfo> house)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer
                .Connect(ConnectionStrings.RedisConnectionString);
            IDatabase db = redis.GetDatabase();
            db.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(house), new System.TimeSpan(1,0,0));
        }
    }
}
