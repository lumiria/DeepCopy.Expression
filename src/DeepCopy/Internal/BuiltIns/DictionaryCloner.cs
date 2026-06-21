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
            var keyType = TypeUtils.IsUnmanagedType<TKey>()
                ? TypeValue.Value
                : GetTypeValue(typeof(TKey));
            var valueType = TypeUtils.IsUnmanagedType<TValue>()
                ? TypeValue.Value
                : GetTypeValue(typeof(TValue));

            if (keyType == TypeValue.Array)
                _arrayKeyCloner = ArrayCloneDelegateGenerator.GetOrCreateDelegate<Func<TKey, DeepCopyContext, TKey>>(typeof(TKey));
            if (valueType == TypeValue.Array)
                _arrayValueCloner = ArrayCloneDelegateGenerator.GetOrCreateDelegate<Func<TValue, DeepCopyContext, TValue>>(typeof(TValue));

            _clone ??= (keyType, valueType) switch
            {
                (TypeValue.Value, TypeValue.Value) => CloneValueSemanticsDictionary,
                (TypeValue.Value, TypeValue.Default) => CloneValueSemanticsKeyDictionary,
                (TypeValue.Value, TypeValue.Object) => CloneValueSemanticsKeyAndObjectValueDictionary,
                (TypeValue.Value, TypeValue.Array) => CloneValueSemanticsKeyAndArrayValueDictionary,
                (TypeValue.Value, TypeValue.Sealed) => CloneValueSemanticsKeyAndSealedValueDictionary,
                (TypeValue.Default, TypeValue.Value) => CloneValueSemanticsValueDictionary,
                (TypeValue.Object, TypeValue.Value) => CloneValueSemanticsValueAndObjectKeyDictionary,
                (TypeValue.Array, TypeValue.Value) => CloneValueSemanticsValueAndArrayKeyDictionary,
                (TypeValue.Sealed, TypeValue.Value) => CloneValueSemanticsValueAndSealedKeyDictionary,
                (TypeValue.Object, TypeValue.Default) => CloneObjectKeyDictionary,
                (TypeValue.Object, TypeValue.Object) => CloneObjectKeyValueDictionary,
                (TypeValue.Object, TypeValue.Array) => CloneObjectKeyArrayValueDictionary,
                (TypeValue.Object, TypeValue.Sealed) => CloneObjectKeySealedValueDictionary,
                (TypeValue.Array, TypeValue.Default) => CloneArrayKeyDictionary,
                (TypeValue.Array, TypeValue.Object) => CloneArrayKeyObjectValueDictionary,
                (TypeValue.Array, TypeValue.Array) => CloneArrayKeyValueDictionary,
                (TypeValue.Array, TypeValue.Sealed) => CloneArrayKeySealedValueDictionary,
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

        private static Dictionary<TKey, TValue> CloneValueSemanticsKeyAndSealedValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Value");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(item.Key, ObjectCloner._CloneAs<TValue>(item.Value, context));
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
                dict.Add(ObjectCloner.CloneKey(item.Key, context), item.Value);
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
                dict.Add((TKey)ObjectCloner.CloneKey((object)item.Key, context), item.Value);
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

        private static Dictionary<TKey, TValue> CloneValueSemanticsValueAndSealedKeyDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone ArrayKey");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(ObjectCloner.CloneKeyAs(item.Key, context), item.Value);
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
                dict.Add((TKey)ObjectCloner.CloneKey((object)item.Key, context), ObjectCloner._Clone(item.Value, context));
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
                dict.Add((TKey)ObjectCloner.CloneKey((object)item.Key, context), (TValue)ObjectCloner._CloneObject(item.Value, context));
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
                dict.Add((TKey)ObjectCloner.CloneKey((object)item.Key, context), item.Value != null ? _arrayValueCloner!(item.Value, context) : item.Value);
            }

            return dict;
        }

        private static Dictionary<TKey, TValue> CloneObjectKeySealedValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Object Key / Sealed Value)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add((TKey)ObjectCloner.CloneKey((object)item.Key, context), (TValue)ObjectCloner._CloneAs(item.Value, context));
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

        private static Dictionary<TKey, TValue> CloneArrayKeySealedValueDictionary(Dictionary<TKey, TValue> source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Dictionary<TKey, TValue>).Name}] | Clone Both (Array Key/ Sealed Value)");
#endif
            var dict = Create(source, context);
            foreach (var item in source)
            {
                dict.Add(_arrayKeyCloner!(item.Key, context), (TValue)ObjectCloner._CloneAs(item.Value, context));
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
                dict.Add(ObjectCloner.CloneKey(item.Key, context), item.Value != null ? ObjectCloner._Clone(item.Value, context) : item.Value);
            }

            return dict;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TypeValue GetTypeValue(Type type) =>
            type switch
            {
                _ when type == typeof(object) => TypeValue.Object,
                _ when type.IsArray => TypeValue.Array,
                _ when type.IsSealed => TypeValue.Sealed,
                _ => TypeValue.Default,
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<TKey, TValue> Create(Dictionary<TKey, TValue> source, DeepCopyContext context) =>
            new(source.Count,
                source.Comparer == EqualityComparer<TKey>.Default
                    ? (IEqualityComparer<TKey>)EqualityComparer<TKey>.Default
                    : ObjectCloner._Clone(source.Comparer, context));

        internal enum TypeValue : uint
        {
            Default = 0,
            Object = 1,
            Array = 2,
            Sealed = 3,
            Value = 4,
        }
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
