#nullable enable

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners.Core
{
    internal static class EqualityComparerCloneExpressionBuilder
    {
        public static ConditionalExpression Build(
            Type type,
            Expression sourceComparer,
            Expression context)
        {
            var genericType = typeof(EqualityComparer<>);
            var comparerType = genericType.MakeGenericType(type);
            var defaultComparer = Expression.Property(null, comparerType, nameof(EqualityComparer<>.Default));

            var condition = Expression.Equal(sourceComparer, defaultComparer);

            var iComparerType = typeof(IEqualityComparer<>).MakeGenericType(type);
            var trueExpression = Expression.Convert(defaultComparer, iComparerType);

            var cloneMethod = ReflectionUtils.ObjectClone.MakeGenericMethod(sourceComparer.Type);

            var falseExpression = Expression.Call(cloneMethod, sourceComparer, context);

            return Expression.Condition(
                condition,
                trueExpression,
                falseExpression);
        }
    }
}
