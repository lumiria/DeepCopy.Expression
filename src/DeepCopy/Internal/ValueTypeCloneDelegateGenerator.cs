using System;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class ValueTypeCloneExpressionGenerator<T>
    {
        public delegate void ValueTypeCloneDelegate(in T source, ref T destination, ObjectReferencesCache cache);

        private static readonly Type _type;
        private static readonly ValueTypeCloneDelegate _delegate;

        static ValueTypeCloneExpressionGenerator()
        {
            _type = typeof(T);
            _delegate = ValueTypeCloneExpressionGeneratorHelper.Create<T>(_type).Compile();
        }

        public static void Cleanup()
        {
            var field = typeof(ValueTypeCloneExpressionGenerator<T>).GetField(nameof(_delegate),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field.SetValue(null, null);
        }

        public static ValueTypeCloneDelegate Delegate => _delegate;
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
                : Nullable.GetUnderlyingType(type) is Type underlyingType
                    ? Expression.Assign(
                        destinationParameter,
                        ExpressionUtils.NullTernaryCheck(
                            type,
                            sourceParameter,
                            Expression.TypeAs(
                                Expression.Call(
                                    ReflectionUtils.ValueClone.MakeGenericMethod(underlyingType),
                                    Expression.MakeMemberAccess(
                                        sourceParameter,
                                        sourceParameter.Type.GetField("value", BindingFlags.Instance | BindingFlags.NonPublic)),
                                    cacheParameter),
                                type
                            )
                        )
                    )
                    : CoreCloneExpressionGenerator.CreateCloneExpression<T>(
                        sourceParameter, destinationParameter, cacheParameter);

            return Expression.Lambda<ValueTypeCloneExpressionGenerator<T>.ValueTypeCloneDelegate> (
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }
    }
}
