using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace HouseMap.Common
{
    public class RedisTool
    {
        private ConfigurationOptions GetRedisOptions()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(configuration.RedisConnectionString);
            options.SyncTimeout = 10 * 1000;
            return options;
        }

        private AppSettings configuration;

        public RedisTool(IOptions<AppSettings> configuration)
        {
            this.configuration = configuration.Value;
        }


        public string ReadCache(string key, int dbName)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    var data = db.KeyExists(key) == true ? db.StringGet(key).ToString() : null;
                    redis.Close();
                    return data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                LogHelper.Error("ReadCache", ex, key);
                return null;
            }
        }

        public void WriteObject(string key, Object value, int dbName, int minutes = 60)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    db.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(value), new System.TimeSpan(0, minutes, 0));
                    redis.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteObject", ex, key);
                Console.WriteLine(ex.ToString());
            }

        }

        public T ReadCache<T>(string key, int dbName)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    var data = db.KeyExists(key) == true ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(db.StringGet(key).ToString()) : default(T);
                    redis.Close();
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("ReadCache", ex, key);
                Console.WriteLine(ex.ToString());
                return default(T);
            }
        }




        public bool DeleteCache(string key, int dbName = 0)
        {
            try
            {
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(GetRedisOptions()))
                {
                    IDatabase db = redis.GetDatabase(dbName);
                    var result = db.KeyDelete(key);
                    redis.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("DeleteCache", ex, key);
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}