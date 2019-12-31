using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HouseMap.Common;
using HouseMap.Dao;
using HouseMap.Dao.DBEntity;
using HouseMapConsumer.Dao.DBEntity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace HouseMapConsumer.Dao
{
    public class HouseMongoMapper : MongoDBMapper
    {
        private readonly ILogger<HouseMongoMapper> _logger;

        public HouseMongoMapper(IOptions<AppSettings> configuration, ILogger<HouseMongoMapper> logger) : base(configuration)
        {
            _logger = logger;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public virtual async Task<bool> WriteRecordAsync(MongoHouseEntity houseEntity)
        {
            if (null == houseEntity || string.IsNullOrEmpty(houseEntity.Id) || string.IsNullOrEmpty(houseEntity.City))
            {
                _logger.LogInformation($"house没有ID，跳过。");
                return false;
            }
            //var dateValue = houseEntity.PubTime.ToString("yyyyMM");
            var collectionName = $"{ConstMongoTables.HouseData}_{houseEntity.City}";
            await InitCollectionAsync(collectionName);
            var containsRecord = await ContainsRecordAsync(houseEntity);
            if (containsRecord)
            {
                _logger.LogInformation($"house:{houseEntity.Title} 已存在，直接跳过。");
                return true;
            }
            await _client.GetDatabase(this._mongoDBName).GetCollection<MongoHouseEntity>(collectionName).InsertOneAsync(houseEntity);
            _logger.LogInformation($"write collectionName:{collectionName}, house:{houseEntity.Title} success!");
            return true;
        }

        public bool WriteRecord(MongoHouseEntity houseEntity)
        {
            if (string.IsNullOrEmpty(houseEntity?.Id)
            || string.IsNullOrEmpty(houseEntity?.City)
            || string.IsNullOrEmpty(houseEntity?.OnlineURL))
            {
                Console.WriteLine($"houseEntity is empty, skip it.");
                return false;
            }

            var collectionName = $"{ConstMongoTables.HouseData}_{houseEntity.City}";
            InitCollection(collectionName);
            if (ContainsRecord(houseEntity))
            {
                Console.WriteLine($"house:{houseEntity.Title}-{houseEntity.OnlineURL} exits, skip it.");
            }
            var database = _client.GetDatabase(this._mongoDBName);
            var query = Builders<MongoHouseEntity>.Filter.Eq(x => x.Id, houseEntity.Id);
            var update = Builders<MongoHouseEntity>.Update.Set(x => x.Latitude, houseEntity.Latitude)
                                                         .Set(x => x.Longitude, houseEntity.Longitude)
                                                         .Set(x => x.Id, houseEntity.Id)
                                                         .Set(x => x.City, houseEntity.City)
                                                         .Set(x => x.CreateTime, houseEntity.CreateTime)
                                                         .Set(x => x.Labels, houseEntity.Labels)
                                                         .Set(x => x.Location, houseEntity.Location)
                                                         .Set(x => x.OnlineURL, houseEntity.OnlineURL)
                                                         .Set(x => x.PicURLs, houseEntity.PicURLs)
                                                         .Set(x => x.Price, houseEntity.Price)
                                                         .Set(x => x.PubTime, houseEntity.PubTime)
                                                         .Set(x => x.RentType, houseEntity.RentType)
                                                         .Set(x => x.Source, houseEntity.Source)
                                                         .Set(x => x.Tags, houseEntity.Tags)
                                                          .Set(x => x.Text, houseEntity.Text)
                                                         .Set(x => x.Title, houseEntity.Title)
                                                         .Set(x => x.Timestamp, houseEntity.Timestamp)
                                                         .Set(x => x.UpdateTime, DateTime.Now);
            var options = new FindOneAndUpdateOptions<MongoHouseEntity> { ReturnDocument = ReturnDocument.After, IsUpsert = true };
            var result = database.GetCollection<MongoHouseEntity>(collectionName).FindOneAndUpdate(query, update, options);
            Console.WriteLine($"save {houseEntity.Id} to {collectionName} mongodb finish, result:{result != null}");
            return result != null;
        }





        public bool UpdateHousesLngLat(HousesLatLng dbHouse)
        {
            if (string.IsNullOrEmpty(dbHouse?.latitude)
            || string.IsNullOrEmpty(dbHouse?.longitude)
            || string.IsNullOrEmpty(dbHouse.city))
            {
                _logger.LogInformation($"dbHouse Latitude/Longitude is empty, skip it.");
                return false;
            }
            var collectionName = $"{ConstMongoTables.HouseData}_{dbHouse.city}";
            var database = _client.GetDatabase(this._mongoDBName);
            var query = Builders<MongoHouseEntity>.Filter.Eq(x => x.Id, dbHouse.id);
            var latitude = double.Parse(dbHouse.latitude);
            var longitude = double.Parse(dbHouse.longitude);
            var update = Builders<MongoHouseEntity>.Update.Set(x => x.Latitude, latitude)
                                                         .Set(x => x.UpdateTime, DateTime.Now)
                                                         .Set(x => x.Longitude, longitude);
            var options = new FindOneAndUpdateOptions<MongoHouseEntity> { ReturnDocument = ReturnDocument.After };
            var result = database.GetCollection<MongoHouseEntity>(collectionName).FindOneAndUpdate(query, update, options);
            _logger.LogInformation($"save {dbHouse.id} Latitude/Longitude to mongodb finish, result:{result != null}");
            return result != null;
        }

        /// <summary>
        /// 是否包含此记录
        /// </summary>
        public virtual async Task<bool> ContainsRecordAsync(MongoHouseEntity houseEntity)
        {
            var db = _client.GetDatabase(this._mongoDBName);
            var collectionName = $"{ConstMongoTables.HouseData}_{houseEntity.City}";
            return await db.GetCollection<MongoHouseEntity>(collectionName).AsQueryable()
                         .AnyAsync(x => x.OnlineURL == houseEntity.OnlineURL);
        }

        /// <summary>
        /// 是否包含此记录
        /// </summary>
        public virtual bool ContainsRecord(MongoHouseEntity houseEntity)
        {
            var db = _client.GetDatabase(this._mongoDBName);
            var collectionName = $"{ConstMongoTables.HouseData}_{houseEntity.City}";
            return db.GetCollection<MongoHouseEntity>(collectionName).AsQueryable().Any(x => x.OnlineURL == houseEntity.OnlineURL);
        }

        /// <summary>
        /// 查询记录
        /// </summary>
        public virtual async Task<List<MongoHouseEntity>> QueryHouses(MongoHouseQuery houseQuery)
        {
            var db = _client.GetDatabase(this._mongoDBName);
            var collectionName = $"{ConstMongoTables.HouseData}_{houseQuery.city}";
            var query = db.GetCollection<MongoHouseEntity>(collectionName).AsQueryable();
            if (!string.IsNullOrEmpty(houseQuery.source))
            {
                query = query.Where(h => h.Source == houseQuery.source);
            }
            if (houseQuery.rentType.HasValue)
            {
                query = query.Where(h => h.RentType == houseQuery.rentType);
            }
            if (!string.IsNullOrEmpty(houseQuery.source))
            {
                query = query.Where(h => h.Source == houseQuery.source);
            }
            var position = default(PositionModel);
            if (houseQuery.latitude != null && houseQuery.longitude != null)
            {
                position = DistanceHelper.FindNeighPosition(houseQuery.longitude.Value, houseQuery.latitude.Value, houseQuery.distance);
                query = query.Where(item => item.Latitude.HasValue && item.Longitude.HasValue
                                            && item.Longitude.Value >= position.MinLng
                                            && item.Longitude.Value <= position.MaxLng
                                            && item.Latitude.Value >= position.MinLat
                                            && item.Latitude.Value <= position.MaxLat);
            }
            // var text = query.ToString();
            var results = await query.OrderByDescending(h => h.Timestamp)
                               .Skip(houseQuery.page * houseQuery.pageSize)
                               .Take(houseQuery.pageSize).ToListAsync();

            return results;
        }



        private async Task InitCollectionAsync(string collectionName)
        {
            var db = _client.GetDatabase(this._mongoDBName);
            var filter = new BsonDocument("name", collectionName);
            var collections = await db.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            if (!collections.Any())
            {
                await db.CreateCollectionAsync(collectionName);
                var builder = Builders<MongoHouseEntity>.IndexKeys;
                var indexModel = new CreateIndexModel<MongoHouseEntity>(
                                    builder
                                    .Ascending(x => x.Id)
                                    .Ascending(x => x.Latitude)
                                    .Ascending(x => x.Longitude)
                                    .Ascending(x => x.Price)
                                    .Ascending(x => x.Source)
                                    .Ascending(x => x.RentType)
                                    .Ascending(x => x.Timestamp));
                var uniqueUserIndex = new CreateIndexModel<MongoHouseEntity>(builder.Ascending(x => x.OnlineURL), new CreateIndexOptions() { Unique = true });
                await db.GetCollection<MongoHouseEntity>(collectionName).Indexes.CreateManyAsync(new List<CreateIndexModel<MongoHouseEntity>>()
                {
                     uniqueUserIndex, indexModel
                });
            }
        }

        private void InitCollection(string collectionName)
        {
            var db = _client.GetDatabase(this._mongoDBName);
            var filter = new BsonDocument("name", collectionName);
            var collections = db.ListCollections(new ListCollectionsOptions { Filter = filter });
            if (!collections.Any())
            {
                db.CreateCollection(collectionName);
                var builder = Builders<MongoHouseEntity>.IndexKeys;
                var indexModel = new CreateIndexModel<MongoHouseEntity>(
                                    builder
                                    .Ascending(x => x.Id)
                                    .Ascending(x => x.Latitude)
                                    .Ascending(x => x.Longitude)
                                    .Ascending(x => x.Price)
                                    .Ascending(x => x.Source)
                                    .Ascending(x => x.RentType)
                                    .Ascending(x => x.Timestamp));
                var uniqueUserIndex = new CreateIndexModel<MongoHouseEntity>(builder.Ascending(x => x.OnlineURL), new CreateIndexOptions() { Unique = true });
                db.GetCollection<MongoHouseEntity>(collectionName).Indexes.CreateMany(new List<CreateIndexModel<MongoHouseEntity>>()
                {
                     uniqueUserIndex, indexModel
                });
            }
        }
    }

}
