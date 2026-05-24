#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DeepCopy.Internal
{
    internal sealed class CopyQueue
    {
        private readonly Queue<(DeepCopyContext context, Queue<Action<DeepCopyContext>> Queue)> queue = new();
        private Queue<Action<DeepCopyContext>>? _current;

        private CopyQueue()
        {
            _current = new Queue<Action<DeepCopyContext>>();
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => queue.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NETSTANDARD2_0
        public static void NewOrAdd(ref CopyQueue instance, DeepCopyContext context)
#else
        public static void NewOrAdd([NotNull] ref CopyQueue? instance, DeepCopyContext context)
#endif
        {
            if (instance is null)
            {
                instance = new();
                instance.queue.Enqueue((context.Clone(), instance._current!));
                return;
            }

            instance.Add(context.Clone());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push<T>(Type type, T source, RefBox<T> instance)
            where T : notnull
        {
            _current?.Enqueue(context =>
            {
                int queueCount = context.QueueCount;
                context.Cache.Add(source, instance.Value);
                ObjectCloner.CopyTo(type, source, ref instance.Value, context);
                context.ExitScope(queueCount);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push<T>(T source, RefBox<T> instance)
            where T : notnull
        {
            _current?.Enqueue(context =>
            {
                int queueCount = context.QueueCount;
                context.Cache.Add(source, instance.Value);
                ObjectCloner.CopyToAs(source, ref instance.Value, context);
                context.ExitScope(queueCount);
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push()
        {
            _current?.Enqueue(context => context.Cache.RemoveLatest());
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CloneAll()
        {
            while (queue.Count > 0)
            {
                _current = null;

                var (context, queue) = this.queue.Dequeue();
                while (queue.Count > 0)
                {
                    queue.Dequeue()(context);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(DeepCopyContext context)
        {
            _current = new Queue<Action<DeepCopyContext>>();
            queue.Enqueue((context, _current));
        }
    }

    internal struct RefBox<T>
    {
        public RefBox(T value)
        {
            Value = value;
        }

        public T Value;
    }
}
