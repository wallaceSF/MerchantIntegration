using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MerchantIntegration.Core.Entity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
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
//            BsonClassMap.RegisterClassMap<Customer>(config => {
//                config.AutoMap();
//                config.IdMemberMap
//                    .SetIdGenerator(StringObjectIdGenerator.Instance)
//                    .SetSerializer(new StringSerializer(BsonType.ObjectId))
//                    .SetIgnoreIfDefault(true);
//            });

          //  var z = _collection.Find<T>(t => true).ToJson();
          //  var zz = _collection.Find<T>(t => true).ToBsonDocument().;
         //   var zhz = _collection.Find<T>(t => true).ToList();
            
            return _collection.Find<T>(t => true).ToList();
        }
    }
}