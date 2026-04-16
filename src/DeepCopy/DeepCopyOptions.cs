using System;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace DeepCopy
{
    public static class DeepCopyOptions
    {
        public static CopyBehavior ReadOnlyStructBehavior { get; set; } = CopyBehavior.TryClone;

        public static HashSet<Type> NonCopyableGenericTypes { get; } =
        [
#if NET8_0_OR_GREATER
            typeof(ImmutableList<>),
            typeof(ImmutableStack<>),
            typeof(ImmutableQueue<>),
            typeof(ImmutableHashSet<>),
#endif
        ];

        public enum CopyBehavior
        {
            Ignore,
            ShallowCopy,
            TryClone,
        }
    }
}
