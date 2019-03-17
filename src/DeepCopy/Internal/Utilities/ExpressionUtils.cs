using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal.Utilities
{
    internal static class ExpressionUtils
    {
        public static Expression Zero { get; } =
            Expression.Constant(0);

        public static Expression One { get; } =
            Expression.Constant(1);

        public static Expression MemberwiseClone(Type type, Expression instance) =>
            Expression.Convert(
                Expression.Call(instance, ReflectionUtils.MemberwizeClone),
                type);

        public static Expression ArrayCopy(Expression source, Expression destination, Expression length) =>
            Expression.Call(ReflectionUtils.ArrayCopy, source, destination, length);

        public static Expression GetArrayLength(Expression array, int dimention) =>
            Expression.Call(array, ReflectionUtils.GetArrayLength, Expression.Constant(dimention));

        public static Expression NullCheck(Expression value, Expression body) =>
            Expression.IfThen(
                Expression.NotEqual(value, Null),
                body);

        private static Expression Null { get; } =
            Expression.Constant(null, typeof(object));
    }
}
