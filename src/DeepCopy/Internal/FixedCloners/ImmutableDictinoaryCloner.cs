#if NET8_0_OR_GREATER
#nullable enable

using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.FixedCloners.Core;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class ImmutableDictinoaryCloner
    {
        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
            var dictionaryType = source.Type;
            var genericArguments = dictionaryType.GetGenericArguments();

            var createBuilderMethod = typeof(ImmutableDictionary).GetMethods(
                    BindingFlags.Static | BindingFlags.Public
                ).Single(m => m.Name == nameof(ImmutableDictionary.CreateBuilder) &&
                    m.GetParameters().Length == 2)
                .MakeGenericMethod(genericArguments);

            var keyComparerProperty = Expression.Property(
                source,
                nameof(ImmutableDictionary<,>.KeyComparer));

            var valueComparerProperty = Expression.Property(
                source,
                nameof(ImmutableDictionary<,>.ValueComparer));

            var builder = Expression.Variable(
                typeof(ImmutableDictionary<,>.Builder).MakeGenericType(genericArguments),
                "builder");

            var assignBuilder = Expression.Assign(
                builder,
                Expression.Call(
                    null, createBuilderMethod,
                    EqualityComparerCloneExpressionBuilder.Build(
                        genericArguments[0], keyComparerProperty, context),
                    EqualityComparerCloneExpressionBuilder.Build(
                        genericArguments[1], valueComparerProperty, context)
                ));

            var toImmuutableMethod = assignBuilder.Type.GetMethod(
                nameof(ImmutableDictionary<,>.Builder.ToImmutable),
                BindingFlags.Instance | BindingFlags.Public)!;

            var assignDestination = Expression.Assign(
                destination,
                Expression.Call(builder, toImmuutableMethod));

            return Expression.Block(
                [builder],
                assignBuilder,
                CoreDictionaryCloneExpressionBuilder.Build(source, builder, context),
                assignDestination);
        }
    }
}
#endif
