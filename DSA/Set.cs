using System;
using System.Collections.Generic;

namespace Set
{
    public class Set<T> : IEnumerable<T>
        where T: IComparable<T>
    {
        private readonly List<T> _items = new List<T>();

        public Set()
        {
        }

        public Set(IEnumerable<T> items)
        {
            AddRange(items);
        }

        public void Add(T item)
        {
            if (Contains(item))
            {
                throw new InvalidOperationException($"Item already exists in Set");
            }

            _items.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        private void AddSkipDuplicates(T item)
        {
            if (!Contains(item))
            {
                _items.Add(item);
            }
        }

        private void AddRangeSkipDuplicates(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                AddSkipDuplicates(item);
            }
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public int Count => _items.Count;

        public Set<T> Union(Set<T> other)
        {
            var result = new Set<T>(_items);
            result.AddRangeSkipDuplicates(other._items);

            return result;
        }

        public Set<T> Intersection(Set<T> other)
        {
            var result = new Set<T>();

            foreach (var item in _items)
            {
                if (other._items.Contains(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public Set<T> Difference(Set<T> other)
        {
            var result = new Set<T>(_items);

            foreach (var item in other._items)
            {
                result.Remove(item);
            }

            return result;
        }

        public Set<T> SymmetricDifference(Set<T> other)
        {
            var intersection = Intersection(other);
            var union = Union(other);

            return union.Difference(intersection);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
