using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GPGO_MultiPLCs.Helpers
{
    public interface IDataBase<T> where T : class, new()
    {
        bool Add(T data);

        Task<bool> AddAsync(T data);

        bool AddMany(ICollection<T> datas);

        Task<bool> AddManyAsync(ICollection<T> data);

        bool Delete(Expression<Func<T, bool>> condition);

        Task<bool> DeleteAsync(Expression<Func<T, bool>> condition);

        bool DeleteOne(Expression<Func<T, bool>> condition);

        Task<bool> DeleteOneAsync(Expression<Func<T, bool>> condition);

        List<T> Find(Expression<Func<T, bool>> condition);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> condition);

        T FindOne(Expression<Func<T, bool>> condition);

        Task<T> FindOneAsync(Expression<Func<T, bool>> condition);

        bool UpdateOne(Expression<Func<T, bool>> condition, string propertyName, object value);

        Task<bool> UpdateOneAsync(Expression<Func<T, bool>> condition, string propertyName, object value);

        bool Upsert(Expression<Func<T, bool>> condition, T data);

        Task<bool> UpsertAsync(Expression<Func<T, bool>> condition, T data);
    }

    public class MongoBase<T> : IDataBase<T> where T : class, new()
    {
        public bool Add(T data)
        {
            MongoCollection.InsertOne(data);

            return true;
        }

        public async Task<bool> AddAsync(T data)
        {
            await MongoCollection.InsertOneAsync(data);

            return true;
        }

        public bool AddMany(ICollection<T> datas)
        {
            MongoCollection.InsertMany(datas);

            return true;
        }

        public async Task<bool> AddManyAsync(ICollection<T> datas)
        {
            await MongoCollection.InsertManyAsync(datas);

            return true;
        }

        public bool Delete(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.DeleteMany(condition).IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(Expression<Func<T, bool>> condition)
        {
            return (await MongoCollection.DeleteManyAsync(condition)).IsAcknowledged;
        }

        public bool DeleteOne(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.DeleteOne(condition).IsAcknowledged;
        }

        public async Task<bool> DeleteOneAsync(Expression<Func<T, bool>> condition)
        {
            return (await MongoCollection.DeleteOneAsync(condition)).IsAcknowledged;
        }

        public List<T> Find(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.Find(condition).ToList();
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> condition)
        {
            return await (await MongoCollection.FindAsync(condition)).ToListAsync();
        }

        public T FindOne(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.Find(condition).FirstOrDefault();
        }

        public async Task<T> FindOneAsync(Expression<Func<T, bool>> condition)
        {
            return await (await MongoCollection.FindAsync(condition)).FirstOrDefaultAsync();
        }

        public bool UpdateOne(Expression<Func<T, bool>> condition, string propertyName, object value)
        {
            return MongoCollection.UpdateOne(condition, Builders<T>.Update.Set(propertyName, value)).IsAcknowledged;
        }

        public async Task<bool> UpdateOneAsync(Expression<Func<T, bool>> condition, string propertyName, object value)
        {
            return (await MongoCollection.UpdateOneAsync(condition, Builders<T>.Update.Set(propertyName, value))).IsAcknowledged;
        }

        public bool Upsert(Expression<Func<T, bool>> condition, T data)
        {
            return MongoCollection.ReplaceOne(condition, data, new UpdateOptions { IsUpsert = true }).IsAcknowledged;
        }

        public async Task<bool> UpsertAsync(Expression<Func<T, bool>> condition, T data)
        {
            return (await MongoCollection.ReplaceOneAsync(condition, data, new UpdateOptions { IsUpsert = true })).IsAcknowledged;
        }

        private readonly IMongoCollection<T> MongoCollection;

        public MongoBase(IMongoCollection<T> collection)
        {
            MongoCollection = collection;
        }
    }
}