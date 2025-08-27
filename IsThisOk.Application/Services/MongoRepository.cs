using IsThisOk.Application.Interfaces;
using IsThisOk.Application.Extensions;
using MongoDB.Driver;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace IsThisOk.Application.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<T>(); 
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> UpdateAsync(string id, T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = await _collection.ReplaceOneAsync(filter, entity);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}