using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class CloneExpressionGenerator<T>
    {
        private static readonly Type _type;
        private static Action<T, T> _delegate;

        static CloneExpressionGenerator()
        {
            _type = typeof(T);
        }

        public static void Clearnup() =>
            _delegate = null;

        public static Action<T, T> CreateDelegate() =>
            _delegate ?? (_delegate = Create().Compile());

        private static Expression<Action<T, T>> Create()
        {
            var sourceParameter = Expression.Parameter(_type, "source");
            var destinationParameter = Expression.Parameter(_type, "destination");

            var body = CreateCloneExpression(sourceParameter, destinationParameter);

            return Expression.Lambda<Action<T, T>>(
                body,
                sourceParameter, destinationParameter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Expression CreateCloneExpression(ParameterExpression source, ParameterExpression destination)
        {
            var targets = CopyMemberExtractor.Extract<T>();

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, destination));
            //var expressions = CreateExpressions(targets, source, destination);

            return Expression.Block(expressions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Expression> CreateExpressions(IEnumerable<(MemberInfo, CopyPolicy)> targets, ParameterExpression source, ParameterExpression destination)
        {
            foreach (var target in targets)
                yield return CreateCloneMemberExpression(source, destination, target.Item1, target.Item2);
        }

        private static Expression CreateCloneMemberExpression(
            ParameterExpression source,
            ParameterExpression destination,
            MemberInfo member,
            CopyPolicy copyPolicy)
        {
            var value = MemberAccessorGenerator.CreateGetter(source, member);

            if (copyPolicy == CopyPolicy.Assign)
            {
                return MemberAccessorGenerator.CreateSetter(destination, member, value);
            }

            var memberType = value.Type;
            Expression body = null;
            if (memberType.IsArray)
            {
                body = ArrayCloner.Instance.Build(
                    copyPolicy,
                    memberType,
                    value,
                    destination,
                    member);
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
                    member);
            }

            return ExpressionUtils.NullCheck(value, body);
        }
    }
}
