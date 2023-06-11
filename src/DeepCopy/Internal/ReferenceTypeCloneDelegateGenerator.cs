using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class ReferenceTypeCloneDelegateGenerator<T>
    {
        private static readonly Type _type;
        private static Action<T, T, ObjectReferencesCache> _delegate;

        static ReferenceTypeCloneDelegateGenerator()
        {
            _type = typeof(T);
        }

        public static void Clearnup() =>
            _delegate = null;

        public static Action<T, T, ObjectReferencesCache> CreateDelegate() =>
            _delegate ??= Create().Compile();

        private static Expression<Action<T, T, ObjectReferencesCache>> Create()
        {
            var sourceParameter = Expression.Parameter(_type, "source");
            var destinationParameter = Expression.Parameter(_type, "destination");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = CoreCloneExpressionGenerator.CreateCloneExpression<T>(
                sourceParameter, destinationParameter, cacheParameter);

            return Expression.Lambda<Action<T, T, ObjectReferencesCache>>(
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }
    }
}
