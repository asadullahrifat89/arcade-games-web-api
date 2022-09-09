using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace AstroOdysseyCore
{
    public class MongoDbService : IMongoDbService
    {
        #region Fields

        private readonly string _connectionString;
        private readonly ILogger<MongoDbService> _logger;

        #endregion

        #region Ctor
        public MongoDbService(
           IConfiguration configuration,
           ILogger<MongoDbService> logger)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnectionString");
            _logger = logger;
        }

        #endregion

        #region Methods

        #region Common

        private IMongoCollection<T> GetCollection<T>()
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(Constants.DatabaseName);

            var collectionName = typeof(T).Name + "s";

            var collection = database.GetCollection<T>(collectionName);
            return collection;
        }

        #endregion

        #region Document

        public async Task<long> CountDocuments<T>(FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.Find(filter).CountDocumentsAsync();
            return result;
        }

        public async Task<bool> Exists<T>(FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result is not null;
        }

        public async Task<T> FindOne<T>(FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<T> FindOne<T>(Expression<Func<T, bool>> expression)
        {
            var collection = GetCollection<T>();
            var result = await collection.AsQueryable().Where(expression).FirstOrDefaultAsync();
            return result;
        }

        public async Task<T> FindOne<T>(FilterDefinition<T> filter, SortOrder sortOrder, string sortFieldName)
        {
            var collection = GetCollection<T>();

            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    {
                        var sort = Builders<T>.Sort.Ascending(sortFieldName);
                        var result = await collection.Find(filter).Sort(sort).FirstOrDefaultAsync();
                        return result;
                    }
                case SortOrder.Descending:
                    {
                        var sort = Builders<T>.Sort.Descending(sortFieldName);
                        var result = await collection.Find(filter).Sort(sort).FirstOrDefaultAsync();
                        return result;
                    }
                case SortOrder.None:
                default:
                    {
                        var result = await collection.Find(filter).FirstOrDefaultAsync();
                        return result;
                    }
            }
        }

        public async Task<List<T>> GetDocuments<T>(FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<List<T>> GetDocuments<T>(FilterDefinition<T> filter, int skip, int limit)
        {
            var collection = GetCollection<T>();
            var result = await collection.Find(filter).Skip(skip).Limit(limit).ToListAsync();
            return result;
        }

        public async Task<bool> InsertDocument<T>(T document)
        {
            var collection = GetCollection<T>();
            await collection.InsertOneAsync(document);
            return true;
        }

        public async Task<bool> InsertDocuments<T>(IEnumerable<T> documents)
        {
            var collection = GetCollection<T>();
            await collection.InsertManyAsync(documents);
            return true;
        }

        public async Task<T> ReplaceDocument<T>(T document, FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.FindOneAndReplaceAsync(filter: filter, replacement: document, options: new FindOneAndReplaceOptions<T>
            {
                ReturnDocument = ReturnDocument.After
            });
            return result;
        }

        public async Task<T> UpdateDocument<T>(UpdateDefinition<T> update, FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.FindOneAndUpdateAsync(filter: filter, update: update, options: new FindOneAndUpdateOptions<T>
            {
                ReturnDocument = ReturnDocument.After
            });
            return result;
        }

        public async Task<bool> UpdateDocuments<T>(UpdateDefinition<T> update, FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.UpdateManyAsync(filter: filter, update: update);
            return result.IsAcknowledged;
        }

        public async Task<bool> UpsertDocument<T>(T document, FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.ReplaceOneAsync(filter: filter, replacement: document, options: new ReplaceOptions() { IsUpsert = true });
            return result is not null && result.IsAcknowledged;
        }

        public async Task<T> DeleteDocument<T>(FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.FindOneAndDeleteAsync(filter: filter);
            return result;
        }

        public async Task<bool> DeleteDocuments<T>(FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>();
            var result = await collection.DeleteManyAsync(filter);
            return result is not null;
        }

        public async Task<bool> ExistsById<T>(int id)
        {
            var collection = GetCollection<T>();
            var filter = Builders<T>.Filter.Eq("Id", id);
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result is not null;
        }

        public async Task<T> FindById<T>(int id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var collection = GetCollection<T>();
            var result = await collection.Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<T> UpdateById<T>(UpdateDefinition<T> update, string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var collection = GetCollection<T>();
            var result = await collection.FindOneAndUpdateAsync(filter: filter, update: update, options: new FindOneAndUpdateOptions<T>
            {
                ReturnDocument = ReturnDocument.After
            });
            return result;
        }

        public async Task<T> ReplaceById<T>(T document, string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var collection = GetCollection<T>();
            var result = await collection.FindOneAndReplaceAsync(filter: filter, replacement: document, options: new FindOneAndReplaceOptions<T>
            {
                ReturnDocument = ReturnDocument.After
            });
            return result;
        }

        public async Task<bool> DeleteById<T>(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var collection = GetCollection<T>();
            var result = await collection.FindOneAndDeleteAsync(filter);
            return result is not null;
        }

        public async Task<bool> UpsertById<T>(T document, string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", id);
            var collection = GetCollection<T>();
            var result = await collection.ReplaceOneAsync(filter: filter, replacement: document, options: new ReplaceOptions() { IsUpsert = true });
            return result is not null && result.IsAcknowledged;
        }

        #endregion

        #endregion
    }

    public enum SortOrder
    {
        None,
        Ascending,
        Descending
    }
}
