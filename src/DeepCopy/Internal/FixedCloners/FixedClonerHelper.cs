#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class FixedClonerHelper
    {
        public static BinaryExpression AssignField(Expression destination, FieldInfo fieldInfo, Expression value)
        {
            var field = Expression.MakeMemberAccess(
                destination,
                fieldInfo);

            return Expression.Assign(field, value);
        }

        public static BinaryExpression AssignClonedComparer(Expression destination, FieldInfo fieldInfo, MemberExpression comparer, Expression cache)
        {
            var destinationComparer = Expression.MakeMemberAccess(
                destination,
                fieldInfo);
            var equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(comparer.Type.GetGenericArguments()[0]);
            var defaultComparer = Expression.Convert(
                Expression.Property(
                    null,
                    equalityComparerType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public)!),
                comparer.Type);

            return Expression.Assign(
                destinationComparer,
                Expression.Condition(
                    Expression.Equal(comparer, defaultComparer),
                    defaultComparer,
                    Expression.Call(
                        ReflectionUtils.ObjectClone.MakeGenericMethod(comparer.Type),
                        comparer,
                        cache
                    )
                )
            );
        }

        public static Expression GetClonedField(FieldInfo fieldInfo, Expression source, Expression cache)
        {
            var field = Expression.Field(source, fieldInfo);
            var type = field.Type;

            if (TypeUtils.IsAssignableType(type))
            {
                return field;
            }

            if (type.IsArray)
            {
                var array = Expression.Parameter(type, "array");
                return ExpressionUtils.NullTernaryCheck(
                    type,
                    field,
                    Expression.Block(
                        type,
                        [array],
                        ArrayCloner.Instance.Build(
                             TypeUtils.IsAssignableType(type.GetElementType())
                                 ? CopyPolicy.ShallowCopy
                                 : CopyPolicy.DeepCopy,
                             type,
                             field,
                             array,
                             cache
                        ),
                        array
                    )
                );
            }

            var cloneExpression = Expression.Call(
                ReflectionUtils.ObjectClone.MakeGenericMethod(type),
                field,
                cache);

            if (type != typeof(object))
            {
                return cloneExpression;
            }

            return Expression.Condition(
                ExpressionUtils.IsObject(field),
                Expression.Constant(new object()),
                Expression.Condition(
                    ExpressionUtils.IsObjectOrValueType(field),
                    field,
                    cloneExpression
                )
            );
        }
    }
}
