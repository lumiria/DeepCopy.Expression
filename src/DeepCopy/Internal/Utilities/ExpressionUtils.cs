﻿using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal.Utilities
{
    internal static class ExpressionUtils
    {
        public static Expression Zero { get; } =
            Expression.Constant(0);

        public static Expression MemberwiseClone(Type type, Expression instance) =>
            Expression.Convert(
                Expression.Call(instance, ReflectionUtils.MemberwizeClone),
                type);

        public static Expression ArrayCopy(Expression source, Expression destination, Expression length) =>
            Expression.Call(ReflectionUtils.ArrayCopy, source, destination, length);

        public static Expression GetArrayLength(Expression array, int dimention) =>
            Expression.Call(array, ReflectionUtils.GetArrayLength, Expression.Constant(dimention));

        public static Expression IsObject(Expression instance) =>
            Expression.TypeEqual(instance, typeof(object));

        public static Expression IsObjectOrValueType(Expression instance) =>
            Expression.Call(ReflectionUtils.IsObjectOrValueType,
                Expression.Call(instance, ReflectionUtils.GetObjectType));

        public static Expression IsValueType(Expression instance) =>
            Expression.Call(ReflectionUtils.IsValueType,
                Expression.Call(instance, ReflectionUtils.GetObjectType));

        public static Expression CloneObjectType(Expression source, Expression cache) =>
            Expression.Call(ReflectionUtils.ObjectTypeClone, source, cache);

        public static BlockExpression Loop(Expression array, Expression length, Func<Expression, Expression> body, params Expression[] initializeExpressions)
        {
            var i = Expression.Parameter(typeof(int), "i");
            var endLoop = Expression.Label("EndLoop");

            var item = Expression.ArrayIndex(array, i);

            return Expression.Block(
                [i],
                [.. initializeExpressions,
                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(
                                Expression.GreaterThanOrEqual(i, length),
                                Expression.Break(endLoop)
                            ),
                            body(item),
                            Expression.PreIncrementAssign(i)
                        ),
                        endLoop
                    )
                ]
            );
        }

        public static Expression NullCheck(Expression value, Expression body) =>
            Expression.IfThen(
                Expression.NotEqual(value, Null),
                body);

        public static Expression NullTernaryCheck(Expression value, Expression body) =>
            Expression.Condition(
                Expression.NotEqual(value, Null),
                body,
                Null);

        public static Expression NullTernaryCheck(Type type, Expression value, Expression body) =>
            Expression.Condition(
                Expression.NotEqual(value, Expression.Constant(null, type)),
                body,
                Expression.Constant(null, type));

        private static Expression Null { get; } =
            Expression.Constant(null, typeof(object));
    }
}
