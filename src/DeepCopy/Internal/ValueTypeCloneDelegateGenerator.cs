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
            _delegate = ValueTypeCloneExpressionGeneratorHelper.Create<T>(_type).Compile();
        }

        public static void Clearnup() =>
            _delegate = null;

        public static ValueTypeCloneDelegate CreateDelegate() =>
            _delegate ??= ValueTypeCloneExpressionGeneratorHelper.Create<T>(_type).Compile();
    }

    file static class ValueTypeCloneExpressionGeneratorHelper
    {
        public static Expression<ValueTypeCloneExpressionGenerator<T>.ValueTypeCloneDelegate> Create<T>(Type type)
        {
            var refType = type.MakeByRefType();
            var sourceParameter = Expression.Parameter(refType, "source");
            var destinationParameter = Expression.Parameter(refType, "destination");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = TypeUtils.IsAssignableType(type)
                ? Expression.Assign(destinationParameter, sourceParameter)
                : CoreCloneExpressionGenerator.CreateCloneExpression<T>(
                    sourceParameter, destinationParameter, cacheParameter);

            return Expression.Lambda<ValueTypeCloneExpressionGenerator<T>.ValueTypeCloneDelegate> (
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }
    }
}
