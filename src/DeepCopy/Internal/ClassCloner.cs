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

            var memberwiseCloneExpression = ExpressionUtils.MemberwiseClone(type, source);
            return Expression.Assign(
                destination,
                ExpressionUtils.NullTernaryCheck(
                    source,
                    Expression.Condition(
                        ExpressionUtils.IsObjectOrValueType(source),
                        ExpressionUtils.MemberwiseClone(type, source),
                        cloneExpression)));
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
                    ExpressionUtils.IsObjectOrValueType(source),
                    ExpressionUtils.MemberwiseClone(type, source),
                    cloneExpression));
        }

        private sealed class ClonerCache
        {
            private readonly ConcurrentDictionary<Type, MethodInfo> _cache;

            private ClonerCache()
            {
                _cache = new ConcurrentDictionary<Type, MethodInfo>(-1, 10);
            }

            public static ClonerCache Instance { get; } =
                new ClonerCache();

            public MethodCallExpression Get(Type type, Expression source, Expression cache)
            {
                var genericMethod = _cache.GetOrAdd(type, t =>
                        !TypeUtils.IsValueType(t)
                            ? type.IsInterface
                                ? ReflectionUtils.InterfaceClone.MakeGenericMethod(t)
                                : ReflectionUtils.ObjectClone.MakeGenericMethod(t)
                            : ReflectionUtils.ValueClone.MakeGenericMethod(t));

                return Expression.Call(genericMethod, source, cache);
            }
        }
    }
}
