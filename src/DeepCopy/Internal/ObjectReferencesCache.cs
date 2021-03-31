using System.Collections.Concurrent;

namespace DeepCopy.Internal
{
    internal sealed class ObjectReferencesCache
    {
        private readonly bool _isEmpty;
        private readonly ConcurrentDictionary<object, object> _cache;

        private ObjectReferencesCache(bool isEmpty = false)
        {
            _isEmpty = isEmpty;
            _cache = new ConcurrentDictionary<object, object>();
        }

        public bool Get<T>(T source, out T referenceObject)
        {
            if (!_isEmpty && _cache.TryGetValue(source, out var instance))
            {
                referenceObject = (T)instance;
                return true;
            }

            referenceObject = default;
            return false;
        }

        public void Add<T>(T source, T clonedObject)
        {
            if (_isEmpty) return;

            _cache[source] = clonedObject;
        }


        public static ObjectReferencesCache Create() =>
            new(false);

        public static ObjectReferencesCache Empty() =>
            new(true);
    }
}
