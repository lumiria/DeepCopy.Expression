#if NET8_0_OR_GREATER
#nullable enable

using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.FixedCloners.Core;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class FrozenDictinoaryCloner
    {
        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
            var dictionaryType = source.Type;
            var genericArguments = dictionaryType.GetGenericArguments()[^2 ..];

            var ctor = typeof(Dictionary<,>).MakeGenericType(genericArguments)
                .GetConstructor(
                [
                    typeof(int),
                    typeof(IEqualityComparer<>).MakeGenericType(genericArguments[0])
                ]);

            var countProperty = Expression.Property(source, nameof(FrozenDictionary<,>.Count));
            var comparerProperty = Expression.Property(source, nameof(FrozenDictionary<,>.Comparer));

            var newDictionary = Expression.New(
                ctor!,
                countProperty,
                EqualityComparerCloneExpressionBuilder.Build(
                    genericArguments[0], comparerProperty, context)
            );

            var dictionary = Expression.Variable(
                typeof(Dictionary<,>).MakeGenericType(genericArguments),
                "dictionary");

            var assignDictionary = Expression.Assign(
                dictionary,
                newDictionary);

            var toFrozeneMethod = typeof(FrozenDictionary).GetMethods(
                    BindingFlags.Static | BindingFlags.Public
                ).Single(m => m.Name == nameof(FrozenDictionary.ToFrozenDictionary) &&
                    m.GetParameters().Length == 2)
                .MakeGenericMethod(genericArguments);

            var nullComparer = Expression.Constant(
                null,
                typeof(IEqualityComparer<>).MakeGenericType(genericArguments[0]));

            var assignDestination = Expression.Assign(
                destination,
                Expression.Call(null, toFrozeneMethod, dictionary, nullComparer));

            return Expression.Block(
                [dictionary],
                assignDictionary,
                CoreDictionaryCloneExpressionBuilder.Build(source, dictionary, context),
                assignDestination);
        }
    }
}
#endif
