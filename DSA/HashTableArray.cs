using System;
using System.Collections.Generic;
using System.Linq;

namespace HashTable
{
    /// <summary>
    /// The fixed size array of the nodes in the hash table
    /// </summary>
    /// <typeparam name="TKey">The key type of the hash table</typeparam>
    /// <typeparam name="TValue">The value type of the hash table</typeparam>
    internal class HashTableArray<TKey, TValue>
    {
        readonly HashTableArrayNode<TKey, TValue>[] _array;

        /// <summary>
        /// Constructs a new hash table array with the specified capacity
        /// </summary>
        /// <param name="capacity">The capacity of the array</param>
        public HashTableArray(int capacity)
        {
            _array = new HashTableArrayNode<TKey, TValue>[capacity];
            for (var i = 0; i < capacity; i++)
            {
                _array[i] = new HashTableArrayNode<TKey, TValue>();
            }
        }

        /// <summary>
        /// Adds the key/value pair to the node.  If the key already exists in the
        /// node array an ArgumentException will be thrown
        /// </summary>
        /// <param name="key">The key of the item being added</param>
        /// <param name="value">The value of the item being added</param>
        public void Add(TKey key, TValue value)
        {
            _array[GetIndex(key)].Add(key, value);
        }

        /// <summary>
        /// Updates the value of the existing key/value pair in the node array.
        /// If the key does not exist in the array an ArgumentException
        /// will be thrown.
        /// </summary>
        /// <param name="key">The key of the item being updated</param>
        /// <param name="value">The updated value</param>
        public void Update(TKey key, TValue value)
        {
            _array[GetIndex(key)].Update(key, value);
        }

        /// <summary>
        /// Removes the item from the node array whose key matches
        /// the specified key.
        /// </summary>
        /// <param name="key">The key of the item to remove</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        public bool Remove(TKey key)
        {
            return _array[GetIndex(key)].Remove(key);
        }

        /// <summary>
        /// Finds and returns the value for the specified key.
        /// </summary>
        /// <param name="key">The key whose value is sought</param>
        /// <param name="value">The value associated with the specified key</param>
        /// <returns>True if the value was found, false otherwise</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _array[GetIndex(key)].TryGetValue(key, out value);
        }

        /// <summary>
        /// The capacity of the hash table array
        /// </summary>
        public int Capacity => _array.Length;

        /// <summary>
        /// Removes every item from the hash table array
        /// </summary>
        public void Clear()
        {
            foreach (var node in _array)
            {
                node.Clear();
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the values in the node array
        /// </summary>
        public IEnumerable<TValue> Values 
        {
            get {
                return _array.SelectMany(node => node.Values);
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the keys in the node array
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get {
                return _array.SelectMany(node => node.Keys);
            }
        }

        /// <summary>
        /// Returns an enumerator for all of the Items in the node array
        /// </summary>
        public IEnumerable<HashTableNodePair<TKey, TValue>> Items
        {
            get {
                return _array.SelectMany(node => node.Items);
            }
        }

        // Maps a key to the array index based on hash code
        private int GetIndex(TKey key)
        {
            return Math.Abs(key.GetHashCode() % Capacity);
        }
    }
}
