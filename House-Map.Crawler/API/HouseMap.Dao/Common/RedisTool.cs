using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace HouseMap.Common
{
    public class RedisTool
    {
        private readonly ConnectionMultiplexer _redisMultiplexer;


        public RedisTool(ConnectionMultiplexer redisMultiplexer)
        {
            _redisMultiplexer = redisMultiplexer;
        }


        public string ReadCache(string key, int dbName)
        {
            try
            {

                IDatabase db = _redisMultiplexer.GetDatabase(dbName);
                var data = db.KeyExists(key) == true ? db.StringGet(key).ToString() : null;
                return data;

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
                IDatabase db = _redisMultiplexer.GetDatabase(dbName);
                db.StringSet(key, Newtonsoft.Json.JsonConvert.SerializeObject(value), new System.TimeSpan(0, minutes, 0));

            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteObject", ex, key);
                Console.WriteLine(ex.ToString());
            }

        }


        public void WriteObject(KeyConfig keyConfig, Object value)
        {
            try
            {
                IDatabase db = _redisMultiplexer.GetDatabase(keyConfig.DBName);
                db.StringSet(keyConfig.Key, Newtonsoft.Json.JsonConvert.SerializeObject(value), keyConfig.ExpireTime);

            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteObject", ex, keyConfig);
                Console.WriteLine(ex.ToString());
            }

        }

        public virtual void WriteHash(KeyConfig keyConfig, string hashKey, string value)
        {
            try
            {
                IDatabase db = _redisMultiplexer.GetDatabase(keyConfig.DBName);
                db.HashSet(keyConfig.Key, hashKey, value);

            }
            catch (Exception ex)
            {
                LogHelper.Error("WriteHash", ex, keyConfig);
            }
        }

        public virtual Dictionary<RedisValue, RedisValue> ReadHash(KeyConfig keyConfig)
        {
            try
            {
                IDatabase db = _redisMultiplexer.GetDatabase(keyConfig.DBName);
                return db.HashGetAll(keyConfig.Key).ToDictionary();

            }
            catch (Exception ex)
            {
                LogHelper.Error("ReadHash", ex, keyConfig);
                return default(Dictionary<RedisValue, RedisValue>);
            }
        }

        public T ReadCache<T>(string key, int dbName)
        {
            try
            {

                IDatabase db = _redisMultiplexer.GetDatabase(dbName);
                var data = db.KeyExists(key) == true ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(db.StringGet(key).ToString()) : default(T);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.Error("ReadCache", ex, key);
                Console.WriteLine(ex.ToString());
                return default(T);
            }
        }


         public T ReadCache<T>(KeyConfig keyConfig)
        {
            try
            {
                IDatabase db = _redisMultiplexer.GetDatabase(keyConfig.DBName);
                var data = db.KeyExists(keyConfig.Key) == true ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(db.StringGet(keyConfig.Key).ToString()) : default(T);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.Error("ReadCache", ex, keyConfig);
                Console.WriteLine(ex.ToString());
                return default(T);
            }
        }




        public bool DeleteCache(string key, int dbName = 0)
        {
            try
            {
                IDatabase db = _redisMultiplexer.GetDatabase(dbName);
                var result = db.KeyDelete(key);
                return result;

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