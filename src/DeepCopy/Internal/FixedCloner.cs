#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Collections.Frozen;
#endif
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DeepCopy.Internal.FixedCloners;
using DeepCopy.Internal.FixedCloners.Core;

namespace DeepCopy.Internal
{
    internal static class FixedCloner
    {
        private readonly static Dictionary<Type, CustomCloneBuilder> _bag;

        static FixedCloner()
        {
            _bag = new()
            {
                [typeof(HashSet<>)] = HashSetCloner.Build,
                [typeof(ConcurrentDictionary<,>)] = ConcurrentDictionaryCloner.Build,
                [typeof(ReadOnlyDictionary<,>)] = ReadOnlyDictionaryCloner.Build,
                [typeof(SortedDictionary<,>)] = DictionaryCloneExpressionBuilder.Build,
#if NET8_0_OR_GREATER
                [typeof(ImmutableArray<>)] = ImmutableArrayCloner.Build,
                [typeof(ImmutableHashSet<>)] = ImmutableHashSetCloner.Build,
                [typeof(ImmutableSortedSet<>)] = ImmutableSortedSetCloner.Build,
                [typeof(ImmutableDictionary<,>)] = ImmutableDictinoaryCloner.Build,
                [typeof(FrozenDictionary<,>)] = FrozenDictinoaryCloner.Build,
#endif
#if NET10_0_OR_GREATER
                [typeof(OrderedDictionary<,>)] = DictionaryCloneExpressionBuilder.Build,
#endif
            };
        }

#if NETSTANDARD2_0
        public static bool TryGetBuilder(Type type, out CustomCloneBuilder? builder)
#else
        public static bool TryGetBuilder(Type type, [MaybeNullWhen(false)] out CustomCloneBuilder? builder)
#endif
        {
            if (type.IsGenericType)
            {
                if (_bag.TryGetValue(type.GetGenericTypeDefinition(), out builder))
                    return true;

#if NET10_0_OR_GREATER
                if (IsSubclassOfGeneric(type, typeof(FrozenDictionary<,>)))
                {
                    builder = FrozenDictinoaryCloner.Build;
                    return true;
                }
#endif
            }


            return _bag.TryGetValue(type, out builder);
        }

        public static void Add(Type type, CustomCloneBuilder builder)
        {
            _bag.Add(type, builder);
        }

        private static bool IsSubclassOfGeneric(Type? type, Type genericType)
        {
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
