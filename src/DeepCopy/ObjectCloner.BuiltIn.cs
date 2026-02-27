#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DeepCopy.Internal;
using DeepCopy.Internal.BuiltIns;

namespace DeepCopy
{
    public static partial class ObjectCloner
    {
        public static object? Clone(object? source, bool preserveObjectReferences = false)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(object).Name}] | Source = [{source?.GetType()}]");
#endif
            if (source == null) return default;

            var type = source.GetType();
            if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var cloner = DictionaryCloneDelegateGenerator.GetOrCreateDelegate<Func<object, ObjectReferencesCache, object>>(type);
                return cloner(source, ObjectReferencesCache.Create(preserveObjectReferences));
            }

#if NETSTANDARD2_0
            var instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#else
            var instance = RuntimeHelpers.GetUninitializedObject(type);
#endif

            if (type.IsValueType)
            {
                _CopyValueType(type, source, ref instance, ObjectReferencesCache.Default);
                    //CreateObjectReferenceCache(preserveObjectReferences));
            }
            else
            {
                _CopyTo(type, source, instance,
                    ObjectReferencesCache.Create(preserveObjectReferences, source, instance));
            }

            return instance;
        }

#if NETSTANDARD2_0
        public static Dictionary<TKey, TValue>? Clone<TKey, TValue>(Dictionary<TKey, TValue>? source, bool preserveObjectReferences = false)
#else
        [return: NotNullIfNotNull(nameof(source))]
        public static Dictionary<TKey, TValue>? Clone<TKey, TValue>(Dictionary<TKey, TValue>? source, bool preserveObjectReferences = false)
#endif
            where TKey : notnull
        {
            return DictionaryCloner<TKey, TValue>.Clone(source, ObjectReferencesCache.Create(preserveObjectReferences));
        }
    }
}
