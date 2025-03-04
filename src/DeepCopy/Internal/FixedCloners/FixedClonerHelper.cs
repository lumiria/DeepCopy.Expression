#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class FixedClonerHelper
    {
        private readonly static BindingFlags bindingFlags = 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static BinaryExpression AssignField(Expression destination, FieldInfo fieldInfo, Expression value)
        {
            var field = Expression.MakeMemberAccess(
                destination,
                fieldInfo);

            return Expression.Assign(field, value);
        }

        public static BlockExpression AssignClonedFields(Expression destination, string[] fieldNames, Expression source, Expression cache)
        {
            return Expression.Block(
                fieldNames.Select(field =>
                    AssignClonedField(
                        destination,
                        source,
                        source.Type.GetField(field, bindingFlags)!,
                        cache
                    )
                )
            );
        }

        public static Expression AssignClonedField(Expression destination, Expression source, FieldInfo fieldInfo, Expression cache)
        {
            return MemberAccessorGenerator.CreateSetter(
                destination,
                fieldInfo,
                GetClonedField(fieldInfo, source, cache));
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

            if (type.IsGenericType && type.GetGenericTypeDefinition() is Type genericType)
            {
                if (genericType == typeof(IEqualityComparer<>))
                    return GetClonedEqualityComparer(fieldInfo, source, cache);
                if (genericType == typeof(IComparer<>))
                    return GetClonedComparer(fieldInfo, source, cache);
            }

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
                    ExpressionUtils.IsValueType(field),
                    field,
                    cloneExpression
                )
            );
        }

        private static ConditionalExpression GetClonedEqualityComparer(
            FieldInfo fieldInfo, Expression source, Expression cache)
        {
            var comparer = Expression.Field(source, fieldInfo);

            var equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(
                fieldInfo.FieldType.GetGenericArguments()[0]);

            return GetClonedComparerCore(equalityComparerType, comparer, cache);
        }

        private static ConditionalExpression GetClonedComparer(
            FieldInfo fieldInfo, Expression source, Expression cache)
        {
            var comparer = Expression.Field(source, fieldInfo);

            var equalityComparerType = typeof(Comparer<>).MakeGenericType(
                fieldInfo.FieldType.GetGenericArguments()[0]);

            return GetClonedComparerCore(equalityComparerType, comparer, cache);
        }

        private static ConditionalExpression GetClonedComparerCore(
            Type comparerInstanceType, Expression comparer, Expression cache)
        {
            var defaultComparer = Expression.Convert(
                Expression.Property(
                    null,
                    comparerInstanceType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public)!),
                comparer.Type);

            return Expression.Condition(
                Expression.Equal(comparer, defaultComparer),
                defaultComparer,
                Expression.Call(
                    ReflectionUtils.ObjectClone.MakeGenericMethod(comparer.Type),
                    comparer,
                    cache
                )
            );
        }
    }
}
