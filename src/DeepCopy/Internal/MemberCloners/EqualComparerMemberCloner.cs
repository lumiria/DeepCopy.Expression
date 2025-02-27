#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DeepCopy.Internal.MemberCloners
{
    internal static class EqualComparerMemberCloner
    {
        public static bool TryGetBuilder(Type memberType,
            out Func<Type, Expression, Expression, MemberInfo, Expression, Expression>? builder)
        {
            var genericInterfaceType = typeof(IEqualityComparer<>);
            var interfaceType =
                memberType.IsInterface && memberType.IsGenericType && memberType.GetGenericTypeDefinition() == genericInterfaceType
                    ? memberType
                    : memberType.GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .FirstOrDefault(i => i.GetGenericTypeDefinition() == genericInterfaceType);

            builder = null;
            if (interfaceType is null) return false;

            builder = (Type memberType,
                Expression value,
                Expression destination,
                MemberInfo member,
                Expression cache) =>
                Build(interfaceType, memberType, value, destination, member, cache);
            return true;
        }

        private static Expression Build(
            Type interfaceType,
            Type memberType,
            Expression value,
            Expression destination,
            MemberInfo member,
            Expression cache)
        {
            var equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(interfaceType.GetGenericArguments()[0]);
            var defaultComparer = Expression.Convert(
                Expression.Property(
                    null,
                    equalityComparerType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public)!),
                interfaceType);

            return Expression.IfThenElse(
                Expression.Equal(value, defaultComparer),
                MemberAccessorGenerator.CreateSetter(
                    destination,
                    member,
                    defaultComparer
                ),
                ClassCloner.Instance.Build(
                    memberType,
                    value,
                    destination,
                    member,
                    cache)
            );
        }
    }
}
