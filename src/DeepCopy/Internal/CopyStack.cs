using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DeepCopy.Internal
{
    internal sealed class CopyStack
    {
        private readonly Stack<Action> _stack = new();

        public int Count => _stack.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push<T>(Type type, T source, RefBox<T> instance, DeepCopyContext context)
        {
            _stack.Push(() => ObjectCloner.CopyTo(type, source, ref instance.Value, context));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push<T>(T source, RefBox<T> instance, DeepCopyContext context) =>
            _stack.Push(() => ObjectCloner.CopyToAs(source, ref instance.Value, context));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CloneAll()
        {
            while (_stack.Count > 0)
            {
                var clone = _stack.Pop();
                clone();
            }
        }
    }

    internal sealed class RefBox<T>
    {
        public RefBox(T value)
        {
            Value = value;
        }

        public T Value;
    }
}
