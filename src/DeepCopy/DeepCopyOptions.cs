using System;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Collections.Frozen;
#endif

namespace DeepCopy
{
    public static class DeepCopyOptions
    {
        public static int MaxRecursionDepth { get; set; } = 500;

        public static CopyBehavior ReadOnlyStructBehavior { get; set; } = CopyBehavior.TryClone;

#if NET8_0_OR_GREATER
        public static ImmutableHashSet<Type> NonCopyableGenericTypes { get; } =
#else
        public static HashSet<Type> NonCopyableGenericTypes { get; } =
#endif
        [
            typeof(Dictionary<,>),
            typeof(SortedDictionary<,>),
#if NET8_0_OR_GREATER
            typeof(ImmutableList<>),
            typeof(ImmutableStack<>),
            typeof(ImmutableQueue<>),
            typeof(ImmutableHashSet<>),
            typeof(ImmutableSortedSet<>),
            typeof(ImmutableDictionary<,>),
            typeof(FrozenDictionary<,>),
#endif
#if NET10_0_OR_GREATER
            typeof(OrderedDictionary<,>),
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
