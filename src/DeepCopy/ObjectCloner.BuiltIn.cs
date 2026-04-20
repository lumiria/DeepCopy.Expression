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
                var cloner = DictionaryCloneDelegateGenerator.GetOrCreateDelegate<Func<object, DeepCopyContext, object>>(type);
                return cloner(source, new(ObjectReferencesCache.Create(preserveObjectReferences)));
            }

#if NETSTANDARD2_0
            var instance = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
#else
            var instance = RuntimeHelpers.GetUninitializedObject(type);
#endif

            if (type.IsValueType)
            {
                _CopyValueType(type, source, ref instance,
                    new (ObjectReferencesCache.Create(preserveObjectReferences)));
            }
            else
            {
                _CopyTo(type, source, ref instance,
                    new (ObjectReferencesCache.Create(preserveObjectReferences, source, instance)));
            }

            return instance;
        }

#if NETSTANDARD2_0
        public static Array? Clone(Array? source, bool preserveObjectReferences = false)
#else
        [return: NotNullIfNotNull(nameof(source))]
        public static Array? Clone(Array? source, bool preserveObjectReferences = false)
#endif
        {
            if (source == null) return null;

            var type = source.GetType();
            var cloner = ArrayCloneDelegateGenerator.GetOrCreateWrapperDelegate(type);
            var instance = cloner(source, new (ObjectReferencesCache.Create(preserveObjectReferences)));

            return instance;
        }

#if NETSTANDARD2_0
        public static void CopyTo(Array? source, Array desitination, bool preserveObjectReferences = false)
#else
        public static void CopyTo(Array? source, Array desitination, bool preserveObjectReferences = false)
#endif
        {
            if (source == null) return;

            var type = source.GetType();
            var cloner = ArrayCloneDelegateGenerator.GetOrCreateWrapperDelegate(type);
            var instance = cloner(source, new (ObjectReferencesCache.Create(preserveObjectReferences)));

            Array.Copy(instance, desitination, desitination.Length);
        }

#if NETSTANDARD2_0
        public static Dictionary<TKey, TValue>? Clone<TKey, TValue>(Dictionary<TKey, TValue>? source, bool preserveObjectReferences = false)
#else
        [return: NotNullIfNotNull(nameof(source))]
        public static Dictionary<TKey, TValue>? Clone<TKey, TValue>(Dictionary<TKey, TValue>? source, bool preserveObjectReferences = false)
#endif
            where TKey : notnull
        {
            return DictionaryCloner<TKey, TValue>.Clone(source, new (ObjectReferencesCache.Create(preserveObjectReferences)));
        }
    }
}
