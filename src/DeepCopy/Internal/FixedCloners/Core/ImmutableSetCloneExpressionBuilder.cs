#if NET8_0_OR_GREATER
#nullable enable
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners.Core
{
    internal class ImmutableSetCloneExpressionBuilder
    {
        private readonly Type _immutableCollectionType;
        private readonly Func<Type, Expression, Expression, Expression> _buildComparer;

        private ImmutableSetCloneExpressionBuilder(
            Type immutableCollectionType,
            Func<Type, Expression, Expression, Expression> buildComparer)
        {
            _immutableCollectionType = immutableCollectionType;
            _buildComparer = buildComparer;
        }

        private MethodInfo? _createMethod;

        private MethodInfo CreateMethod
            => _createMethod ??= _immutableCollectionType
                        .GetMethods()
                        .First(m =>
                            m.Name == "Create" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 2 &&
                            m.GetParameters()[1].ParameterType.IsArray);

        public static ImmutableSetCloneExpressionBuilder Create(
            Type immutableCollectionType,
            Func<Type, Expression, Expression, Expression> buildComparer)
            => new (immutableCollectionType, buildComparer);

        public BlockExpression Build(
            Expression source,
            Expression destination,
            Expression cache)
        {
            var (method, elementType) = GetCloneMethod(source.Type);
            var comparer = Expression.Property(source, "KeyComparer");
            var clonedComparer = _buildComparer(elementType, comparer, cache);

            var toArrayMethod = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.ToArray))
                !.MakeGenericMethod(elementType);
            var toArrayCall = Expression.Call(toArrayMethod, source);

            var replaceCacheMethod = typeof(ObjectReferencesCache)
                .GetMethod(nameof(ObjectReferencesCache.ReplaceLatest))
                !.MakeGenericMethod(source.Type);

            var replaceCache = ExpressionUtils.NullCheck(
                source, Expression.Call(cache, replaceCacheMethod, source, destination));

            return TypeUtils.IsAssignableType(elementType)
                ? Expression.Block(
                    Expression.Assign(
                        destination,
                        Expression.Call(method, clonedComparer, toArrayCall))
                )
                : Expression.Block(
                    Expression.Assign(
                        destination,
                        Expression.Call(
                            method,
                            clonedComparer,
                            Expression.Call(toArrayMethod, EnumerableCloneExpressionBuilder.Build(elementType, source, cache)))),
                    replaceCache
                );
        }

        private (MethodInfo cloneMethod, Type elementType) GetCloneMethod(Type type)
        {
            var argTypes = type.GetGenericArguments()!;

            var method = CreateMethod;
            return (method.MakeGenericMethod(argTypes), argTypes[0]);
        }
    }
}
#endif
