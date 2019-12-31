using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseMap.Common;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HouseMapConsumer.Dao
{
    public class MongoDBMapper
    {


        protected readonly string _mongoDBName;

        protected readonly AppSettings _configuration;


        public MongoDBMapper(IOptions<AppSettings> configuration)
        {
            this._configuration = configuration.Value;
            this._mongoDBName = configuration.Value.MongoDBName;
        }

        public async Task<IClientSession> StartSessionAsync()
        {
            return await _client.StartSessionAsync();
        }

        protected MongoClient _client => new MongoClient(_configuration.MongoDBConnectionString);

        /// <summary>
        /// 初始化数据表
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="indexer"></param>
        /// <typeparam name="T"></typeparam>
        protected async Task InitializeCollectionAsync<T>(string collectionName, IEnumerable<CreateIndexModel<T>> indexer)
            where T : class
        {
            var db = _client.GetDatabase(this._mongoDBName);
            var filter = new BsonDocument("name", collectionName);
            var collections = await db.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            if (!collections.Any())
            {
                await db.CreateCollectionAsync(collectionName);
                if (indexer != null && indexer.Any())
                    await db.GetCollection<T>(collectionName).Indexes.CreateManyAsync(indexer);
            }
        }
    }
}
