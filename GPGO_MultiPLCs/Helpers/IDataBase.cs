using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GPGO_MultiPLCs.Helpers
{
    //!因MongoDB Driver的支援問題，所以條件式皆用Func<T, bool>而非Predicate<T>
    //!Expression<Func<T, bool>>是運算式資料結構，等於是將委派包裝成物件(因此理論上可序列化)，
    //!此處是為將委派傳遞給MongoDB Driver讓它去實作動態查詢(因為它無法預知你要執行什麼查詢)
    /// <summary>資料庫基本介面</summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataBase<T> where T : new()
    {
        /// <summary>新增資料</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Add(T data);

        /// <summary>(非同步)新增資料</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ValueTask<bool> AddAsync(T data);

        /// <summary>新增數筆資料</summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        bool AddMany(ICollection<T> datas);

        /// <summary>(非同步)新增數筆資料</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ValueTask<bool> AddManyAsync(ICollection<T> data);

        /// <summary>刪除符合條件之資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool Delete(Expression<Func<T, bool>> condition);

        /// <summary>(非同步)刪除符合條件之資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        ValueTask<bool> DeleteAsync(Expression<Func<T, bool>> condition);

        /// <summary>刪除符合條件之單一資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        bool DeleteOne(Expression<Func<T, bool>> condition);

        /// <summary>(非同步)刪除符合條件之單一資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        ValueTask<bool> DeleteOneAsync(Expression<Func<T, bool>> condition);

        /// <summary>尋找符合條件之所有資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<T> Find(Expression<Func<T, bool>> condition);

        /// <summary>(非同步)尋找符合條件之所有資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        ValueTask<List<T>> FindAsync(Expression<Func<T, bool>> condition);

        /// <summary>尋找符合條件之單一資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        T FindOne(Expression<Func<T, bool>> condition);

        /// <summary>(非同步)尋找符合條件之單一資料</summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        ValueTask<T> FindOneAsync(Expression<Func<T, bool>> condition);

        /// <summary>更新符合條件之單一資料的特定屬性</summary>
        /// <param name="condition"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool UpdateOne(Expression<Func<T, bool>> condition, string propertyName, object value);

        /// <summary>(非同步)更新符合條件之單一資料的特定屬性</summary>
        /// <param name="condition"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ValueTask<bool> UpdateOneAsync(Expression<Func<T, bool>> condition, string propertyName, object value);

        /// <summary>更新單一資料，若資料庫中本來沒有則會自動新增</summary>
        /// <param name="condition"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Upsert(Expression<Func<T, bool>> condition, T data);

        /// <summary>(非同步)更新單一資料，若資料庫中本來沒有則會自動新增</summary>
        /// <param name="condition"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ValueTask<bool> UpsertAsync(Expression<Func<T, bool>> condition, T data);
    }

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