#nullable enable

using System.Linq.Expressions;
using System.Reflection;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class HashSetCloner
    {
#if NETSTANDARD2_0
        private readonly static BindingFlags privateBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
#endif

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression cache)
        {
#if NETSTANDARD2_0
            string entriesFileName = "m_slots";
            string countFileName = "m_count";
            string comparerFileName = "m_comparer";

            var construct = FixedClonerHelper.AssignField(
                destination,
                destination.Type.GetField("m_freeList", privateBindingFlags)!,
                Expression.Constant(-1, typeof(int)));
#else
            string entriesFileName = "_entries";
            string countFileName = "_count";
            string comparerFileName = "_comparer";

            var construct = Expression.Empty();
#endif

            return HashTableClonerHelper.Build(
                source,
                destination,
                cache,
                entriesFileName,
                countFileName,
                comparerFileName,
                construct,
                Add());
        }

        private static HashTableClonerHelper.AddExpressionDelegate Add() =>
            (Expression destination, Expression entry, Expression cache) =>
                {
#if NETSTANDARD2_0
                    var valueField = entry.Type.GetField("value", privateBindingFlags)!;
#else
                    var valueField = entry.Type.GetField("Value")!;
#endif
                    var addMethod = destination.Type.GetMethod("Add")!;
                    return Expression.Call(
                        destination,
                        addMethod,
                        GetClonedValue(valueField, entry, cache)
                    );
                };

        private static Expression GetClonedValue(FieldInfo fieldInfo, Expression entry, Expression cache)
        {
            return FixedClonerHelper.GetClonedField(fieldInfo, entry, cache);
        }
    }
}
