using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using DeepCopy.Internal.BuiltIns;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class ReferenceTypeCloneDelegateGenerator
    {
        public delegate void ReferenceTypeCloneDelegate(object source, ref object destination, DeepCopyContext context);

        private static readonly ConcurrentDictionary<Type, ReferenceTypeCloneDelegate> _caches;

        static ReferenceTypeCloneDelegateGenerator()
        {
            _caches = new();
        }

        public static void Cleanup() =>
           _caches.Clear();

        public static void Cleanup(Type type) =>
            _caches.TryRemove(type, out _);

        public static ReferenceTypeCloneDelegate CreateDelegate(Type type) =>
            _caches.GetOrAdd(type, t => Create(t).Compile());

        private static Expression<ReferenceTypeCloneDelegate> Create(Type type)
        {
            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var destinationParameter = Expression.Parameter(typeof(object).MakeByRefType(), "destination");
            var contextParameter = Expression.Parameter(typeof(DeepCopyContext), "context");

            var body = CoreCloneExpressionGenerator.CreateCloneExpression(
                type,
                Expression.Convert(sourceParameter, type),
                destinationParameter,
                contextParameter);

            return Expression.Lambda<ReferenceTypeCloneDelegate>(
                body,
                sourceParameter, destinationParameter, contextParameter);
        }
    }

    internal static class ValueTypeCloneDelegateGenerator
    {
        public delegate void ValueTypeCloneDelegate(object source, out object destination, DeepCopyContext context);
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
            var contextParameter = Expression.Parameter(typeof(DeepCopyContext), "context");

            var body = TypeUtils.IsAssignableType(type)
                ? Expression.Assign(destinationParameter, sourceParameter)
                : CoreCloneExpressionGenerator.CreateCloneExpression(
                    type,
                    Expression.Convert(sourceParameter, type),
                    destinationParameter,
                    Expression.Variable(type, "tmp"),
                    contextParameter);

            return Expression.Lambda<ValueTypeCloneDelegate>(
                body,
                sourceParameter, destinationParameter, contextParameter);
        }
    }

    internal static class ArrayCloneDelegateGenerator
    {
        public delegate object ArrayCloneDelegate(object source, DeepCopyContext context);
        private static readonly ConcurrentDictionary<Type, object> _caches;
        private static readonly ConcurrentDictionary<Type, Func<Array, DeepCopyContext, Array>> _wrapperCaches;

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

        public static Func<Array, DeepCopyContext, Array> GetOrCreateWrapperDelegate(Type type) =>
            _wrapperCaches.GetOrAdd(type, t =>
            {
                var @delegate = GetOrCreateDelegate<Delegate>(t);

                var source = Expression.Parameter(typeof(Array), "source");
                var context = Expression.Parameter(typeof(DeepCopyContext), "context");

                var castSource = Expression.Convert(source, t);
                var delegateExpression = Expression.Constant(@delegate, @delegate.GetType());
                var call = Expression.Invoke(delegateExpression, castSource, context);
                var castDest = Expression.Convert(call, typeof(Array));

                return Expression.Lambda<Func<Array, DeepCopyContext, Array>>(castDest, source, context)
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
                var contextParameter = Expression.Parameter(typeof(DeepCopyContext), "context");

                var call = Expression.Call(null, method, Expression.Convert(sourceParameter, t), contextParameter);
                var lambda = Expression.Lambda(call, sourceParameter, contextParameter);

                return lambda.Compile();
            });
    }
}
