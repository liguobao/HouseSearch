using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace HouseCrawler.Core
{
    public class RedisService
    {
        public static IDatabase database = ConnectionMultiplexer
                .Connect(ConnectionStrings.RedisConnectionString).GetDatabase();

        public static List<BaseHouseInfo> ReadSearchCache(string key)
        {
            try
            {
                ConfigurationOptions options = ConfigurationOptions.Parse(ConnectionStrings.RedisConnectionString);
                options.SyncTimeout = 10 * 1000;
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
                IDatabase db = redis.GetDatabase();
                if (db.KeyExists(key))
                {
                    string houseJson = db.StringGet(key);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<BaseHouseInfo>>(houseJson);
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error("ReadSearchCache", ex);
                return null;
            }

        }

        public static void WriteSearchCache(string key, List<BaseHouseInfo> house)
        {
            try
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer
                .Connect(ConnectionStrings.RedisConnectionString);
                IDatabase db = redis.GetDatabase();
                db.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(house), new System.TimeSpan(0, 30, 0));
            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteSearchCache", ex);
            }

        }



        public static string ReadCache(string key)
        {
            try
            {
                ConfigurationOptions options = ConfigurationOptions.Parse(ConnectionStrings.RedisConnectionString);
                options.SyncTimeout = 10 * 1000;
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);
                IDatabase db = redis.GetDatabase();
                return db.KeyExists(key) == true ? db.StringGet(key).ToString() : null;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ReadCache", ex);
                return null;
            }


        }


        public static void WriteCache(string key, string value)
        {

            try
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer
                .Connect(ConnectionStrings.RedisConnectionString);
                IDatabase db = redis.GetDatabase();
                db.StringSet(key, value, new System.TimeSpan(0, 30, 0));
            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteCache", ex);
            }

        }
    }
}
