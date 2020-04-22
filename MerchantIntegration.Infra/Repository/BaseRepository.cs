using System;
using System.Collections.Generic;
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

        protected T Find(FilterDefinition<T> expression)
        {
            var objectGeneric = _collection.Find<T>(expression).FirstOrDefault();
            return (T) Convert.ChangeType(objectGeneric, typeof(T));
        }

        protected List<T> FindAll()
        {
            return _collection.Find<T>(t => true).ToList();
        }
    }
}