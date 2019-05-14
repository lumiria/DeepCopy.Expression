using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal sealed class ClassCloner
    {
        public static ClassCloner Instance { get; } =
            new ClassCloner();

        public Expression Build(
                Type type,
                Expression source,
                Expression destination,
                Expression cache)
        {
            var cloneExpression = ClonerCache.Instance.Get(type, source, cache);

            if (type != typeof(object))
            {
                return Expression.Assign(destination, cloneExpression);
            }

            return Expression.Assign(
                destination,
                Expression.Condition(
                    //Expression.TypeEqual(source, typeof(object)),
                    ExpressionUtils.IsObjectOrValueType(source),
                    ExpressionUtils.MemberwiseClone(type, source),
                    cloneExpression));
        }

        public Expression Build(
                Type type,
                Expression source,
                Expression destination,
                MemberInfo member,
                Expression cache)
        {
            var cloneExpression = ClonerCache.Instance.Get(type, source, cache);

            if (type != typeof(object))
            {
                return MemberAccessorGenerator.CreateSetter(
                    destination, member, cloneExpression);
            }

            return MemberAccessorGenerator.CreateSetter(
                destination,
                member,
                Expression.Condition(
                    //Expression.TypeEqual(source, typeof(object)),
                    ExpressionUtils.IsObjectOrValueType(source),
                    ExpressionUtils.MemberwiseClone(type, source),
                    cloneExpression));
        }

        private sealed class ClonerCache
        {
            private readonly ConcurrentDictionary<Type, MethodInfo> _cache;

            private ClonerCache()
            {
                _cache = new ConcurrentDictionary<Type, MethodInfo>();
            }

            public static ClonerCache Instance { get; } =
                new ClonerCache();

            public MethodCallExpression Get(Type type, Expression source, Expression cache)
            {
                var genericMethod = _cache.GetOrAdd(type, t =>
                        ReflectionUtils.ObjectClone.MakeGenericMethod(t));

                return Expression.Call(genericMethod, source, cache);
            }
        }
    }
}
