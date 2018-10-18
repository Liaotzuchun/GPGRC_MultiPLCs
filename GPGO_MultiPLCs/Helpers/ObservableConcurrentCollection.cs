using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace GPGO_MultiPLCs.Helpers
{
    /// <summary>
    ///     Provides a base implementation for producer-consumer collections that wrap other producer-consumer
    ///     collections.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
    [Serializable]
    public abstract class ProducerConsumerCollectionBase<T> : IProducerConsumerCollection<T>
    {
        /// <summary>Gets the number of elements contained in the collection.</summary>
        public int Count => ContainedCollection.Count;

        /// <summary>Gets whether the collection is synchronized.</summary>
        bool ICollection.IsSynchronized => ContainedCollection.IsSynchronized;

        /// <summary>Gets the synchronization root object for the collection.</summary>
        object ICollection.SyncRoot => ContainedCollection.SyncRoot;

        /// <summary>Copies the contents of the collection to an array.</summary>
        /// <param name="array">The array to which the data should be copied.</param>
        /// <param name="index">The starting index at which data should be copied.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            ContainedCollection.CopyTo(array, index);
        }

        /// <summary>Gets an enumerator for the collection.</summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Gets an enumerator for the collection.</summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ContainedCollection.GetEnumerator();
        }

        /// <summary>Copies the contents of the collection to an array.</summary>
        /// <param name="array">The array to which the data should be copied.</param>
        /// <param name="index">The starting index at which data should be copied.</param>
        public void CopyTo(T[] array, int index)
        {
            ContainedCollection.CopyTo(array, index);
        }

        /// <summary>Creates an array containing the contents of the collection.</summary>
        /// <returns>The array.</returns>
        public T[] ToArray()
        {
            return ContainedCollection.ToArray();
        }

        /// <summary>Attempts to add the specified value to the end of the deque.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns>true if the item could be added; otherwise, false.</returns>
        bool IProducerConsumerCollection<T>.TryAdd(T item)
        {
            return TryAdd(item);
        }

        /// <summary>Attempts to remove and return an item from the collection.</summary>
        /// <param name="item">
        ///     When this method returns, if the operation was successful, item contains the item removed. If no
        ///     item was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>true if an element was removed and returned from the collection; otherwise, false.</returns>
        bool IProducerConsumerCollection<T>.TryTake(out T item)
        {
            return TryTake(out item);
        }

        /// <summary>Gets the contained collection.</summary>
        protected IProducerConsumerCollection<T> ContainedCollection { get; }

        /// <summary>Attempts to add the specified value to the end of the deque.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns>true if the item could be added; otherwise, false.</returns>
        protected virtual bool TryAdd(T item)
        {
            return ContainedCollection.TryAdd(item);
        }

        /// <summary>Attempts to remove and return an item from the collection.</summary>
        /// <param name="item">
        ///     When this method returns, if the operation was successful, item contains the item removed. If no
        ///     item was available to be removed, the value is unspecified.
        /// </param>
        /// <returns>true if an element was removed and returned from the collection; otherwise, false.</returns>
        protected virtual bool TryTake(out T item)
        {
            return ContainedCollection.TryTake(out item);
        }

        /// <summary>Initializes the ProducerConsumerCollectionBase instance.</summary>
        /// <param name="contained">The collection to be wrapped by this instance.</param>
        protected ProducerConsumerCollectionBase(IProducerConsumerCollection<T> contained)
        {
            ContainedCollection = contained ?? throw new ArgumentNullException(nameof(contained));
        }
    }

    public sealed class ObservableConcurrentCollection<T> : ProducerConsumerCollectionBase<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>Event raised when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>Event raised when a property on the collection changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly SynchronizationContext _context;

        public void Add(T item)
        {
            TryAdd(item);
        }

        public void Clear()
        {
            while (ContainedCollection.Count > 0)
            {
                base.TryTake(out _);
            }

            NotifyObserversOfChange();
        }

        protected override bool TryAdd(T item)
        {
            // Try to add the item to the underlying collection.  If we were able to,
            // notify any listeners.
            var result = base.TryAdd(item);
            if (result)
            {
                NotifyObserversOfChange();
            }

            return result;
        }

        protected override bool TryTake(out T item)
        {
            // Try to remove an item from the underlying collection.  If we were able to,
            // notify any listeners.
            var result = base.TryTake(out item);
            if (result)
            {
                NotifyObserversOfChange();
            }

            return result;
        }

        /// <summary>Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.</summary>
        private void NotifyObserversOfChange()
        {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;
            if (collectionHandler != null || propertyHandler != null)
            {
                _context.Post(s =>
                              {
                                  collectionHandler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                                  propertyHandler?.Invoke(this, new PropertyChangedEventArgs("Count"));
                              },
                              null);
            }
        }

        /// <summary>Initializes an instance of the ObservableConcurrentCollection class with an underlying queue data structure.</summary>
        public ObservableConcurrentCollection() : this(new ConcurrentQueue<T>())
        {
        }

        /// <summary>
        ///     Initializes an instance of the ObservableConcurrentCollection class with the specified collection as the
        ///     underlying data structure.
        /// </summary>
        public ObservableConcurrentCollection(IProducerConsumerCollection<T> collection) : base(collection)
        {
            _context = AsyncOperationManager.SynchronizationContext;
        }
    }
}