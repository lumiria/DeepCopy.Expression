#nullable enable

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DeepCopy.Internal
{
    internal sealed class ObjectReferencesCache
    {
        private readonly bool _canCacheAnything;
        private readonly IDictionary<object, object> _cache;
        private static readonly object dummy = new();

        private ObjectReferencesCache(object? self,  object? cloned, bool canCacheAnything = true)
        {
            _canCacheAnything = canCacheAnything;
            _cache = canCacheAnything
                ? self != null
                    ? new ConcurrentDictionary<object, object>(ReferenceEqualityComparer.Instance) { [self] = cloned! }
                    : new ConcurrentDictionary<object, object>(ReferenceEqualityComparer.Instance)
                : self != null
                    ? new ObjectCacheDictionary() { [self] = cloned! }
                    : new ObjectCacheDictionary();
        }

#if NETSTANDARD2_0
        public bool Get<T>(in T source, out T? referenceObject)
#else
        public bool Get<T>(in T source, [NotNullWhen(true)] out T? referenceObject)
#endif
            where T : notnull
        {
            if (_cache.TryGetValue(source, out var instance))
            {
                referenceObject = (T)instance;
                return true;
            }

            referenceObject = default;
            return false;
        }

        public void RemoveLatest()
        {
            if (_canCacheAnything) return;
            _cache.Remove(dummy);
        }

        public void Add<T>(T source, T clonedObject)
            where T : notnull
        {
            _cache[source] = clonedObject;
        }

        public static ObjectReferencesCache Create(object? self, object? cloned, bool canCacheAnything) =>
            new(self, cloned, canCacheAnything);


        private sealed class ObjectCacheDictionary : IDictionary<object, object>
        {
            private KeyValuePair<object, object>[] _items = new KeyValuePair<object, object>[4];
            private int _lastIndex = -1;

            public object this[object key]
            {
                get => throw new NotImplementedException();
                set => Add(key, value);
            }

            public ICollection<object> Keys => throw new NotImplementedException();

            public ICollection<object> Values => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(object key, object value)
            {
                if (++_lastIndex >= _items.Length)
                {
                    var items = new KeyValuePair<object, object>[_items.Length + 4];
                    _items.CopyTo(items, 0);
                    _items = items;
                }
                _items[_lastIndex] = new KeyValuePair<object, object>(key, value);
            }

            public void Add(KeyValuePair<object, object> item)
            {
                Add(item.Key, item.Value);
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<object, object> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(object key)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(object key)
            {
                _lastIndex--;
                return true;
            }

            public bool Remove(KeyValuePair<object, object> item)
            {
                throw new NotImplementedException();
                //return Remove(item.Key);
            }

#if NETSTANDARD2_0
            public bool TryGetValue(object key, out object value)
            {
                //foreach (var item in _list)
                int index = -1;
                foreach (var item in _items)
                {
                    if (++index > _lastIndex) break;

#else
            public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
            {
                //foreach (ref var item in CollectionsMarshal.AsSpan(_list)[..(_lastIndex + 1)])
                foreach (ref var item in _items.AsSpan(0, _lastIndex + 1))
                {
#endif
                    if (item.Key == key)
                    {
                        value = item.Value;
                        return true;
                    }
                }
                value = null;
                return false;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }

    file sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static ReferenceEqualityComparer Instance { get; } =
            new();

        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

#if NETSTANDARD2_0
        public int GetHashCode(object obj)
#else
        public int GetHashCode([DisallowNull] object obj)
#endif
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
