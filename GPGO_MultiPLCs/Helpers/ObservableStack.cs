using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GPGO_MultiPLCs.Helpers
{
    public class ObservableStack<T> : INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable<T>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return stack.GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly Stack<T> stack = new Stack<T>();
        public int Count => stack.Count;

        public void Push(T item)
        {
            stack.Push(item);
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            PropertyChanged?.Invoke(this,
                                    new PropertyChangedEventArgs(nameof(Count)));
        }

        public T Pop()
        {
            var item = stack.Pop();
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            PropertyChanged?.Invoke(this,
                                    new PropertyChangedEventArgs(nameof(Count)));

            return item;
        }
    }
}