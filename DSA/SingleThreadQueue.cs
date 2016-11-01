using System;
using Queue.Common;

namespace Queue.Single
{
    /// <summary>
    /// A First In First Out collection
    /// </summary>
    /// <typeparam name="T">The type of data stored in the collection</typeparam>
    public class Queue<T> : IQueue<T>
    {
        private T[] _items = new T[0];

        // the number of items in the queue

        // the index of the first (oldest) item in the queue
        private int _head;

        // the index of the last (newest) item in the queue
        private int _tail = -1;

        /// <summary>
        /// Adds an item to the back of the queue
        /// </summary>
        /// <param name="item">The item to place in the queue</param>
        public void Enqueue(T item)
        {
            // if the array needs to grow
            if (_items.Length == Count)
            {
                var newLength = (Count == 0) ? 4 : Count * 2;

                var newArray = new T[newLength];

                if (Count > 0)
                {
                    // copy contents...
                    // if the array has no wrapping, just copy the valid range
                    // else copy from head to end of the array and then from 0 to the tail
                    // if tail is less than head we've wrapped
                    var targetIndex = 0;

                    if (_tail < _head)
                    {
                        // copy the _items[head].._items[end] -> newArray[0]..newArray[N]
                        for (var index = _head; index < _items.Length; index++)
                        {
                            newArray[targetIndex] = _items[index];
                            targetIndex++;
                        }

                        // copy _items[0].._items[tail] -> newArray[N+1]..
                        for (var index = 0; index <= _tail; index++)
                        {
                            newArray[targetIndex] = _items[index];
                            targetIndex++;
                        }
                    }
                    else
                    {
                        // copy the _items[head].._items[tail] -> newArray[0]..newArray[N]
                        for (var index = _head; index <= _tail; index++)
                        {
                            newArray[targetIndex] = _items[index];
                            targetIndex++;
                        }
                    }

                    _head = 0;
                    _tail = targetIndex - 1; // compensate for the extra bump
                }
                else
                {
                    _head = 0;
                    _tail = -1;
                }

                _items = newArray;
            }

            // now we have a properly sized array and can focus on wrapping issues.

            // if _tail is at the end of the array we need to wrap around
            if (_tail == _items.Length - 1)
            {
                _tail = 0;
            }
            else
            {
                _tail++;
            }

            _items[_tail] = item;
            Count++;
        }

        /// <summary>
        /// Removes and returns the front item from the queue
        /// </summary>
        /// <returns>The front item from the queue</returns>

        public T Dequeue()
        {
            var value = _items[_head];

            if (_head == _items.Length - 1)
            {
                // if the head is at the last 
                // index in the array - wrap around.
                _head = 0;
            }
            else
            {
                // move to the next value
                _head++;
            }

            Count--;

            return value;
        }

        /// <summary>
        /// Returns the front item from the queue without removing it from the queue
        /// </summary>
        /// <returns>The front item from the queue</returns>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new QueueEmptyException();
            }

            return _items[_head];
        }

        /// <summary>
        /// The number of items in the queue
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Removes all items from the queue
        /// </summary>
        public void Clear()
        {
            Count = 0;
            _head = 0;
            _tail = -1;
        }

        /// <summary>
        /// Returns an enumerator that enumerates the queue
        /// </summary>
        /// <returns>The enumerator</returns>
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            if (Count <= 0) yield break;
            // if the queue wraps then handle that case
            if (_tail < _head)
            {
                // head -> end
                for (var index = _head; index < _items.Length; index++)
                {
                    yield return _items[index];
                }

                // 0 -> tail
                for (var index = 0; index <= _tail; index++)
                {
                    yield return _items[index];
                }
            }
            else
            {
                // head -> tail
                for (var index = _head; index <= _tail; index++)
                {
                    yield return _items[index];
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that enumerates the queue
        /// </summary>
        /// <returns>The enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public interface IQueue<T> : System.Collections.Generic.IEnumerable<T>
    {
        /// <summary>
        /// Adds an item to the back of the queue
        /// </summary>
        /// <param name="item">The item to place in the queue</param>
        void Enqueue(T item);

        /// <summary>
        /// Removes and returns the front item from the queue
        /// </summary>
        /// <returns>The front item from the queue</returns>
        T Dequeue();

        /// <summary>
        /// Returns the front item from the queue without removing it from the queue
        /// </summary>
        /// <returns>The front item from the queue</returns>
        T Peek();

        /// <summary>
        /// The number of items in the queue
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Removes all items from the queue
        /// </summary>
        void Clear();
    }

    public class QueueEmptyException : Exception
    {
    }
}
