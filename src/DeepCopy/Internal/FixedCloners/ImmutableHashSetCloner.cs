#if NET8_0_OR_GREATER
#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using DeepCopy.Internal.FixedCloners.Core;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class ImmutableHashSetCloner
    {
        private static MethodInfo? _createMethod;

        private static MethodInfo CreateMethod
            => _createMethod ??= typeof(ImmutableHashSet)
                        .GetMethods()
                        .First(m =>
                            m.Name == nameof(ImmutableHashSet.Create) &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[1].ParameterType.IsArray);

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression cache)
        {
            var (method, elementType) = GetCloneMethod(source.Type);
            var comparer = Expression.Property(source, nameof(ImmutableHashSet<>.KeyComparer));

            var toArrayMethod = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.ToArray))
                !.MakeGenericMethod(elementType);
            var toArrayCall = Expression.Call(toArrayMethod, source);

            return TypeUtils.IsAssignableType(elementType)
                ? Expression.Block(
                    Expression.Assign(
                        destination,
                        Expression.Call(method, comparer, toArrayCall))
                )
                : Expression.Block(
                    Expression.Assign(
                        destination,
                        Expression.Call(
                            method,
                            comparer,
                            Expression.Call(toArrayMethod, EnumerableCloneExpressionBuilder.Build(elementType, source, cache))))
                );
        }

        private static (MethodInfo cloneMethod, Type elementType) GetCloneMethod(Type type)
        {
            var argTypes = type.GetGenericArguments()!;

            var method = CreateMethod;
            return (method.MakeGenericMethod(argTypes), argTypes[0]);
        }
    }
}
#endif
