using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HouseCrawler.Core.Common
{
    public class RedisService
    {
        public static bool ContainsHouse(string key, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer
                .Connect(ConnectionStrings.RedisConnectionString);
            IDatabase db = redis.GetDatabase();
            if (db.KeyExists(key))
            {
                return true;
            }
            else
            {
                db.StringSet(key,"");
                return false;
            }
        }
    }
}
