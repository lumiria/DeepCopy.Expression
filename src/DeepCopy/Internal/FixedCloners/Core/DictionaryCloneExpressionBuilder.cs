#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners.Core
{
    internal static class DictionaryCloneExpressionBuilder
    {
        public static Expression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
            var dictionaryType = source.Type;
            var genericArguments = dictionaryType.GetGenericArguments();
            var builderType = typeof(DictionaryCloneExpressionBuilder<,,>).MakeGenericType([dictionaryType, .. genericArguments]);

            var method = builderType.GetMethod(
                nameof(DictionaryCloneExpressionBuilder<,,>.Build),
                BindingFlags.Public | BindingFlags.Static)!;

            return (Expression)method.Invoke(
                null,
                [source, destination, context])!;
        }
    }

    internal static class DictionaryCloneExpressionBuilder<TDictionary, TKey, TValue>
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull 
    {
        static readonly Func<Expression, Expression, Expression> _clone;

        static readonly TypeValue _keyType;
        static readonly TypeValue _valueType;

        static readonly Type _comparerType;

        static DictionaryCloneExpressionBuilder()
        {
            _keyType = TypeUtils.IsUnmanagedType<TKey>()
                ? TypeValue.Value
                : GetTypeValue(typeof(TKey));
            _valueType = TypeUtils.IsUnmanagedType<TValue>()
                ? TypeValue.Value
                : GetTypeValue(typeof(TValue));

            _comparerType = typeof(TDictionary).GetProperty("Comparer")!.PropertyType;

            _clone ??= (_keyType, _valueType) switch
            {
                (TypeValue.Value, TypeValue.Value) => CloneValueSemanticsDictionary,
                _ => Clone,
            };
        }

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
            return Expression.Block(
                Expression.Assign(
                    destination,
                    _clone(source, context)
                )
            );
        }

        private static Expression CloneValueSemanticsDictionary(Expression source, Expression context)
        {
            var type = typeof(TDictionary);
            var ctor = type.GetConstructor(new[]
            {
                typeof(IDictionary<TKey, TValue>),
                _comparerType,
            });

            return Expression.New(
                ctor!,
                source,
                Expression.Constant(null, _comparerType));
        }

        private static Expression Clone(Expression source, Expression context)
        {
            var type = typeof(TDictionary);
            var dictionary = Expression.Variable(type, "dict");
            var assignDictionary = Expression.Assign(
                dictionary,
                New(source, context)
            );

            return Expression.Block(
                [dictionary],
                assignDictionary,
                CoreDictionaryCloneExpressionBuilder<TDictionary, TKey, TValue>.Build(
                    source, dictionary, context),
                dictionary
            );
        }

        private static Expression New(Expression source, Expression context)
        {
            var comparer = Expression.Property(source, "Comparer");
            var defaultComparer = GetDefaultComparer();
            var cloneMethod = typeof(ObjectCloner)
                .GetMethod(nameof(ObjectCloner._Clone), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(comparer.Type);

            var comparerArg = Expression.Condition(
                Expression.ReferenceEqual(
                    comparer,
                    defaultComparer),
                defaultComparer,
                Expression.Call(
                    cloneMethod,
                    comparer,
                    context));

            var ctor = typeof(TDictionary).GetConstructor(new[]
            {
                _comparerType,
            });

            return Expression.New(
                ctor!,
                comparerArg);

            static UnaryExpression GetDefaultComparer() =>
                Expression.Convert(
                    _comparerType == typeof(IComparer<TKey>)
                        ? Expression.Property(
                            null,
                            typeof(Comparer<TKey>),
                            nameof(Comparer<TKey>.Default))
                        : Expression.Property(
                            null,
                            typeof(EqualityComparer<TKey>),
                            nameof(EqualityComparer<TKey>.Default)),
                    _comparerType);
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

        internal enum TypeValue : uint
        {
            Default = 0,
            Object = 1,
            Array = 2,
            Sealed = 3,
            Value = 4,
        }
    }
}
