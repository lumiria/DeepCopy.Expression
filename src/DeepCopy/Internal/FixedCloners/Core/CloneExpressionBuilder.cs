using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.BuiltIns;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners.Core
{
    internal static class CloneExpressionBuilder
    {
        public static Expression Build(
            Expression source,
            Expression context)
        => Build(source.Type, source, context);
        public static Expression Build(
            Type type,
            Expression source,
            Expression context)
        {
            var cloneMethod = GetCloneMethod(type, source);
            return Expression.Call(cloneMethod, source, context);
        }

        public static MethodInfo GetCloneMethod(
            Type type,
            Expression source)
            => type switch
            {
                _ when Nullable.GetUnderlyingType(type) is Type nullableType
                    => ReflectionUtils.NullableValueClone.MakeGenericMethod(nullableType),

                _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                    => FixedDictionaryCloner.GetCloneMethod(type),

                _ when type == typeof(Array)
                    => ReflectionUtils.ArrayClone,

                _ when TypeUtils.IsValueType(type)
                    => ReflectionUtils.ValueClone.MakeGenericMethod(type),

                _ when type.IsInterface
                    => ReflectionUtils.InterfaceClone.MakeGenericMethod(type),

                _ when type.IsSealed
                    => ReflectionUtils.CloneAs.MakeGenericMethod(type),

                _ => ReflectionUtils.ObjectClone.MakeGenericMethod(type),
            };
    }
}
