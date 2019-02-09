using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal sealed class ArrayCloner
    {
        public static ArrayCloner Instance { get; } =
            new ArrayCloner();

        public Expression Build(
            CopyPolicy copyPolicy,
            Type type,
            Expression source,
            Expression destination)
        {
            var elementType = type.GetElementType();

            if (copyPolicy == CopyPolicy.DeepCopy)
            {
                if (type.GetArrayRank() > 1)
                {
                    return CreateDeepCopyRectangulerArrayExpression(
                        type, elementType, source, destination);
                }

                var length = Expression.ArrayLength(source);
                var arrayAssign = Expression.Assign(
                    destination,
                    Expression.NewArrayBounds(elementType, length));

                return CreateDeepCopyArrayExpression(
                    elementType,
                    source,
                    destination,
                    length,
                    arrayAssign);
            }

            if (copyPolicy == CopyPolicy.ShallowCopy)
            {
                return CreateShallowCopyArrayExpression(
                    type,
                    source,
                    destination);
            }

            throw new InvalidOperationException();
        }

        public Expression Build(
            CopyPolicy copyPolicy,
            Type type,
            Expression source,
            Expression destination,
            MemberInfo member)
        {
            var elementType = type.GetElementType();

            if (copyPolicy == CopyPolicy.DeepCopy)
            {
                if (type.GetArrayRank() > 1)
                {
                    return CreateDeepCopyRectangulerArrayExpression(
                        type, elementType, source, MemberAccessorGenerator.CreateGetter(destination, member));
                }

                var length = Expression.ArrayLength(source);
                var arrayAssign = MemberAccessorGenerator.CreateSetter(
                    destination,
                    member,
                    Expression.NewArrayBounds(elementType, length));

                return CreateDeepCopyArrayExpression(
                    elementType,
                    source,
                    MemberAccessorGenerator.CreateGetter(destination, member),
                    length,
                    arrayAssign);
            }

            if (copyPolicy == CopyPolicy.ShallowCopy)
            {
                return CreateShallowCopyArrayExpression(
                    type,
                    source,
                    MemberAccessorGenerator.CreateGetter(destination, member));
            }

            throw new InvalidOperationException();
        }

        private Expression CreateShallowCopyArrayExpression(
                Type type,
                Expression source,
                Expression destination) =>
            Expression.Assign(destination,
                Expression.Convert(Expression.Call(source, ReflectionUtils.CloneArray), type));

        private Expression CreateDeepCopyArrayExpression(
            Type elementType,
            Expression source,
            Expression destination,
            Expression length,
            Expression arrayAssign)
        {
            var i = Expression.Parameter(typeof(int), "i");
            var endLoop = Expression.Label("EndLoop");

            var elementAssign = elementType.IsArray
                ? Build(
                    TypeUtils.IsValueType(elementType.GetElementType())
                        ? CopyPolicy.ShallowCopy : CopyPolicy.DeepCopy,
                    elementType,
                    Expression.ArrayIndex(source, i),
                    Expression.ArrayAccess(destination, i))
                : ClassCloner.Instance.Build(
                    elementType,
                    Expression.ArrayIndex(source, i),
                    Expression.ArrayAccess(destination, i));

            return Expression.Block(
                new[] { i },
                Expression.Assign(i, Expression.Constant(0)),
                arrayAssign,
                Expression.Loop(
                    Expression.Block(
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(i, length),
                            Expression.Break(endLoop)),
                        elementAssign,
                        Expression.AddAssign(i, Expression.Constant(1))),
                    endLoop));
        }

        private Expression CreateDeepCopyRectangulerArrayExpression(
            Type type,
            Type elementType,
            Expression source,
            Expression destination)
        {
            int rank = type.GetArrayRank();
            var indexes = Enumerable.Range(0, rank)
                .Select(x => Expression.Parameter(typeof(int), $"i{x}"))
                .ToArray();

            var lengths = Enumerable.Range(0, rank)
                .Select(x => ExpressionUtils.GetArrayLength(source, x))
                .ToArray();

            var arrayAssign = Expression.Assign(
                destination,
                Expression.NewArrayBounds(elementType, lengths));

            var elementAssign = elementType.IsArray
                ? Build(
                    TypeUtils.IsValueType(elementType.GetElementType())
                        ? CopyPolicy.ShallowCopy : CopyPolicy.DeepCopy,
                    elementType,
                    Expression.ArrayIndex(source, indexes),
                    Expression.ArrayAccess(destination, indexes))
                : ClassCloner.Instance.Build(
                    elementType,
                    Expression.ArrayIndex(source, indexes),
                    Expression.ArrayAccess(destination, indexes));

            Expression Loop(int rankIndex) 
            {
                var endLoop = Expression.Label($"EndLoop{rankIndex}");
                return Expression.Block(
                    Expression.Assign(indexes[rankIndex], Expression.Constant(0)),
                    Expression.Loop(
                    Expression.Block(
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(indexes[rankIndex], lengths[rankIndex]),
                            Expression.Break(endLoop)),
                        rankIndex < rank - 1 ? Loop(rankIndex + 1) : elementAssign,
                        Expression.AddAssign(indexes[rankIndex], Expression.Constant(1))),
                    endLoop));
            }

            return Expression.Block(
                indexes,
                indexes.Select(i => Expression.Assign(i, Expression.Constant(0)))
                    .Concat(new[] { arrayAssign, Loop(0) }));
        }
    }
}
