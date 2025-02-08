using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class CoreCloneExpressionGenerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression CreateCloneExpression<T>(
            ParameterExpression source, ParameterExpression destination, ParameterExpression cache)
        {
            var targets = CopyMemberExtractor.Extract<T>();

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, destination, cache));

            return Expression.Block(expressions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression CreateCloneExpression(Type type,
            Expression source, Expression destination, Expression cache)
        {
            var targets = CopyMemberExtractor.Extract(type);

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, destination, cache));
            return expressions.Any()
                ? Expression.Block(expressions)
                : Expression.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression CreateCloneExpression(Type type,
            Expression source, Expression destination, ParameterExpression variable, Expression cache)
        {
            var targets = CopyMemberExtractor.Extract(type);

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, variable, cache))
            {
                Expression.Assign(destination, Expression.Convert(variable, typeof(object)))
            };
            return expressions.Any()
                ? Expression.Block(new[] { variable }, expressions)
                : Expression.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Expression> CreateExpressions(
            IEnumerable<(MemberInfo, CopyPolicy)> targets,
            Expression source, Expression destination, Expression cache)
        {
            foreach (var target in targets)
                yield return CreateCloneMemberExpression(source, destination, cache, target.Item1, target.Item2);
        }

        private static Expression CreateCloneMemberExpression(
            Expression source,
            Expression destination,
            Expression cache,
            MemberInfo member,
            CopyPolicy copyPolicy)
        {
            var value = MemberAccessorGenerator.CreateGetter(source, member);
#if DEBUGLOG
            Console.WriteLine($" * {member.Name} : {value.Type} : {copyPolicy}");
#endif

            if (copyPolicy == CopyPolicy.Assign)
            {
                return MemberAccessorGenerator.CreateSetter(destination, member, value);
            }

            var memberType = value.Type;
            Expression body;
            if (memberType.IsArray)
            {
                body = ArrayCloner.Instance.Build(
                    copyPolicy,
                    memberType,
                    value,
                    destination,
                    member,
                    cache);
            }
            else if (copyPolicy == CopyPolicy.ShallowCopy)
            {
                body = MemberAccessorGenerator.CreateSetter(
                    destination,
                    member,
                    Expression.Convert(
                        Expression.Call(value, ReflectionUtils.MemberwizeClone),
                        memberType));
            }
            else
            {
                body = ClassCloner.Instance.Build(
                    memberType,
                    value,
                    destination,
                    member,
                    cache);
            }

            return TypeUtils.IsNullable(memberType)
                ? ExpressionUtils.NullCheck(value, body)
                : body;
        }
    }
}
