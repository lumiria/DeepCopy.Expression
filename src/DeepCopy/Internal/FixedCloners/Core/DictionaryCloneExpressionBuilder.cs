using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            return (Expression)method.Invoke(
                null,
                [source, destination, context]);
        }
    }

    internal static class DictionaryCloneExpressionBuilder<TDictionary, TKey, TValue>
        where TDictionary : IDictionary<TKey, TValue>, new()
        where TKey : notnull 
    {
        static readonly Func<Expression, Expression, Expression> _clone;
        static readonly Func<TKey, DeepCopyContext, TKey>? _arrayKeyCloner;
        static readonly Func<TValue, DeepCopyContext, TValue>? _arrayValueCloner;

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

            if (_keyType == TypeValue.Array)
                _arrayKeyCloner = ArrayCloneDelegateGenerator.GetOrCreateDelegate<Func<TKey, DeepCopyContext, TKey>>(typeof(TKey));
            if (_valueType == TypeValue.Array)
                _arrayValueCloner = ArrayCloneDelegateGenerator.GetOrCreateDelegate<Func<TValue, DeepCopyContext, TValue>>(typeof(TValue));

            _comparerType =  typeof(TDictionary).GetProperty("Comparer").PropertyType;

            _clone ??= (_keyType, _valueType) switch
            {
                (TypeValue.Value, TypeValue.Value) => CloneValueSemanticsDictionary,
                _ => CloneValueSemanticsKeyDictionary,
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TypeValue GetTypeValue(Type type) =>
            type switch
            {
                _ when type == typeof(object) => TypeValue.Object,
                _ when type.IsArray => TypeValue.Array,
                _ when type.IsSealed => TypeValue.Sealed,
                _ => TypeValue.Default,
            };

        private static Expression CloneValueSemanticsDictionary(Expression source, Expression context)
        {
            var type = typeof(TDictionary);
            var ctor = type.GetConstructor(new[]
            {
                typeof(IDictionary<TKey, TValue>),
                _comparerType,
            });

            return Expression.New(
                ctor,
                source,
                Expression.Constant(null, _comparerType));
        }

        private static Expression CloneValueSemanticsKeyDictionary(Expression source, Expression context)
        {
            var type = typeof(TDictionary);

            var getEnumeratorMethod = type.GetMethod(nameof(IEnumerable.GetEnumerator))
                    ?? type.GetMethod("GetEnumerator")
                    ?? throw new InvalidOperationException($"Type {type} does not have a GetEnumerator method.");

            var enumeratorType = getEnumeratorMethod.ReturnType;

            var moveNextMethod = enumeratorType.GetMethod(nameof(IEnumerator.MoveNext))
                ?? throw new InvalidOperationException($"Enumerator type {enumeratorType} does not have a MoveNext method.");
            var currentProperty = enumeratorType.GetProperty(nameof(IEnumerator.Current))
                ?? throw new InvalidOperationException($"Enumerator type {enumeratorType} does not have a Current property.");
            var disposeMethod = typeof(IDisposable).IsAssignableFrom(enumeratorType)
                ? enumeratorType.GetMethod(nameof(IDisposable.Dispose))
                : null;

            var valueProperty = typeof(KeyValuePair<TKey, TValue>)
                .GetProperty(nameof(KeyValuePair<TKey, TValue>.Value))!;

            var keyProperty = typeof(KeyValuePair<TKey, TValue>)
                .GetProperty(nameof(KeyValuePair<TKey, TValue>.Key))!;

            var addMethod = typeof(TDictionary).GetMethod(
                nameof(IDictionary<TKey, TValue>.Add),
                [typeof(TKey), typeof(TValue)])
                ?? throw new InvalidOperationException($"Type {type} does not have an Add method that takes TKey and TValue.");

            var cloneMethod = typeof(ObjectCloner)
                .GetMethod(nameof(ObjectCloner._Clone), BindingFlags.Static | BindingFlags.NonPublic);

            var cloneAsMethod = typeof(ObjectCloner)
                .GetMethod(nameof(ObjectCloner._CloneAs), BindingFlags.Static | BindingFlags.NonPublic);

            var cloneObjectMethod = typeof(ObjectCloner)
                .GetMethod(nameof(ObjectCloner._CloneObject), BindingFlags.Static | BindingFlags.NonPublic);

            var cloneKeyMethod = typeof(ObjectCloner)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Single(m => m.Name == nameof(ObjectCloner.CloneKey) &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2);

            var dictionary = Expression.Variable(type, "dict");
            var enumerator = Expression.Variable(enumeratorType, "enumerator");
            var item = Expression.Variable(typeof(KeyValuePair<TKey, TValue>), "item");

            var assignDictionary = Expression.Assign(
                dictionary,
                New(source, context)
            );

            var assignEnumerator = Expression.Assign(
                enumerator,
                Expression.Call(source, getEnumeratorMethod)
            );

            var assignItem = Expression.Assign(
                item,
                Expression.Property(enumerator, currentProperty)
            );

            Expression keyClone = BuildCloneKeyExpression(item, context);
            Expression valueClone = BuildValueExpression(item, context);

            var addCall = Expression.Call(
                dictionary,
                addMethod,
                keyClone,
                valueClone
            );

            var breakLabel = Expression.Label();

            return Expression.Block(
                [dictionary, enumerator, item],
                assignDictionary,
                assignEnumerator,
                Expression.TryFinally(
                    Expression.Loop(
                        Expression.IfThenElse(
                            Expression.Call(enumerator, moveNextMethod),
                            Expression.Block(
                                assignItem,
                                addCall
                            ),
                            Expression.Break(breakLabel)
                        ),
                        breakLabel
                    ),
                    disposeMethod != null
                        ? Expression.Call(enumerator, disposeMethod)
                        : (Expression)Expression.Empty()
                ),
                dictionary
            );
        }

        private static Expression New(Expression source, Expression context)
        {
            var comparer = Expression.Property(source, "Comparer");
            var defaultComparer = GetDefaultComparer();
            var cloneMethod = typeof(ObjectCloner)
                .GetMethod(nameof(ObjectCloner._Clone), BindingFlags.Static | BindingFlags.NonPublic)
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
                ctor,
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

        private static Expression BuildCloneKeyExpression(ParameterExpression item, Expression context)
        {
            var keyProperty = typeof(KeyValuePair<TKey, TValue>)
                .GetProperty(nameof(KeyValuePair<TKey, TValue>.Key))!;

            return _keyType switch
            {
                TypeValue.Default => Expression.Call(
                    typeof(ObjectCloner)
                        .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                        .Single(m => m.Name == nameof(ObjectCloner.CloneKey) &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(TKey)),
                    Expression.Property(item, keyProperty),
                    context
                ),
                TypeValue.Sealed => Expression.Call(
                    typeof(ObjectCloner)
                        .GetMethod(nameof(ObjectCloner.CloneKeyAs), BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(typeof(TKey)),
                    Expression.Property(item, keyProperty),
                    context
                ),
                TypeValue.Object => Expression.Call(
                    typeof(ObjectCloner)
                        .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                        .Single(m => m.Name == nameof(ObjectCloner.CloneKey) &&
                            !m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2),
                    Expression.Property(item, keyProperty),
                    context
                ),
                TypeValue.Array => Expression.Invoke(
                    Expression.Constant(_arrayKeyCloner),
                    Expression.Property(item, keyProperty),
                    context
                ),
                _ => Expression.Property(item, keyProperty),
            };
        }

        private static Expression BuildValueExpression(ParameterExpression item, Expression context)
        {
            var valueProperty = typeof(KeyValuePair<TKey, TValue>)
                .GetProperty(nameof(KeyValuePair<TKey, TValue>.Value))!;

            return _valueType switch
            {
                TypeValue.Default => Expression.Call(
                     typeof(ObjectCloner)
                        .GetMethod(nameof(ObjectCloner._Clone), BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(typeof(TValue)),
                    Expression.Property(item, valueProperty),
                    context
                ),
                TypeValue.Sealed => Expression.Call(
                    typeof(ObjectCloner)
                        .GetMethod(nameof(ObjectCloner._CloneAs), BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(typeof(TValue)),
                    Expression.Property(item, valueProperty),
                    context
                ),
                TypeValue.Object => Expression.Call(
                    typeof(ObjectCloner)
                        .GetMethod(nameof(ObjectCloner._CloneObject), BindingFlags.Static | BindingFlags.NonPublic),
                    Expression.Property(item, valueProperty),
                    context
                ),
                TypeValue.Array => Expression.Invoke(
                    Expression.Constant(_arrayValueCloner),
                    Expression.Property(item, valueProperty),
                    context
                ),
                _ => Expression.Property(item, valueProperty),
            };
        }

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
