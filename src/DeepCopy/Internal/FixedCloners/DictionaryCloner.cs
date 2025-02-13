#nullable enable

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class DictionaryCloner
    {
        private readonly static BindingFlags privateBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression cache)
        {
            var entries = Expression.MakeMemberAccess(
                source,
                source.Type.GetField("_entries", privateBindingFlags)!);
            var length = Expression.Field(source, source.Type.GetField("_count", privateBindingFlags)!);
            var comparer = Expression.MakeMemberAccess(
                source,
                source.Type.GetField("_comparer", privateBindingFlags)!);

            var i = Expression.Parameter(typeof(int), "i");
            var endLoop = Expression.Label("EndLoop");

            var entry = Expression.ArrayIndex(entries, i);

            return Expression.Block(
                [i],
                InitializeDestination(destination, length),
                AssignClonedComparer(destination, comparer, cache),
                Expression.Loop(
                    Expression.Block(
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(i, length),
                            Expression.Break(endLoop)
                        ),
                        Add(destination, entry, cache),
                        Expression.PreIncrementAssign(i)
                    ),
                    endLoop
                )
            );
        }

        private static Expression InitializeDestination(Expression destination, Expression length)
        {
            var initializeMethod = destination.Type.GetMethod("Initialize", privateBindingFlags)!;
            return Expression.Call(destination, initializeMethod, length);
        }

        private static BinaryExpression AssignClonedComparer(Expression destination, MemberExpression comparer, Expression cache)
        {
            var destinationComparer = Expression.MakeMemberAccess(
                destination,
                destination.Type.GetField("_comparer", privateBindingFlags)!);
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

        private static Expression Add(Expression destination, Expression entry, Expression cache)
        {
            var methods = destination.Type.GetMethods(privateBindingFlags);
            var addMethod = destination.Type.GetMethod("Add")!;
            return Expression.Call(
                destination,
                addMethod,
                GetClonedKey(entry, cache),
                GetClonedValue(entry, cache)
            );
        }

        private static Expression GetClonedKey(Expression entry, Expression cache)
        {
            return GetClonedField("key", entry, cache);
        }

        private static Expression GetClonedValue(Expression entry, Expression cache)
        {
            return GetClonedField("value", entry, cache);
        }

        private static Expression GetClonedField(string fieldName, Expression entry, Expression cache)
        {
            var keyField = entry.Type.GetField(fieldName)!;
            var field = Expression.Field(entry, keyField);
            var type = field.Type;

            if (TypeUtils.IsAssignableType(type))
            {
                return field;
            }

            if (type.IsArray)
            {
                var array = Expression.Parameter(type, "array");
                var length = Expression.ArrayLength(array);
                var arrayAssign = Expression.Assign(
                    array,
                    Expression.NewArrayBounds(array.Type.GetElementType()!, length)
                );

                return ExpressionUtils.NullTernaryCheck(
                    field,
                    Expression.Block(
                        [array],
                        arrayAssign,
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
                ExpressionUtils.IsObjectOrValueType(field),
                field,
                cloneExpression
            );
        }

    }
}
