using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class CloneArrayExpressionGenerator<T>
    {
        private static readonly Type _type;
        private static Func<T[], ObjectReferencesCache, T[]> _delegate;

        static CloneArrayExpressionGenerator()
        {
            _type = typeof(T[]);
        }

        public static void Clearnup() =>
            _delegate = null;

        public static Func<T[], ObjectReferencesCache, T[]> CreateDelegate() =>
            _delegate ?? (_delegate = Create().Compile());

        private static Expression<Func<T[], ObjectReferencesCache, T[]>> Create()
        {
            var sourceParameter = Expression.Parameter(_type, "source");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = CreateCloneExpression(sourceParameter, cacheParameter);

            return Expression.Lambda<Func<T[], ObjectReferencesCache, T[]>>(
                body,
                sourceParameter, cacheParameter);
        }

        private static Expression CreateCloneExpression(ParameterExpression source, ParameterExpression cache)
        {
            var destination = Expression.Parameter(_type, "destination");

            return Expression.Block(
                new[] { destination },
                ArrayCloner.Instance.Build(
                    typeof(T),//_type,
                    source,
                    destination,
                    cache),
                destination);
        }
    }
}
