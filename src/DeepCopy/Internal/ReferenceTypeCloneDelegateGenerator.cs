using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class ReferenceTypeCloneDelegateGenerator<T>
    {
        private static readonly Type _type;
        private static readonly Action<T, T, ObjectReferencesCache> _delegate;

        static ReferenceTypeCloneDelegateGenerator()
        {
            _type = typeof(T);
            _delegate = ReferenceTypeCloneDelegateGeneratorHelper.Create<T>(_type).Compile();
        }

        public static void Cleanup()
        {
            var field = typeof(ReferenceTypeCloneDelegateGenerator<T>).GetField(nameof(_delegate),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field.SetValue(null, null);
        }

        public static Action<T, T, ObjectReferencesCache> Delegate => _delegate;
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
