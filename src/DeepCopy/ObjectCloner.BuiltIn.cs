#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));
            var instance = DictionaryCloner<TKey, TValue>.Clone(source, context);
            context.Flush();

            return instance;
        }

        internal static T CloneKey<T>(T source, DeepCopyContext context)
            where T : notnull
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            var type = source.GetType();
            if (context.Cache.TryGetOrCache(type, source, out T instance)) return instance;

            context.LockQeueue();
            _CopyTo(type, source, ref instance, context);
            context.UnlockQeueue();

            context.Cache.RemoveLatest();

            return instance;
        }

        internal static object CloneKey(object source, DeepCopyContext context)
        {
            var type = source.GetType();
#if DEBUGLOG
            Console.WriteLine($"[{typeof(object).Name} >> {type.Name}]");
#endif

#if NETSTANDARD2_0
            if (type == typeof(object)) return new object();
#endif
            if (type == typeof(string)) return source;

            if (!type.IsValueType && context.Cache.Get(source, out var obj)) return obj;

#if NETSTANDARD2_0
            var instance = FormatterServices.GetUninitializedObject(type);
#else
            var instance = RuntimeHelpers.GetUninitializedObject(type);
#endif

            context.LockQeueue();
            if (type.IsValueType)
            {
                _CopyValueType(type, source, ref instance, context);
            }
            else
            {
                context.Cache.Add(source, instance);
                _CopyTo(type, source, ref instance, context);
                context.Cache.RemoveLatest();
            }
            context.UnlockQeueue();

            return instance;
        }

        internal static T CloneKeyAs<T>(T source, DeepCopyContext context)
            where T : notnull
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (context.Cache.TryGetOrCache(typeof(T), source, out T instance)) return instance;

            context.LockQeueue();
            _CopyToAs(source, ref instance, context);
            context.UnlockQeueue();

            context.Cache.RemoveLatest();

            return instance;
        }
    }
}
