#nullable enable

using System.Linq.Expressions;
using System.Reflection;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class DictionaryCloner
    {
        private readonly static BindingFlags privateBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression context)
        {
#if NETSTANDARD2_0
            string entriesFileName = "entries";
            string countFileName = "count";
            string comparerFileName = "comparer";
#else
            string entriesFileName = "_entries";
            string countFileName = "_count";
            string comparerFileName = "_comparer";
#endif

            return HashTableClonerHelper.Build(
                source,
                destination,
                context,
                entriesFileName,
                countFileName,
                comparerFileName,
                Expression.Empty(),
                Add());
        }

        private static HashTableClonerHelper.AddExpressionDelegate Add() =>
            (Expression destination, Expression entry, Expression context) =>
            {
                var methods = destination.Type.GetMethods(privateBindingFlags);
                var addMethod = destination.Type.GetMethod("Add")!;
                return Expression.Call(
                    destination,
                    addMethod,
                    GetClonedKey(entry, context),
                    GetClonedValue(entry, context)
                );
            };

        private static Expression GetClonedKey(Expression entry, Expression context)
        {
            var keyField = entry.Type.GetField("key")!;
            return FixedClonerHelper.GetClonedField(keyField, entry, context);
        }

        private static Expression GetClonedValue(Expression entry, Expression context)
        {
            var valueField = entry.Type.GetField("value")!;
            return FixedClonerHelper.GetClonedField(valueField, entry, context);
        }
    }
}
