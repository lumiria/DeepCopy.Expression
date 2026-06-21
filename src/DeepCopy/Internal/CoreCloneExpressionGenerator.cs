using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepCopy.Internal.MemberCloners;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class CoreCloneExpressionGenerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression CreateCloneExpression<T>(
            ParameterExpression source, ParameterExpression destination, ParameterExpression context)
        {
            try
            {
                if (FixedCloner.TryGetBuilder(typeof(T), out var builder))
                    return builder(source, destination, context);
            }
            catch (Exception exception)
            {
                throw new InvalidCloneBuilderException(typeof(T), exception);
            }

            return CreateCloneExpressionInner<T>(source, destination, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression CreateCloneExpression(Type type,
            Expression source, Expression destination, Expression context)
        {
            try
            {
                if (FixedCloner.TryGetBuilder(type, out var builder))
                    return builder(source, destination, context);
            }
            catch (Exception exception)
            {
                throw new InvalidCloneBuilderException(type, exception);
            }

            return CreateCloneExpressionInner(type, source, Expression.Convert(destination, type), context);
        }

        internal static Expression CreateCloneExpressionInner<T>(
        ParameterExpression source, ParameterExpression destination, ParameterExpression context,
            params string[] ignoreFields)
        {
            var targets = CopyMemberExtractor.Extract<T>(ignoreFields);

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, destination, context));

#if NET8_0_OR_GREATER
            return Expression.Block(expressions);
#else
            return expressions.Count > 0
                ? Expression.Block(expressions)
                : Expression.Empty();
#endif
        }

        internal static Expression CreateCloneExpressionInner(Type type,
            Expression source, Expression destination, Expression context,
            params string[] ignoreFields)
        {
            var targets = CopyMemberExtractor.Extract(type, ignoreFields);

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, destination, context));
            return expressions.Any()
                ? Expression.Block(expressions)
                : Expression.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expression CreateCloneExpression(Type type,
            Expression source, Expression destination, ParameterExpression variable, Expression context)
        {
            var targets = CopyMemberExtractor.Extract(type);

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, variable, context))
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
            Expression source, Expression destination, Expression context)
        {
            foreach (var target in targets)
                yield return CreateCloneMemberExpression(source, destination, context, target.Item1, target.Item2);
        }

        private static Expression CreateCloneMemberExpression(
            Expression source,
            Expression destination,
            Expression context,
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
                    context);
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
            else if (TryGetSpecialMemberCloneBuilder(memberType, out var builder))
            {
                body = builder(
                    memberType,
                    value,
                    destination,
                    member,
                    context);
            }
            else
            {
                body = ClassCloner.Instance.Build(
                    memberType,
                    value,
                    destination,
                    member,
                    context);
            }

            return TypeUtils.IsNullable(memberType)
                ? ExpressionUtils.NullCheck(value, body)
                : body;
        }

        private static bool TryGetSpecialMemberCloneBuilder(
            Type memberType,
            out Func<Type, Expression, Expression, MemberInfo, Expression, Expression> builder)
        {
            return EqualComparerMemberCloner.TryGetBuilder(memberType, out builder);
        }
    }
}
