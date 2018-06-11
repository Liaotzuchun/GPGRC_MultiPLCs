using System.Collections.Generic;
using System.Linq;

namespace GPGO_MultiPLCs.Helpers
{
    public class TwoKeyDictionary<TKey1, TKey2, TValue>
    {
        public delegate void Key1Updated(TKey1 name, TValue value);

        public Dictionary<TKey1, TValue> Key1Dictionary = new Dictionary<TKey1, TValue>();

        public Dictionary<TKey2, TKey1> Key2Dictionary = new Dictionary<TKey2, TKey1>();

        public TValue this[TKey1 idx]
        {
            get => Key1Dictionary[idx];
            set
            {
                Key1Dictionary[idx] = value;
                Key1UpdatedEvent?.Invoke(idx, value);
            }
        }

        public TValue this[TKey2 idx]
        {
            get => Key1Dictionary[Key2Dictionary[idx]];
            set
            {
                var key = Key2Dictionary[idx];
                Key1Dictionary[key] = value;
                Key1UpdatedEvent?.Invoke(key, value);
            }
        }

        public event Key1Updated Key1UpdatedEvent;

        public void Add(TKey1 Key1, TKey2 Key2, TValue value)
        {
            Key1Dictionary.Add(Key1, value);
            Key2Dictionary.Add(Key2, Key1);
        }

        public IEnumerable<KeyValuePair<TKey1, TValue>> GetKeyValuePairsOfKey1()
        {
            return Key1Dictionary.ToList();
        }

        public IEnumerable<KeyValuePair<TKey2, TValue>> GetKeyValuePairsOfKey2()
        {
            return Key2Dictionary.Select(x => new KeyValuePair<TKey2, TValue>(x.Key, Key1Dictionary[x.Value]));
        }

        public IEnumerable<TValue> GetValues(IEnumerable<TKey1> keys)
        {
            return keys.Select(key => Key1Dictionary[key]);
        }

        public IEnumerable<TValue> GetValues(IEnumerable<TKey2> keys)
        {
            return keys.Select(key => Key1Dictionary[Key2Dictionary[key]]);
        }

        public bool Remove(TKey1 Key1)
        {
            if (!Key2Dictionary.Any(f => f.Value.Equals(Key1)))
            {
                return false;
            }

            var Key2ToDelete = Key2Dictionary.First(f => f.Value.Equals(Key1));

            Key2Dictionary.Remove(Key2ToDelete.Key);
            Key1Dictionary.Remove(Key1);

            return true;
        }

        public bool Remove(TKey2 Key2)
        {
            if (!Key2Dictionary.ContainsKey(Key2))
            {
                return false;
            }

            var Key1 = Key2Dictionary[Key2];
            Key2Dictionary.Remove(Key2);
            Key1Dictionary.Remove(Key1);

            return true;
        }
    }
}