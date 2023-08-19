#nullable enable

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace DeepCopy.Internal
{
    internal sealed class ObjectReferencesCache
    {
        private readonly bool _canCacheAnything;
        private readonly IDictionary<object, object> _cache;

        private ObjectReferencesCache(object? self,  object? cloned, bool canCacheAnything = true)
        {
            _canCacheAnything = canCacheAnything;
            _cache = canCacheAnything
                ? self != null
                    ? new ConcurrentDictionary<object, object>() { [self] = cloned! }
                    : new ConcurrentDictionary<object, object>()
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

        public void RemoveSelfCache<T>(T obj)
            where T : notnull
        {
            if (_canCacheAnything) return;
            _cache.Remove(obj);
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
            private readonly List<KeyValuePair<object, object>> _list = new();

            public object this[object key]
            {
                get => _list.First(x => x.Key == key).Value;
                set => Add(key, value);
            }

            public ICollection<object> Keys => throw new NotImplementedException();

            public ICollection<object> Values => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(object key, object value)
            {
                _list.Add(new KeyValuePair<object, object>(key, value));
            }

            public void Add(KeyValuePair<object, object> item)
            {
                Add(item.Key, item.Value);
            }

            public void Clear()
            {
                _list.Clear();
            }

            public bool Contains(KeyValuePair<object, object> item)
            {
                return _list.Contains(item);
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
                int index = -1;
#if NETSTANDARD2_0
                foreach (var item in _list)
#else
                foreach (ref var item in CollectionsMarshal.AsSpan(_list))
#endif
                {
                    index++;
                    if (item.Key == key)
                    {
                        _list.RemoveAt(index);
                        return true;
                    }
                }
                return false;
            }

            public bool Remove(KeyValuePair<object, object> item)
            {
                return Remove(item.Key);
            }

#if NETSTANDARD2_0
            public bool TryGetValue(object key, out object value)
            {
                foreach (var item in _list)

#else
            public bool TryGetValue(object key, [MaybeNullWhen(false)] out object value)
            {
                foreach (ref var item in CollectionsMarshal.AsSpan(_list))
#endif
                {
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
}
