#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DeepCopy.Internal
{
    public sealed class ObjectReferencesCache
    {
        private readonly Dictionary<object, object>? _cache;
        private readonly ObjectCacheDictionary? _liteCache;

        private ObjectReferencesCache(bool canCacheAnything , object self, object cloned)
        {
            if (canCacheAnything)
            {
                _cache = new Dictionary<object, object>(ReferenceEqualityComparer.Instance) { [self] = cloned };
                return;
            }

            _liteCache = new ObjectCacheDictionary() { [self] = cloned };
        }

        private ObjectReferencesCache(bool canCacheAnything)
        {
            if (canCacheAnything)
            {
                _cache = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);
                return;
            }

            _liteCache = new ObjectCacheDictionary();
        }

        public static ObjectReferencesCache Default { get; } =
            new ObjectReferencesCache(false);

#if NETSTANDARD2_0
        public bool Get<T>(in T source, out T? referenceObject)
#else
        public bool Get<T>(in T source, [NotNullWhen(true)] out T? referenceObject)
#endif
            where T : notnull
        {
            if (_liteCache?.TryGetValue(source, out var instance) ??
                _cache!.TryGetValue(source, out instance))
                
            {
                referenceObject = (T)instance;
                return true;
            }

            referenceObject = default;
            return false;
        }

        public bool TryGetOrCache<T>(Type type, in T source, out T referenceObject)
    where T : notnull
        {
            if (_liteCache?.TryGetValue(source, out var instance) ??
                _cache!.TryGetValue(source, out instance))
            {
                referenceObject = (T)instance;
                return true;
            }

#if NETSTANDARD2_0
            referenceObject = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#else
            referenceObject = (T)RuntimeHelpers.GetUninitializedObject(type);
#endif
            Add(source, referenceObject);

            return false;
        }

        public void RemoveLatest()
        {
            _liteCache?.Remove();
        }

        public void ReplaceLatest<T>(T source, T clonedObject)
            where T : notnull
        {
            if (_liteCache == null) return;
            _liteCache.Remove();
            _liteCache.Add(source, clonedObject);
        }

        public void Add<T>(T source, T clonedObject)
            where T : notnull
        {
            if (_liteCache == null)
            {
                _cache!.Add(source, clonedObject);
                return;
            }

            _liteCache?.Add(source, clonedObject);
        }

        public static ObjectReferencesCache Create(bool canCacheAnything, object self, object cloned) =>
            new(canCacheAnything, self, cloned);

        public static ObjectReferencesCache Create(bool canCacheAnything) =>
            new(canCacheAnything);


        private sealed class ObjectCacheDictionary
        {
            private KeyValuePair<object, object>[] _items = new KeyValuePair<object, object>[2];
            private int _lastIndex = -1;

            public object this[object key]
            {
                set => Add(key, value);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(object key, object value)
            {
                if (++_lastIndex >= _items.Length)
                {
                    var items = new KeyValuePair<object, object>[_items.Length * 2];
                    _items.CopyTo(items, 0);
                    _items = items;
                }
                _items[_lastIndex] = new KeyValuePair<object, object>(key, value);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove()
            {
                _lastIndex--;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                foreach (ref var item in _items.AsSpan(0, _lastIndex + 1))
                {
#endif
                    if (ReferenceEquals(item.Key, key))
                    {
                        value = item.Value;
                        return true;
                    }
                }
                value = default!;
                return false;
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
