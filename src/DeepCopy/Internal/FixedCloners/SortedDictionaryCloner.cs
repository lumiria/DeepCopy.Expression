using System.Collections.Generic;
using System.Linq.Expressions;
using DeepCopy.Internal.FixedCloners.Core;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class SortedDictionaryCloner
    {
        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
            return Expression.Block(
                DictionaryCloneExpressionBuilder.Build(
                    source,
                    destination,
                    context
                )
            );
        }
    }
}
