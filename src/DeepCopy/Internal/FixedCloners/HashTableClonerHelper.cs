#nullable enable

using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class HashTableClonerHelper
    {
        private readonly static BindingFlags privateBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public delegate Expression AddExpressionDelegate(Expression destination, Expression entry, Expression cache);

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression cache,
            string entriesFieldName,
            string countFieldName,
            string comparerFieldName,
            Expression construct,
            AddExpressionDelegate add)
        {
            var comparerFieldInfo = source.Type.GetField(comparerFieldName, privateBindingFlags)!;

            var entries = Expression.MakeMemberAccess(
                source,
                source.Type.GetField(entriesFieldName, privateBindingFlags)!);
            var length = Expression.Field(source, source.Type.GetField(countFieldName, privateBindingFlags)!);
            var comparer = Expression.MakeMemberAccess(
                source,
                comparerFieldInfo);

            return ExpressionUtils.Loop(
                entries,
                length,
                item => add(destination, item, cache),
                construct,
                InitializeDestination(destination, length),
                FixedClonerHelper.AssignClonedComparer(destination, comparerFieldInfo, comparer, cache)
            );
        }

        private static Expression InitializeDestination(Expression destination, Expression length)
        {
            var initializeMethod = destination.Type.GetMethod("Initialize", privateBindingFlags)!;
            return Expression.Call(destination, initializeMethod, length);
        }
    }
}
