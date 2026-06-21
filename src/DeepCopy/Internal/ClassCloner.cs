using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.FixedCloners.Core;
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
                Expression context)
        {

            if (type != typeof(object))
            {
                var cloneExpression = ClonerCache.Instance.Get(type, source, context);
                return Expression.Assign(destination, cloneExpression);
            }

            return Expression.Assign(
                destination,
                ExpressionUtils.NullTernaryCheck(
                    source,
                    Expression.Condition(
                        ExpressionUtils.IsObjectOrValueType(source),
                        ExpressionUtils.MemberwiseClone(type, source),
                        ExpressionUtils.MaybeCloneObjectType(source, context))));
        }

        public Expression Build(
                Type type,
                Expression source,
                Expression destination,
                MemberInfo member,
                Expression context)
        {
            if (type != typeof(object))
            {
                var cloneExpression = ClonerCache.Instance.Get(type, source, context);
                return MemberAccessorGenerator.CreateSetter(
                    destination, member, cloneExpression);
            }

            return MemberAccessorGenerator.CreateSetter(
                destination,
                member,
                Expression.Condition(
                    ExpressionUtils.IsObjectOrValueType(source),
                    ExpressionUtils.MemberwiseClone(type, source),
                    ExpressionUtils.MaybeCloneObjectType(source, context)));
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

            public Expression Get(Type type, Expression source, Expression context)
                => Expression.Call(
                    _cache.GetOrAdd(type, t => CloneExpressionBuilder.GetCloneMethod(t, source)),
                    source,
                    context);
        }
    }
}
