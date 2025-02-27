#nullable enable

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class ConcurrentDictionaryCloner
    {
        private readonly static BindingFlags privateBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression cache)
        {
            return Expression.Block(
                AssignClonedFields(destination, source, cache),
                Clear(destination),
                LoopBuckets(destination, source, cache)
            );
        }

        private static BlockExpression AssignClonedFields(Expression destination, Expression source, Expression cache)
        {
#if NETSTANDARD2_0
            string[] fields = [
                "m_tables",
                "m_comparer",
                "m_growLockArray",
                "m_keyRehashCount",
                "m_budget"
            ];
#else
            string[] fields = [
                "_comparerIsDefaultForClasses",
                "_growLockArray",
                "_budget",
                "_tables",
            ];
#endif

            return Expression.Block(
                fields.Select(field =>
                    AssignClonedField(
                        destination,
                        source,
                        source.Type.GetField(field, privateBindingFlags)!,
                        cache
                    )
                )
            );
        }

        private static Expression AssignClonedField(Expression destination, Expression source, FieldInfo fieldInfo, Expression cache)
        {
            return MemberAccessorGenerator.CreateSetter(
                destination,
                fieldInfo,
                FixedClonerHelper.GetClonedField(fieldInfo, source, cache));
        }

        private static MethodCallExpression Clear(Expression destination)
        {
            return Expression.Call(
                destination,
                destination.Type.GetMethod("Clear")!);
        }

        private static BlockExpression LoopBuckets(Expression destination, Expression source, Expression cache)
        {
#if NETSTANDARD2_0
            var tableFieldName = "m_tables";
            var bucketsFieldName = "m_buckets";
#else
            var tableFieldName = "_tables";
            var bucketsFieldName = "_buckets";
#endif

            var tables = Expression.MakeMemberAccess(
                source,
                source.Type.GetField(tableFieldName, privateBindingFlags)!);
            var buckets = Expression.MakeMemberAccess(
                tables,
                tables.Type.GetField(bucketsFieldName, privateBindingFlags)!);

            var i = Expression.Parameter(typeof(int), "i");
            var endLoop = Expression.Label("EndLoopBucket");

            var bucket = Expression.ArrayAccess(buckets, i);
            var length = Expression.ArrayLength(buckets);

            return Expression.Block(
                [i],
                Expression.Loop(
                    Expression.Block(
                        Expression.IfThen(
                            Expression.GreaterThanOrEqual(i, length),
                            Expression.Break(endLoop)
                        ),
                        LoopNode(destination, bucket, cache),
                        Expression.PreIncrementAssign(i)
                    ),
                    endLoop
                )
            );
        }

        private static BlockExpression LoopNode(Expression destination, Expression bucket, Expression cache)
        {
#if NETSTANDARD2_0
            var nextFieldName = "m_next";

            var node = bucket;
#else
            var nextFieldName = "_next";

            var node = Expression.MakeMemberAccess(
                bucket,
                bucket.Type.GetField("_node", privateBindingFlags)!);
#endif

            var @null = Expression.Constant(null, node.Type);
            var endLoop = Expression.Label("EndLoopNode");

            var current = Expression.Parameter(node.Type, "current");
            var next = Expression.MakeMemberAccess(
                current,
                current.Type.GetField(nextFieldName, privateBindingFlags)!);

            return Expression.Block(
                [current],
                Expression.Assign(current, node),
                Expression.Loop(
                   Expression.Block(
                        Expression.IfThen(
                            Expression.Equal(current, @null),
                            Expression.Break(endLoop)
                        ),
                        Add(destination, current, cache),
                        Expression.Assign(current, next)
                    ),
                    endLoop
                )
            );
        }

        private static MethodCallExpression Add(Expression destination, Expression node, Expression cache)
        {
#if NETSTANDARD2_0
            var keyFieldName = "m_key";
            var valueFieldName = "m_value";
#else
            var keyFieldName = "_key";
            var valueFieldName = "_value";
#endif

            var key = FixedClonerHelper.GetClonedField(
                node.Type.GetField(keyFieldName, privateBindingFlags)!,
                node,
                cache);
            var value = FixedClonerHelper.GetClonedField(
                node.Type.GetField(valueFieldName, privateBindingFlags)!,
                node,
                cache);

            var addMethod = destination.Type.GetMethod("TryAdd")!;
            return Expression.Call(
                destination,
                addMethod,
                key,
                value
            );
        }
    }
}
