#if NET10_0_OR_GREATER
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
                    null, createBuilderMethod, keyComparerProperty, valueComparerProperty));

            var toImmuutableMethod = assignBuilder.Type.GetMethod(
                nameof(ImmutableDictionary<,>.Builder.ToImmutable),
                BindingFlags.Instance | BindingFlags.Public)!;

            var assignDestination = Expression.Assign(
                destination,
                Expression.Call(builder, toImmuutableMethod));

            return Expression.Block(
                [builder],
                assignBuilder,
                CloneDictionary(source, builder, context),
                assignDestination);
        }

        private static Expression CloneDictionary(
            Expression source,
            Expression destination,
            Expression context)
        {
            var dictionaryType = destination.Type;
            var genericArguments = dictionaryType.GetGenericArguments();
            var clonerType = typeof(CoreDictionaryCloneExpressionBuilder<,,>)
                .MakeGenericType([dictionaryType, .. genericArguments]);

            var method = clonerType.GetMethod(
                nameof(CoreDictionaryCloneExpressionBuilder<,,>.Build),
                BindingFlags.Public | BindingFlags.Static)!;

            return (Expression)method.Invoke(
                null,
                [source, destination, context])!;
        }
    }
}
#endif
