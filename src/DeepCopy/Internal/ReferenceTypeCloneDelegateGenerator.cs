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
            _delegate = ReferenceTypeCloneDelegateGeneratorHelper.Create<T>(_type).Compile();
        }

        public static void Cleanup() =>
           _delegate = null;

        public static Action<T, T, ObjectReferencesCache> CreateDelegate() =>
            _delegate ??= ReferenceTypeCloneDelegateGeneratorHelper.Create<T>(_type).Compile();
    }

    file static class ReferenceTypeCloneDelegateGeneratorHelper
    {
        public static Expression<Action<T, T, ObjectReferencesCache>> Create<T>(Type type)
        {
            var sourceParameter = Expression.Parameter(type, "source");
            var destinationParameter = Expression.Parameter(type, "destination");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = CoreCloneExpressionGenerator.CreateCloneExpression<T>(
                sourceParameter, destinationParameter, cacheParameter);

            return Expression.Lambda<Action<T, T, ObjectReferencesCache>>(
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }
    }
}
