using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DeepCopy.Internal.BuiltIns;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners.Core
{
    internal static class CloneExpressionBuilder
    {
        public static Expression Build(
            Expression source,
            Expression context)
        {
            var type = source.Type;

            var expression = type switch
            {
                _ when Nullable.GetUnderlyingType(type) is Type nullableType
                    => ReflectionUtils.NullableValueClone.MakeGenericMethod(nullableType),

                _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                    => FixedDictionaryCloner.GetCloneMethod(type),

                _ when type == typeof(Array)
                    => ReflectionUtils.ArrayClone,

                _ when !TypeUtils.IsValueType(type)
                    => (type.IsInterface
                        ? ReflectionUtils.InterfaceClone.MakeGenericMethod(type)
                        : ReflectionUtils.ObjectClone.MakeGenericMethod(type)),

                _ => ReflectionUtils.ValueClone.MakeGenericMethod(type),
            };

            return Expression.Call(expression, source, context);
        }
    }
}
