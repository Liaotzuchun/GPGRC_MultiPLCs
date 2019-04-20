using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GPGO_MultiPLCs.Helpers
{
    public class MongoBase<T> : IDataBase<T> where T : class, new()
    {
        public bool Add(T data)
        {
            MongoCollection.InsertOne(data);

            return true;
        }

        public async ValueTask<bool> AddAsync(T data)
        {
            await MongoCollection.InsertOneAsync(data);

            return true;
        }

        public bool AddMany(ICollection<T> datas)
        {
            MongoCollection.InsertMany(datas);

            return true;
        }

        public async ValueTask<bool> AddManyAsync(ICollection<T> datas)
        {
            await MongoCollection.InsertManyAsync(datas);

            return true;
        }

        public bool Delete(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.DeleteMany(condition).IsAcknowledged;
        }

        public async ValueTask<bool> DeleteAsync(Expression<Func<T, bool>> condition)
        {
            return (await MongoCollection.DeleteManyAsync(condition)).IsAcknowledged;
        }

        public bool DeleteOne(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.DeleteOne(condition).IsAcknowledged;
        }

        public async ValueTask<bool> DeleteOneAsync(Expression<Func<T, bool>> condition)
        {
            return (await MongoCollection.DeleteOneAsync(condition)).IsAcknowledged;
        }

        public List<T> Find(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.Find(condition).ToList();
        }

        public async ValueTask<List<T>> FindAsync(Expression<Func<T, bool>> condition)
        {
            return await (await MongoCollection.FindAsync(condition)).ToListAsync();
        }

        public T FindOne(Expression<Func<T, bool>> condition)
        {
            return MongoCollection.Find(condition).FirstOrDefault();
        }

        public async ValueTask<T> FindOneAsync(Expression<Func<T, bool>> condition)
        {
            return await (await MongoCollection.FindAsync(condition)).FirstOrDefaultAsync();
        }

        public bool UpdateOne(Expression<Func<T, bool>> condition, string propertyName, object value)
        {
            return MongoCollection.UpdateOne(condition, Builders<T>.Update.Set(propertyName, value)).IsAcknowledged;
        }

        public async ValueTask<bool> UpdateOneAsync(Expression<Func<T, bool>> condition, string propertyName, object value)
        {
            return (await MongoCollection.UpdateOneAsync(condition, Builders<T>.Update.Set(propertyName, value))).IsAcknowledged;
        }

        public bool UpdateMany(Expression<Func<T, bool>> condition, string propertyName, object value)
        {
            return MongoCollection.UpdateMany(condition, Builders<T>.Update.Set(propertyName, value)).IsAcknowledged;
        }

        public async ValueTask<bool> UpdateManyAsync(Expression<Func<T, bool>> condition, string propertyName, object value)
        {
            return (await MongoCollection.UpdateManyAsync(condition, Builders<T>.Update.Set(propertyName, value))).IsAcknowledged;
        }

        public bool Upsert(Expression<Func<T, bool>> condition, T data)
        {
            return MongoCollection.ReplaceOne(condition, data, new UpdateOptions { IsUpsert = true }).IsAcknowledged;
        }

        public async ValueTask<bool> UpsertAsync(Expression<Func<T, bool>> condition, T data)
        {
            return (await MongoCollection.ReplaceOneAsync(condition, data, new UpdateOptions { IsUpsert = true })).IsAcknowledged;
        }

        private readonly IMongoCollection<T> MongoCollection;

        /// <summary>創建MongoDB資料庫集合檢視</summary>
        /// <param name="collection">MongoDB資料集合</param>
        public MongoBase(IMongoCollection<T> collection)
        {
            MongoCollection = collection;
        }
    }
}
