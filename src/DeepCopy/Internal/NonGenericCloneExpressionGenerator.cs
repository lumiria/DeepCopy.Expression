using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class CloneExpressionGenerator
    {
        private static readonly ConcurrentDictionary<Type, Action<object, object>> _caches;

        static CloneExpressionGenerator()
        {
            _caches = new ConcurrentDictionary<Type, Action<object, object>>();
        }

        public static object NewInstance(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }

        public static Action<object, object> CreateDelegate(Type type) =>
            _caches.GetOrAdd(type, x => Create(x).Compile());

        private static Expression<Action<object, object>> Create(Type type)
        {
            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var destinationParameter = Expression.Parameter(typeof(object), "destination");

            var body = CreateCloneExpression(
                type,
                Expression.Convert(sourceParameter, type),
                Expression.Convert(destinationParameter, type));

            return Expression.Lambda<Action<object, object>>(
                body,
                sourceParameter, destinationParameter);
        }

        private static Expression CreateCloneExpression(Type type, Expression source, Expression destination)
        {
            var targets = CopyMemberExtractor.Extract(type);

            //System.Diagnostics.Debug.WriteLine($"{typeof(T)}");
            //foreach (var target in targets)
            //{
            //    var type = target.Item1 is FieldInfo field ? field.FieldType
            //        : ((PropertyInfo)target.Item1).PropertyType;
            //    System.Diagnostics.Debug.WriteLine($" - {type} : {target.Item1.Name}");
            //}

            var expressions = targets.Select(t =>
                CreateCloneMemberExpression(source, destination, t.Item1, t.Item2));
            return expressions.Any()
                ? (Expression)Expression.Block(expressions)
                : Expression.Empty();
        }

        private static Expression CreateCloneMemberExpression(
            Expression source,
            Expression destination,
            MemberInfo member,
            InnerCopyPolicy copyPolicy)
        {
            var value = MemberAccessorGenerator.CreateGetter(source, member);

            if (copyPolicy == InnerCopyPolicy.Assign)
            {
                return MemberAccessorGenerator.CreateSetter(destination, member, value);
            }

            var memberType = value.Type;
            Expression body = null;
            if (copyPolicy == InnerCopyPolicy.MemberwiseClone)
            {
                body = MemberAccessorGenerator.CreateSetter(
                    destination,
                    member,
                    Expression.Convert(
                        Expression.Call(value, ReflectionUtils.MemberwizeClone),
                        memberType));
            }
            else if (memberType.IsArray)
            {
                body = ArrayCloner.Instance.Build(
                    copyPolicy,
                    memberType,
                    value,
                    destination,
                    member);
            }
            else
            {
                body = ClassCloner.Instance.Build(
                    memberType,
                    value,
                    destination,
                    member);
            }

            return ExpressionUtils.NullCheck(value, body);
        }
    }
}
