using System.Collections.Concurrent;

namespace DeepCopy.Internal
{
    internal sealed class ObjectReferencesCache
    {
        private readonly bool _canCacheAnything;
        private readonly ConcurrentDictionary<object, object> _cache;

        private ObjectReferencesCache(object self, bool canCacheAnything = true)
        {
            _canCacheAnything = canCacheAnything;
            _cache = self != null
                ? new() { [self] = self }
                : new();
        }

        public bool Get<T>(in T source, out T referenceObject)
        {
            if (_cache.TryGetValue(source, out var instance))
            {
                referenceObject = (T)instance;
                return true;
            }

            referenceObject = default;
            return false;
        }

        public void CacheSelf<T>(T obj)
        {
            _cache[obj] = obj;
        }

        public void RemoveCache<T>(T obj)
        {
            _cache.TryRemove(obj, out _);
        }

        public bool Add<T>(T source, T clonedObject)
        {
            if (!_canCacheAnything) return false;

            _cache[source] = clonedObject;
            return true;
        }

        public static ObjectReferencesCache Create(object self, bool canCacheAnything) =>
            new(self, canCacheAnything);
    }
}
