using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class CloneExpressionGenerator
    {
        private static readonly ConcurrentDictionary<Type, Action<object, object, ObjectReferencesCache>> _caches;

        static CloneExpressionGenerator()
        {
            _caches = new ConcurrentDictionary<Type, Action<object, object, ObjectReferencesCache>>();
        }

        public static Action<object, object, ObjectReferencesCache> CreateDelegate(Type type) =>
            _caches.GetOrAdd(type, t => Create(t).Compile());

        private static Expression<Action<object, object, ObjectReferencesCache>> Create(Type type)
        {
            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var destinationParameter = Expression.Parameter(typeof(object), "destination");
            var cacheParameter = Expression.Parameter(typeof(ObjectReferencesCache), "cache");

            var body = CreateCloneExpression(
                type,
                Expression.Convert(sourceParameter, type),
                Expression.Convert(destinationParameter, type),
                cacheParameter);

            return Expression.Lambda<Action<object, object, ObjectReferencesCache>>(
                body,
                sourceParameter, destinationParameter, cacheParameter);
        }

        private static Expression CreateCloneExpression(Type type,
            Expression source, Expression destination, Expression cache)
        {
            var targets = CopyMemberExtractor.Extract(type);

            //System.Diagnostics.Debug.WriteLine($"{typeof(T)}");
            //foreach (var target in targets)
            //{
            //    var type = target.Item1 is FieldInfo field ? field.FieldType
            //        : ((PropertyInfo)target.Item1).PropertyType;
            //    System.Diagnostics.Debug.WriteLine($" - {type} : {target.Item1.Name}");
            //}

            var expressions = new ReadOnlyCollectionBuilder<Expression>(
                CreateExpressions(targets, source, destination, cache));
            //var expressions = CreateExpressions(targets, source, destination);
            return expressions.Any()
                ? (Expression)Expression.Block(expressions)
                : Expression.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<Expression> CreateExpressions(
            IEnumerable<(MemberInfo, CopyPolicy)> targets,
            Expression source, Expression destination, Expression cache)
        {
            foreach (var target in targets)
               yield return CreateCloneMemberExpression(source, destination, cache, target.Item1, target.Item2);
        }

        private static Expression CreateCloneMemberExpression(
            Expression source,
            Expression destination,
            Expression cache,
            MemberInfo member,
            CopyPolicy copyPolicy)
        {
            var value = MemberAccessorGenerator.CreateGetter(source, member);

            if (copyPolicy == CopyPolicy.Assign)
            {
                return MemberAccessorGenerator.CreateSetter(destination, member, value);
            }

            var memberType = value.Type;
            Expression body = null;
            if (memberType.IsArray)
            {
                body = ArrayCloner.Instance.Build(
                    copyPolicy,
                    memberType,
                    value,
                    destination,
                    member,
                    cache);
            }
            else if (copyPolicy == CopyPolicy.ShallowCopy)
            {
                body = MemberAccessorGenerator.CreateSetter(
                    destination,
                    member,
                    Expression.Convert(
                        Expression.Call(value, ReflectionUtils.MemberwizeClone),
                        memberType));
            }
            else
            {
                body = ClassCloner.Instance.Build(
                    memberType,
                    value,
                    destination,
                    member,
                    cache);
            }

            return ExpressionUtils.NullCheck(value, body);
        }
    }
}
