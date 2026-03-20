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

        public static void Cleanup(Type type) =>
            _caches.TryRemove(type, out _);

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

        public static void Cleanup(Type type) =>
            _caches.TryRemove(type, out _);

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
        private static readonly ConcurrentDictionary<Type, Func<Array, ObjectReferencesCache, Array>> _wrapperCaches;

        static ArrayCloneDelegateGenerator()
        {
            _caches = [];
            _wrapperCaches = [];
        }

        public static void Cleanup()
        {
            _caches.Clear();
            _wrapperCaches.Clear();
        }

        public static void Cleanup(Type type)
        {
            _caches.TryRemove(type, out _);
            _wrapperCaches.TryRemove(type, out _);
        }

        public static TDelegate GetOrCreateDelegate<TDelegate>(Type type) =>
            (TDelegate)_caches.GetOrAdd(type, t =>
            {
                var elementType = t.GetElementType();
                var generatorType = typeof(CloneArrayExpressionGenerator<,>);
                var genericGeneratorType = generatorType.MakeGenericType(elementType, t);

                var method = genericGeneratorType.GetProperty(nameof(CloneArrayExpressionGenerator<,>.Delegate));

                return method.GetValue(null);
            });

        public static Func<Array, ObjectReferencesCache, Array> GetOrCreateWrapperDelegate(Type type) =>
            _wrapperCaches.GetOrAdd(type, t =>
            {
                var @delegate = GetOrCreateDelegate<Delegate>(t);

                var source = Expression.Parameter(typeof(Array), "source");
                var cache = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

                var castSource = Expression.Convert(source, t);
                var delegateExpression = Expression.Constant(@delegate, @delegate.GetType());
                var call = Expression.Invoke(delegateExpression, castSource, cache);
                var castDest = Expression.Convert(call, typeof(Array));

                return Expression.Lambda<Func<Array, ObjectReferencesCache, Array>>(castDest, source, cache)
                    .Compile();
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

        public static void Cleanup(Type type) =>
            _caches.TryRemove(type, out _);

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
