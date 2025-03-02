#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using DeepCopy.Internal.FixedCloners;

namespace DeepCopy.Internal
{
    internal static class FixedCloner
    {
        private readonly static Dictionary<Type, CustomCloneBuilder> _bag;

        static FixedCloner()
        {
            _bag = new ()
            {
                [typeof(Dictionary<,>)] = DictionaryCloner.Build,
                [typeof(HashSet<>)] = HashSetCloner.Build,
                [typeof(ConcurrentDictionary<,>)] = ConcurrentDictionaryCloner.Build,
                [typeof(ReadOnlyDictionary<,>)] = ReadOnlyDictionaryCloner.Build
            };
        }

#if NETSTANDARD2_0
        public static bool TryGetBuilder(Type type, out CustomCloneBuilder? builder)
#else
        public static bool TryGetBuilder(Type type, [MaybeNullWhen(false)] out CustomCloneBuilder? builder)
#endif
        {
            if (type.IsGenericType && _bag.TryGetValue(type.GetGenericTypeDefinition(), out builder))
                return true;

            return _bag.TryGetValue(type, out builder);
        }

        public static void Add(Type type, CustomCloneBuilder builder)
        {
            _bag.Add(type, builder);
        }
    }
}
