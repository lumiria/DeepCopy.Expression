#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.BuiltIns
{
    internal static class DictionaryCloner<TKey, TValue>
        where TKey : notnull
    {
        static readonly Func<Dictionary<TKey, TValue>, DeepCopyContext, Dictionary<TKey, TValue>> _clone;
        static readonly Func<TKey, DeepCopyContext, TKey>? _arrayKeyCloner;
        static readonly Func<TValue, DeepCopyContext, TValue>? _arrayValueCloner;

        static DictionaryCloner()
        {
            var key = TypeUtils.IsUnmanagedType<TKey>();
            var value = TypeUtils.IsUnmanagedType<TValue>();
            var keyType = GetTypeValue(typeof(TKey));
            var valueType = GetTypeValue(typeof(TValue));

            if (keyType == 2)
                _arrayKeyCloner = ArrayCloneDelegateGenerator.GetOrCreateDelegate<Func<TKey, DeepCopyContext, TKey>>(typeof(TKey));
            if (valueType == 2)
                _arrayValueCloner = ArrayCloneDelegateGenerator.GetOrCreateDelegate<Func<TValue, DeepCopyContext, TValue>>(typeof(TValue));

            _clone ??= (key, value, keyType, valueType) switch
            {
                (true, true, _, _) => CloneValueSemanticsDictionary,
                (true, false, _, 0) => CloneValueSemanticsKeyDictionary,
                (true, false, _, 1) => CloneValueSemanticsKeyAndObjectValueDictionary,
                (true, false, _, 2) => CloneValueSemanticsKeyAndArrayValueDictionary,
                (false, true, 0, _) => CloneValueSemanticsValueDictionary,
                (false, true, 1, _) => CloneValueSemanticsValueAndObjectKeyDictionary,
                (false, true, 2, _) => CloneValueSemanticsValueAndArrayKeyDictionary,
                (false, false, 1, 0) => CloneObjectKeyDictionary,
                (false, false, 1, 1) => CloneObjectKeyValueDictionary,
                (false, false, 1, 2) => CloneObjectKeyArrayValueDictionary,
                (false, false, 2, 0) => CloneArrayKeyDictionary,
                (false, false, 2, 1) => CloneArrayKeyObjectValueDictionary,
                (false, false, 2, 2) => CloneArrayKeyValueDictionary,
                _ => CloneDictionary,
            };
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<TBD>")]
#if NETSTANDARD2_0
        public static Dictionary<TKey, TValue>? Clone(Dictionary<TKey, TValue>? source, DeepCopyContext context)
#else
        [return: NotNullIfNotNull(nameof(source))]
        public static Dictionary<TKey, TValue>? Clone(Dictionary<TKey, TValue>? source, DeepCopyContext context)
#endif
        {
            if (source is null) return null;
            if (context.Cache.Get(source, out var obj)) return obj;

            return _clone(source, context)!;
        }

        private static Dictionary<TKey, TValue> CloneValueSemanticsDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone");
#endif
            return new(source);
        }

        private static Dictionary<TKey, TValue> CloneValueSemanticsKeyDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Value");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(item.Key, ObjectCloner._Clone<TValue>(item.Value, context));
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneValueSemanticsKeyAndObjectValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone ObjectValue");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(item.Key, (TValue)ObjectCloner._CloneObject(item.Value, context));
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneValueSemanticsKeyAndArrayValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone ArrayValue");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(item.Key, item.Value != null ? _arrayValueCloner!(item.Value, context) : item.Value);
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneValueSemanticsValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Key");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(ObjectCloner._Clone(item.Key, context), item.Value);
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneValueSemanticsValueAndObjectKeyDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Key");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add((TKey)ObjectCloner._CloneObject(item.Key, context), item.Value);
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneValueSemanticsValueAndArrayKeyDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone ArrayKey");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(_arrayKeyCloner!(item.Key, context), item.Value);
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneObjectKeyDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Object Key)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add((TKey)ObjectCloner._CloneObject(item.Key, context), ObjectCloner._Clone(item.Value, context));
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneObjectKeyValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Object Key/Value)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add((TKey)ObjectCloner._CloneObject(item.Key, context), (TValue)ObjectCloner._CloneObject(item.Value, context));
            }

            return dict;
        }


        private static Dictionary<TKey, TValue> CloneObjectKeyArrayValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Object Key / Array Value)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add((TKey)ObjectCloner._CloneObject(item.Key, context), item.Value != null ? _arrayValueCloner!(item.Value, context) : item.Value);
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneArrayKeyDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Array Key)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(_arrayKeyCloner!(item.Key, context), (TValue)ObjectCloner._Clone(item.Value, context));
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneArrayKeyObjectValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Array Key / Object Value)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(_arrayKeyCloner!(item.Key, context), (TValue)ObjectCloner._CloneObject(item.Value, context));
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneArrayKeyValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Array Key/Value)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(_arrayKeyCloner!(item.Key, context), item.Value != null ? _arrayValueCloner!(item.Value, context) : item.Value);
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(ObjectCloner._Clone(item.Key, context), item.Value != null ? ObjectCloner._Clone(item.Value, context) : item.Value);
            }

            return dict;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetTypeValue(Type type) =>
            type switch
            {
                _ when type == typeof(object) => 1,
                _ when type.IsArray => 2,
                _ => 0
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<TKey, TValue> Create(Dictionary<TKey, TValue> source, DeepCopyContext context) =>
            new(source.Count,
                source.Comparer == EqualityComparer<TKey>.Default
                    ? (IEqualityComparer<TKey>)EqualityComparer<TKey>.Default
                    : ObjectCloner._Clone(source.Comparer, context));
    }

    internal static class FixedDictionaryCloner
    {
        public static MethodInfo GetCloneMethod(Type type)
        {
            var genericType = type.GetGenericTypeDefinition();
            var argTypes = type.GetGenericArguments();

            var clonerType = typeof(DictionaryCloner<,>);
            var genericClonerType = clonerType.MakeGenericType(argTypes);

            return genericClonerType.GetMethod(nameof(DictionaryCloner<,>.Clone))!;
        }

        public static BlockExpression Build(Expression source, Expression destination, Expression context)
        {
            var method = GetCloneMethod(source.Type);

            return Expression.Block(
                Expression.Assign(
                    destination,
                    Expression.Call(null, method, source, context)
                )
            );
        }

        public static void Compile(Type type)
        {
            var genericType = type.GetGenericTypeDefinition();
            var argTypes = type.GetGenericArguments();

            var clonerType = typeof(DictionaryCloner<,>);
            var genericClonerType = clonerType.MakeGenericType(argTypes);

            var method = genericClonerType.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Static);
            method?.Invoke(null, null);
        }
    }
}
