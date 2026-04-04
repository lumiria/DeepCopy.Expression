using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.BuiltIns;
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

            if (type != typeof(object))
            {
                var cloneExpression = ClonerCache.Instance.Get(type, source, cache);
                return Expression.Assign(destination, cloneExpression);
            }

            return Expression.Assign(
                destination,
                ExpressionUtils.NullTernaryCheck(
                    source,
                    Expression.Condition(
                        ExpressionUtils.IsObjectOrValueType(source),
                        ExpressionUtils.MemberwiseClone(type, source),
                        ExpressionUtils.CloneObjectType(source, cache))));
        }

        public Expression Build(
                Type type,
                Expression source,
                Expression destination,
                MemberInfo member,
                Expression cache)
        {
            if (type != typeof(object))
            {
                var cloneExpression = ClonerCache.Instance.Get(type, source, cache);
                return MemberAccessorGenerator.CreateSetter(
                    destination, member, cloneExpression);
            }

            return MemberAccessorGenerator.CreateSetter(
                destination,
                member,
                Expression.Condition(
                    ExpressionUtils.IsObjectOrValueType(source),
                    ExpressionUtils.MemberwiseClone(type, source),
                    Expression.Condition(
                        ExpressionUtils.IsArray(source),
                        Expression.Convert(
                            ExpressionUtils.CloneArray(Expression.Convert(source, typeof(Array)), cache),
                            typeof(object)),
                        ExpressionUtils.CloneObjectType(source, cache))));
        }

        private sealed class ClonerCache
        {
            private readonly ConcurrentDictionary<Type, MethodInfo> _cache;

            private ClonerCache()
            {
#if NETSTANDARD2_0
                _cache = new ConcurrentDictionary<Type, MethodInfo>(Environment.ProcessorCount, 10);
#else
                _cache = new ConcurrentDictionary<Type, MethodInfo>(-1, 10);
#endif
            }

            public static ClonerCache Instance { get; } =
                new ClonerCache();

            public MethodCallExpression Get(Type type, Expression source, Expression cache)
            {
                var genericMethod = _cache.GetOrAdd(type, t => t switch
                {
                    _ when Nullable.GetUnderlyingType(t) is Type nullableType
                        => ReflectionUtils.NullableValueClone.MakeGenericMethod(nullableType),

                    _ when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                        => FixedDictionaryCloner.GetCloneMethod(t),

                    _ when t == typeof(Array)
                        => ReflectionUtils.ArrayClone,

                    _ when !TypeUtils.IsValueType(t)
                        => (type.IsInterface
                            ? ReflectionUtils.InterfaceClone.MakeGenericMethod(t)
                            : ReflectionUtils.ObjectClone.MakeGenericMethod(t)),

                    _ => ReflectionUtils.ValueClone.MakeGenericMethod(t)
                });

                return Expression.Call(genericMethod, source, cache);
            }
        }
    }
}
