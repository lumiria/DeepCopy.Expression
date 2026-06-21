#nullable enable

using System.Linq.Expressions;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class ReadOnlyDictionaryCloner
    {
        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
            var expression = CoreCloneExpressionGenerator.CreateCloneExpressionInner(
                destination.Type,
                source,
                destination,
                context,
#if NETSTANDARD2_0
                "m_keys", "m_values"
#else
                "_keys", "_values"
#endif
            );

            return expression is BlockExpression blockExpression
                ? blockExpression
                : Expression.Block(expression);
        }
    }
}
