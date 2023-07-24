using System;
using System.Linq.Expressions;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class ValueTypeCloneExpressionGenerator<T>
    {
        public delegate void ValueTypeCloneDelegate(in T source, ref T destination, ObjectReferencesCache cache);

        private static readonly Type _type;
        private static ValueTypeCloneDelegate _delegate;

        static ValueTypeCloneExpressionGenerator()
        {
            _type = typeof(T);
        }

        public static void Clearnup() =>
            _delegate = null;

        public static ValueTypeCloneDelegate CreateDelegate() =>
            _delegate ??= Create().Compile();

        private static Expression<ValueTypeCloneDelegate> Create()
        {
            var sourceParameter = Expression.Parameter(_type.MakeByRefType(), "source");
            var destinationParameter = Expression.Parameter(_type.MakeByRefType(), "destination");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = TypeUtils.IsAssignableType(_type)
                ? Expression.Assign(destinationParameter, sourceParameter)
                : CoreCloneExpressionGenerator.CreateCloneExpression<T>(
                    sourceParameter, destinationParameter, cacheParameter);

            return Expression.Lambda<ValueTypeCloneDelegate>(
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }
    }
}
