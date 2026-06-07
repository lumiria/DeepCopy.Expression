#nullable enable

using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace DeepCopy.Internal.FixedCloners.Core
{
    internal static class EnumerableCloneExpressionBuilder
    {
        public static MethodCallExpression Build(
            Type genericArgumentType,
            Expression source,
            Expression context)
        {
            var enumerator = Expression.Call(source, nameof(IEnumerable.GetEnumerator), Type.EmptyTypes);

            var item = Expression.Parameter(genericArgumentType, "x");

            var invokeClone = CloneExpressionBuilder.Build(item, context);

            var selectorFuncType = typeof(Func<,>).MakeGenericType(genericArgumentType, genericArgumentType);
            var selector = Expression.Lambda(selectorFuncType, invokeClone, item);

            var selectMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == nameof(Enumerable.Select) && m.GetParameters().Length == 2)
                .MakeGenericMethod(genericArgumentType, genericArgumentType);

            var selectCall = Expression.Call(selectMethod, source, selector);
            return selectCall;
        }
    }
}
