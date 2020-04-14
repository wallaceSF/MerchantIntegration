using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MerchantIntegration.Infra.Repository
{
    public abstract class BaseRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        protected string CollectionName;

        protected BaseRepository(IMongoDatabase mongoDatabase)
        {
            if (string.IsNullOrEmpty(CollectionName))
            {
                var collectionNameType = typeof(T).Name;
                CollectionName = $"{collectionNameType}s";
            }

            _collection = mongoDatabase.GetCollection<T>(CollectionName);
        }

        protected T Create(T objectPersist)
        {
            _collection.InsertOne(objectPersist);
            return (T) Convert.ChangeType(objectPersist, typeof(T));
        }
    }
}