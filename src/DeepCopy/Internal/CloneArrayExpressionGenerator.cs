using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class CloneArrayExpressionGenerator<T, TArray>
    {
        private static readonly Type _type;
        private static readonly Func<TArray, ObjectReferencesCache, TArray> _delegate;

        static CloneArrayExpressionGenerator()
        {
            _type = typeof(TArray);
            _delegate = Create().Compile();
        }

        public static void Cleanup()
        {
            var field = typeof(CloneArrayExpressionGenerator<T, TArray>).GetField(nameof(_delegate),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field.SetValue(null, null);
        }

        public static Func<TArray, ObjectReferencesCache, TArray> Delegate => _delegate;

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
