using System;

namespace Stack.Array
{
    /// <summary>
    /// A Last In First Out (LIFO) collection implemented as an array.
    /// </summary>
    /// <typeparam name="T">The type of item contained in the stack</typeparam>
    public class Stack<T> : System.Collections.Generic.IEnumerable<T>
    {
        // The array of items contained in the stack.  Initialized to 0 length,
        // will grow as needed during Push
        private T[] _items = new T[0];

        // The current number of items in the stack.

        /// <summary>
        /// Adds the specified item to the stack
        /// </summary>
        /// <param name="item">The item</param>
        public void Push(T item)
        {
            // _size = 0 ... first push
            // _size == length ... growth boundary
            if (Count == _items.Length)
            {
                // initial size of 4, otherwise double the current length
                var newLength = Count == 0 ? 4 : Count * 2;

                // allocate, copy and assign the new array
                var newArray = new T[newLength];
                _items.CopyTo(newArray, 0);
                _items = newArray;
            }

            // add the item to the stack array and increase the size
            _items[Count] = item;
            Count++;
        }

        /// <summary>
        /// Removes and returns the top item from the stack
        /// </summary>
        /// <returns>The top-most item in the stack</returns>
        public T Pop()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The stack is empty");
            }

            Count--;
            return _items[Count];
        }

        /// <summary>
        /// Returns the top item from the stack without removing it from the stack
        /// </summary>
        /// <returns>The top-most item in the stack</returns>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The stack is empty");
            }

            return _items[Count - 1];
        }

        /// <summary>
        /// The current number of items in the stack
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Removes all items from the stack
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Enumerates each item in the stack in LIFO order.  The stack remains unaltered.
        /// </summary>
        /// <returns>The LIFO enumerator</returns>
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            for (var i = Count-1; i >= 0; i--)
            {
                yield return _items[i];
            }
        }

        /// <summary>
        /// Enumerates each item in the stack in LIFO order.  The stack remains unaltered.
        /// </summary>
        /// <returns>The LIFO enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
