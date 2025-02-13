#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DeepCopy.Internal.FixedCloners;

namespace DeepCopy.Internal
{
    internal static class FixedCloner
    {
        public delegate BlockExpression ExpressionBuilder(Expression source, Expression desitination, Expression cache);

        private readonly static Dictionary<Type, ExpressionBuilder> _bag;

        static FixedCloner()
        {
            _bag = new ()
            {
                [typeof(Dictionary<,>)] = DictionaryCloner.Build
            };
        }

#if NETSTANDARD2_0
        public static bool TryGetBuilder(Type type, out ExpressionBuilder? builder)
#else
        public static bool TryGetBuilder(Type type, [MaybeNullWhen(false)] out ExpressionBuilder? builder)
#endif
        {
            if (type.IsGenericType)
            {
                return _bag.TryGetValue(type.GetGenericTypeDefinition(), out builder);
            }

            builder = null;
            return false;
        }
    }
}
