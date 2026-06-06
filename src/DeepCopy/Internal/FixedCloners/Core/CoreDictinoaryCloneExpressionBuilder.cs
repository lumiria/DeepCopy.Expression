#nullable enable

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
    internal static class CoreDictionaryCloneExpressionBuilder<TDictionary, TKey, TValue>
        where TDictionary : IDictionary<TKey, TValue>
        where TKey : notnull
    {
        static readonly Func<Expression, Expression, Expression, Expression> _clone;
        static readonly Func<TKey, DeepCopyContext, TKey>? _arrayKeyCloner;
        static readonly Func<TValue, DeepCopyContext, TValue>? _arrayValueCloner;

        static readonly TypeValue _keyType;
        static readonly TypeValue _valueType;

        static CoreDictionaryCloneExpressionBuilder()
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

            _clone = Clone;
        }

        public static Expression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
            return _clone(source, destination, context);
        }


        private static Expression Clone(Expression source, Expression destination, Expression context)
        {
            var type = source.Type;

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

            var addMethod = typeof(TDictionary).GetMethod(
                nameof(IDictionary<TKey, TValue>.Add),
                [typeof(TKey), typeof(TValue)])
                ?? throw new InvalidOperationException($"Type {type} does not have an Add method that takes TKey and TValue.");

            var enumerator = Expression.Variable(enumeratorType, "enumerator");
            var item = Expression.Variable(typeof(KeyValuePair<TKey, TValue>), "item");

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
                destination,
                addMethod,
                keyClone,
                valueClone
            );

            var breakLabel = Expression.Label();

            return Expression.Block(
                [enumerator, item],
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
                )
            );
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
                        .GetMethod(nameof(ObjectCloner.CloneKeyAs), BindingFlags.Static | BindingFlags.NonPublic)!
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
                        .GetMethod(nameof(ObjectCloner._Clone), BindingFlags.Static | BindingFlags.NonPublic)!
                        .MakeGenericMethod(typeof(TValue)),
                    Expression.Property(item, valueProperty),
                    context
                ),
                TypeValue.Sealed => Expression.Call(
                    typeof(ObjectCloner)
                        .GetMethod(nameof(ObjectCloner._CloneAs), BindingFlags.Static | BindingFlags.NonPublic)!
                        .MakeGenericMethod(typeof(TValue)),
                    Expression.Property(item, valueProperty),
                    context
                ),
                TypeValue.Object => Expression.Call(
                    typeof(ObjectCloner)
                        .GetMethod(nameof(ObjectCloner._CloneObject), BindingFlags.Static | BindingFlags.NonPublic)!,
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
