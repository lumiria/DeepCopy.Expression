#if NET8_0_OR_GREATER
#nullable enable
using System.Collections.Immutable;
using System.Linq.Expressions;
using DeepCopy.Internal.FixedCloners.Core;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class ImmutableSortedSetCloner
    {
        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression context)
            => ImmutableSetCloneExpressionBuilder.Create(
                    typeof(ImmutableSortedSet),
                    ComparerCloneExpressionBuilder.Build
                ).Build(source, destination, context);
    }
}
#endif
