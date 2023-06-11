using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class ReferenceTypeCloneDelegateGenerator
    {
        private static readonly ConcurrentDictionary<Type, Action<object, object, ObjectReferencesCache>> _caches;

        static ReferenceTypeCloneDelegateGenerator()
        {
            _caches = new();
        }

        public static Action<object, object, ObjectReferencesCache> CreateDelegate(Type type) =>
            _caches.GetOrAdd(type, t => Create(t).Compile());

        private static Expression<Action<object, object, ObjectReferencesCache>> Create(Type type)
        {
            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var destinationParameter = Expression.Parameter(typeof(object), "destination");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = CoreCloneExpressionGenerator.CreateCloneExpression(
                type,
                Expression.Convert(sourceParameter, type),
                Expression.Convert(destinationParameter, type),
                cacheParameter);

            return Expression.Lambda<Action<object, object, ObjectReferencesCache>>(
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }
    }
}
