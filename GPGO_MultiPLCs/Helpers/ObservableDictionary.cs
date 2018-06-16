using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace GPGO_MultiPLCs.Helpers
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                var exist = TryGetValue(key, out var oldValue);
                var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
                base[key] = value;
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                if (exist)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, Keys.ToList().IndexOf(key)));
                }
                else
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, Keys.ToList().IndexOf(key)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }

        public new void Add(TKey key, TValue value)
        {
            if (TryGetValue(key, out var oldValue))
            {
                var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
                base[key] = value;
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, Keys.ToList().IndexOf(key)));
            }
            else
            {
                var item = new KeyValuePair<TKey, TValue>(key, value);
                base.Add(key, value);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, Keys.ToList().IndexOf(key)));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        }

        public new void Clear()
        {
            base.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        }

        public new bool Remove(TKey key)
        {
            if (!TryGetValue(key, out _))
            {
                return false;
            }

            var item = new KeyValuePair<TKey, TValue>(key, base[key]);
            var index = Keys.ToList().IndexOf(key);
            var result = base.Remove(key);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            return result;
        }

        public ObservableDictionary()
        {
        }

        public ObservableDictionary(int capacity) : base(capacity)
        {
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        {
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        {
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {
        }
    }
}