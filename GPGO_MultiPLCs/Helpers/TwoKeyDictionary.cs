using System;
using System.Collections.Generic;
using System.Linq;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>提供兩個key當索引的Dictionary集合</summary>
    /// <typeparam name="TKey1">key1</typeparam>
    /// <typeparam name="TKey2">key2</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public sealed class TwoKeyDictionary<TKey1, TKey2, TValue>
    {
        private readonly Dictionary<TKey1, TValue> Key1Dictionary = new Dictionary<TKey1, TValue>();
        private readonly Dictionary<TKey1, TKey2> Key1ToKey2Dictionary = new Dictionary<TKey1, TKey2>();
        private readonly Dictionary<TKey2, TKey1> Key2ToKey1Dictionary = new Dictionary<TKey2, TKey1>();

        public TValue this[TKey1 key1]
        {
            get => Key1Dictionary[key1];
            set
            {
                var key2 = Key1ToKey2Dictionary[key1];
                Key1Dictionary[key1] = value;
                UpdatedEvent?.Invoke(key1, key2, value);
            }
        }

        public TValue this[TKey2 key2]
        {
            get => Key1Dictionary[Key2ToKey1Dictionary[key2]];
            set
            {
                var key1 = Key2ToKey1Dictionary[key2];
                Key1Dictionary[key1] = value;
                UpdatedEvent?.Invoke(key1, key2, value);
            }
        }

        public event Action<TKey1, TKey2, TValue> UpdatedEvent;

        public void Add(TKey1 Key1, TKey2 Key2, TValue value)
        {
            Key1Dictionary.Add(Key1, value);
            Key2ToKey1Dictionary.Add(Key2, Key1);
            Key1ToKey2Dictionary.Add(Key1, Key2);
        }

        public void Clear()
        {
            Key1Dictionary.Clear();
            Key1ToKey2Dictionary.Clear();
            Key2ToKey1Dictionary.Clear();
        }

        public TKey1 GetKey1(TKey2 key2)
        {
            return Key2ToKey1Dictionary[key2];
        }

        public TKey2 GetKey2(TKey1 key1)
        {
            return Key1ToKey2Dictionary[key1];
        }

        public IEnumerable<KeyValuePair<TKey1, TValue>> GetKeyValuePairsOfKey1()
        {
            return Key1Dictionary.ToList();
        }

        public IEnumerable<KeyValuePair<TKey2, TValue>> GetKeyValuePairsOfKey2()
        {
            return Key2ToKey1Dictionary.Select(x => new KeyValuePair<TKey2, TValue>(x.Key, Key1Dictionary[x.Value]));
        }

        public IEnumerable<TValue> GetValues(IEnumerable<TKey1> keys)
        {
            return keys.Select(key => Key1Dictionary[key]);
        }

        public IEnumerable<TValue> GetValues(IEnumerable<TKey2> keys)
        {
            return keys.Select(key => Key1Dictionary[Key2ToKey1Dictionary[key]]);
        }

        public bool Remove(TKey1 Key1)
        {
            if (!Key2ToKey1Dictionary.Any(f => f.Value.Equals(Key1)))
            {
                return false;
            }

            var Key2ToDelete = Key2ToKey1Dictionary.First(f => f.Value.Equals(Key1));
            Key1ToKey2Dictionary.Remove(Key1);
            Key2ToKey1Dictionary.Remove(Key2ToDelete.Key);
            Key1Dictionary.Remove(Key1);

            return true;
        }

        public bool Remove(TKey2 Key2)
        {
            if (!Key2ToKey1Dictionary.ContainsKey(Key2))
            {
                return false;
            }

            var Key1 = Key2ToKey1Dictionary[Key2];
            Key1ToKey2Dictionary.Remove(Key1);
            Key2ToKey1Dictionary.Remove(Key2);
            Key1Dictionary.Remove(Key1);

            return true;
        }
    }
}