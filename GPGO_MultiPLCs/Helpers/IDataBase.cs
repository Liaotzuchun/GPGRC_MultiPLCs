using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GPGO_MultiPLCs.Helpers
{
    public interface IDataBase<T> where T : class, new()
    {
        bool Delete(Func<T, bool> condition);

        Task<bool> DeleteAsync(Func<T, bool> condition);

        bool DeleteOne(Func<T, bool> condition);

        Task<bool> DeleteOneAsync(Func<T, bool> condition);

        ICollection<T> Find(Func<T, bool> condition);

        Task<ICollection<T>> FindAsync(Func<T, bool> condition);

        T FindOne(Func<T, bool> condition);

        Task<T> FindOneAsync(Func<T, bool> condition);

        bool UpdateOne(Func<T, bool> condition, string propertyName, object value);

        Task<bool> UpdateOneAsync(Func<T, bool> condition, string propertyName, object value);

        bool Upsert(Func<T, bool> condition, T data);

        Task<bool> UpsertAsync(Func<T, bool> condition, T data);
    }

    public class MongoBase<T> : IDataBase<T> where T : class, new()
    {
        public bool Delete(Func<T, bool> condition)
        {
            return MongoCollection.DeleteMany(x => condition(x)).IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(Func<T, bool> condition)
        {
            return (await MongoCollection.DeleteManyAsync(x => condition(x))).IsAcknowledged;
        }

        public bool DeleteOne(Func<T, bool> condition)
        {
            return MongoCollection.DeleteOne(x => condition(x)).IsAcknowledged;
        }

        public async Task<bool> DeleteOneAsync(Func<T, bool> condition)
        {
            return (await MongoCollection.DeleteOneAsync(x => condition(x))).IsAcknowledged;
        }

        public ICollection<T> Find(Func<T, bool> condition)
        {
            return MongoCollection.Find(x => condition(x)).ToList();
        }

        public async Task<ICollection<T>> FindAsync(Func<T, bool> condition)
        {
            return (await MongoCollection.FindAsync(x => condition(x))).ToList();
        }

        public T FindOne(Func<T, bool> condition)
        {
            return MongoCollection.Find(x => condition(x)).FirstOrDefault();
        }

        public async Task<T> FindOneAsync(Func<T, bool> condition)
        {
            return (await MongoCollection.FindAsync(x => condition(x))).FirstOrDefault();
        }

        public bool UpdateOne(Func<T, bool> condition, string propertyName, object value)
        {
            return MongoCollection.UpdateOne(x => condition(x), Builders<T>.Update.Set(propertyName, value)).IsAcknowledged;
        }

        public async Task<bool> UpdateOneAsync(Func<T, bool> condition, string propertyName, object value)
        {
            return (await MongoCollection.UpdateOneAsync(x => condition(x), Builders<T>.Update.Set(propertyName, value))).IsAcknowledged;
        }

        public bool Upsert(Func<T, bool> condition, T data)
        {
            return MongoCollection.ReplaceOne(x => condition(x), data, new UpdateOptions { IsUpsert = true }).IsAcknowledged;
        }

        public async Task<bool> UpsertAsync(Func<T, bool> condition, T data)
        {
            return (await MongoCollection.ReplaceOneAsync(x => condition(x), data, new UpdateOptions { IsUpsert = true })).IsAcknowledged;
        }

        private readonly IMongoCollection<T> MongoCollection;

        public MongoBase(IMongoCollection<T> collection)
        {
            MongoCollection = collection;
        }
    }
}