using System.Reflection;
using IsThisOk.Domain.Attributes;
using MongoDB.Driver;

namespace IsThisOk.Application.Extensions
{
    public static class MongoExtensions
    {
        public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase database)
        {
            var collectionAttribute = typeof(T).GetCustomAttribute<BsonCollectionAttribute>();

            if (collectionAttribute != null)
            {
                return database.GetCollection<T>(collectionAttribute.CollectionName);
            }
            //var collectionName = typeof(T).Name.Replace("Mst", "s"); // UserMst -> Users
            var collectionName = typeof(T).Name;
            return database.GetCollection<T>(collectionName);
        }
    }
}