using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using DeepCopy.Internal.BuiltIns;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class ReferenceTypeCloneDelegateGenerator
    {
        private static readonly ConcurrentDictionary<Type, Action<object, object, ObjectReferencesCache>> _caches;

        static ReferenceTypeCloneDelegateGenerator()
        {
            _caches = new();
        }

        public static void Cleanup() =>
           _caches.Clear();

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

    internal static class ValueTypeCloneDelegateGenerator
    {
        public delegate void ValueTypeCloneDelegate(object source, out object destination, ObjectReferencesCache cache);
        private static readonly ConcurrentDictionary<Type, ValueTypeCloneDelegate> _caches;

        static ValueTypeCloneDelegateGenerator()
        {
            _caches = new();
        }
        public static void Cleanup() =>
           _caches.Clear();

        public static ValueTypeCloneDelegate CreateDelegate(Type type) =>
            _caches.GetOrAdd(type, t => Create(t).Compile());

        private static Expression<ValueTypeCloneDelegate> Create(Type type)
        {
            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var destinationParameter = Expression.Parameter(typeof(object).MakeByRefType(), "destination");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = TypeUtils.IsAssignableType(type)
                ? Expression.Assign(destinationParameter, sourceParameter)
                : CoreCloneExpressionGenerator.CreateCloneExpression(
                    type,
                    Expression.Convert(sourceParameter, type),
                    destinationParameter,
                    Expression.Variable(type, "tmp"),
                    cacheParameter);

            return Expression.Lambda<ValueTypeCloneDelegate>(
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }
    }

    internal static class ArrayCloneDelegateGenerator
    {
        public delegate object ArrayCloneDelegate(object source, ObjectReferencesCache cache);
        private static readonly ConcurrentDictionary<Type, object> _caches;

        static ArrayCloneDelegateGenerator()
        {
            _caches = [];
        }

        public static void Cleanup() =>
           _caches.Clear();

        public static TDelegate GetOrCreateDelegate<TDelegate>(Type type) =>
            (TDelegate)_caches.GetOrAdd(type, t =>
            {
                var elementType = t.GetElementType();
                var generatorType = typeof(CloneArrayExpressionGenerator<,>);
                var genericGeneratorType = generatorType.MakeGenericType(elementType, t);

                var method = genericGeneratorType.GetMethod(nameof(CloneArrayExpressionGenerator<,>.CreateDelegate));

                return method.Invoke(null, null);
            });
    }

    internal static class DictionaryCloneDelegateGenerator
    {
        private static readonly ConcurrentDictionary<Type, object> _caches;

        static DictionaryCloneDelegateGenerator()
        {
            _caches = [];
        }

        public static void Cleanup() =>
           _caches.Clear();

        public static TDelegate GetOrCreateDelegate<TDelegate>(Type type) =>
            (TDelegate)_caches.GetOrAdd(type, t =>
            {
                var method = FixedDictionaryCloner.GetCloneMethod(t);

                var sourceParameter = Expression.Parameter(typeof(object), "source");
                var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

                var call = Expression.Call(null, method, Expression.Convert(sourceParameter, t), cacheParameter);
                var lambda = Expression.Lambda(call, sourceParameter, cacheParameter);

                return lambda.Compile();
            });
    }
}
