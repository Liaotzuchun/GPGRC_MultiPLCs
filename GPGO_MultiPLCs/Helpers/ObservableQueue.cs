using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GPGO_MultiPLCs.Helpers
{
    public class ObservableQueue<T> : INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable<T>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly Queue<T> queue = new Queue<T>();
        public int Count => queue.Count;

        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            PropertyChanged?.Invoke(this,
                                    new PropertyChangedEventArgs(nameof(Count)));
        }

        public T Dequeue()
        {
            var item = queue.Dequeue();
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            PropertyChanged?.Invoke(this,
                                    new PropertyChangedEventArgs(nameof(Count)));

            return item;
        }
    }
}