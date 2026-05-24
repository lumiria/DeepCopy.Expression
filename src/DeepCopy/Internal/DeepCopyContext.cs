#nullable enable

using System;
using System.Runtime.CompilerServices;

namespace DeepCopy.Internal
{
    internal sealed class DeepCopyContext
    {
        private readonly ObjectReferencesCache _cache;
        private CopyQueue? _queue;
        private int _depth;
        private int _lockCount;

        public DeepCopyContext(ObjectReferencesCache cache)
        {
            _cache = cache;
            _depth = 0;
            _lockCount = 0;
        }

        public DeepCopyContext()
        {
            _cache = ObjectReferencesCache.Default;
            _depth = 0;
            _lockCount = 0;
        }

        public ObjectReferencesCache Cache => _cache;

        public int QueueCount => _queue?.Count ?? 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EnterScope<T>(DeepCopyContext context, Type type, T source, out T instance)
            where T : notnull
        {
            if (_cache.TryGetOrCache(type, source, out instance))
                return true;

            if (_lockCount == 0 &&
                ++_depth > DeepCopyOptions.MaxRecursionDepth &&
                (!type.IsGenericType || !DeepCopyOptions.NonCopyableGenericTypes.Contains(type.GetGenericTypeDefinition())))
            {
                _cache.RemoveLatest();

                CopyQueue.NewOrAdd(ref _queue!, context);
                _queue.Push(type, source, new(instance));
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EnterScope<T>(DeepCopyContext context, T source, T instance)
            where T : notnull
        {
            _cache.Add(source, instance);

            if (_lockCount == 0 &&
                ++_depth > DeepCopyOptions.MaxRecursionDepth &&
                (!typeof(T).IsGenericType || !DeepCopyOptions.NonCopyableGenericTypes.Contains(typeof(T).GetGenericTypeDefinition())))
            {
                _cache.RemoveLatest();

                CopyQueue.NewOrAdd(ref _queue!, context);
                _queue.Push(source, new(instance));
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EnterScope<T>(DeepCopyContext context, Type type, T source, T instance)
            where T : notnull
        {
            _cache.Add(source, instance);

            if (_lockCount == 0 &&
                ++_depth > DeepCopyOptions.MaxRecursionDepth &&
                (!type.IsGenericType || !DeepCopyOptions.NonCopyableGenericTypes.Contains(type.GetGenericTypeDefinition())))
            {
                _cache.RemoveLatest();

                CopyQueue.NewOrAdd(ref _queue!, context);
                _queue.Push(type, source, new(instance));
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExitScope()
        {
            if (_lockCount == 0 &&
                _depth > DeepCopyOptions.MaxRecursionDepth)
            {
                _queue?.Push();
            }

            --_depth;
            _cache.RemoveLatest();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExitScope(int queueCount)
        {
            if (_queue?.Count > queueCount)
            {
                _queue.Push();
            }

            --_depth;
            _cache.RemoveLatest();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LockQeueue()
        {
            _lockCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnlockQeueue()
        {
            _lockCount--;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            _queue?.CloneAll();
        }

        public DeepCopyContext Clone()
        {
            return new DeepCopyContext(_cache.Clone())
            {
                _queue = this._queue
            };
        }
    }
}
