using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace MerchantIntegration.Infra.Repository
{
    public abstract class BaseRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        protected string CollectionName { get; set; }

        protected BaseRepository(IMongoDatabase mongoDatabase)
        {
            if (string.IsNullOrEmpty(this.CollectionName))
            {
                var collectionNameType = typeof(T).Name;
                this.CollectionName = $"{collectionNameType}s";
            }

            this._collection = mongoDatabase.GetCollection<T>(this.CollectionName);
        }

        protected T Create(T objectPersist)
        {
            this._collection.InsertOne(objectPersist);
            return (T) Convert.ChangeType(objectPersist, typeof(T));
        }

        protected T Find(FilterDefinition<T> expression)
        {
            var objectGeneric = this._collection.Find<T>(expression).FirstOrDefault();
            return (T) Convert.ChangeType(objectGeneric, typeof(T));
        }

        protected List<T> FindAll()
        {
            return this._collection.Find<T>(t => true).ToList();
        }
    }
}