using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class CloneArrayExpressionGenerator<T, TArray>
    {
        private static readonly Type _type;
        private static Func<TArray, ObjectReferencesCache, TArray> _delegate;

        static CloneArrayExpressionGenerator()
        {
            _type = typeof(TArray);
        }

        public static void Cleanup() =>
            _delegate = null;

        public static Func<TArray, ObjectReferencesCache, TArray> CreateDelegate() =>
            _delegate ??= Create().Compile();

        private static Expression<Func<TArray, ObjectReferencesCache, TArray>> Create()
        {
            var sourceParameter = Expression.Parameter(_type, "source");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = CreateCloneExpression(sourceParameter, cacheParameter);

            return Expression.Lambda<Func<TArray, ObjectReferencesCache, TArray>>(
                body,
                sourceParameter, cacheParameter);
        }

        private static Expression CreateCloneExpression(ParameterExpression source, ParameterExpression cache)
        {
            var destination = Expression.Parameter(_type, "destination");

            return Expression.Block(
                [destination],
                ArrayCloner.Instance.Build(
                    _type,
                    source,
                    destination,
                    cache),
                destination);
        }
    }
}
