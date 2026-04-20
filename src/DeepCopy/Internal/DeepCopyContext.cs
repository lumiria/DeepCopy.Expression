#nullable enable

namespace DeepCopy.Internal
{
    internal sealed class DeepCopyContext
    {
        private readonly ObjectReferencesCache _cache;

        public DeepCopyContext(ObjectReferencesCache cache)
        {
            _cache = cache;
        }

        public ObjectReferencesCache Cache => _cache;
    }
}
