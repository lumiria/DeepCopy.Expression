#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace DeepCopy.Internal
{
    internal sealed class DeepCopyContext
    {
        private readonly ObjectReferencesCache _cache;
        private CopyStack? _stack;
        private int _depth;

        public DeepCopyContext(ObjectReferencesCache cache)
        {
            _cache = cache;
            _depth = 0;
        }

        public DeepCopyContext()
        {
            _cache = ObjectReferencesCache.Default;
            _depth = 0;
        }

        public ObjectReferencesCache Cache => _cache;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EnterScope<T>(DeepCopyContext context, Type type, T source, out T instance)
            where T : notnull
        {
            if (_cache.TryGetOrCache(type, source, out instance))
                return true;

            if (++_depth > DeepCopyOptions.MaxRecursionDepth &&
                !DeepCopyOptions.NonCopyableGenericTypes.Contains(type))
            {
                _stack ??= new();
                _stack.Push(source, new(instance), context);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EnterScope<T>(DeepCopyContext context,  T source, T instance)
            where T : notnull
        {
            _cache.Add(source, instance);

            if (++_depth > DeepCopyOptions.MaxRecursionDepth &&
                !DeepCopyOptions.NonCopyableGenericTypes.Contains(typeof(T)))
            {
                _stack ??= new();
                _stack.Push(source, new(instance), context);
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExitScope()
        {
            _cache.RemoveLatest();
            --_depth;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            if (_stack?.Count > 0)
            {
                _depth = 0;
                _stack.CloneAll();
            }
        }
    }
}
