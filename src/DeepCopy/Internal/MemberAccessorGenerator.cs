using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class MemberAccessorGenerator
    {
        public static Expression CreateGetter(Expression target, MemberInfo member) =>
            member is FieldInfo fieldInfo
                ? Expression.Field(target, fieldInfo)
                : Expression.Property(target, (PropertyInfo)member);

        public static Expression CreateSetter(Expression target, MemberInfo member, Expression value)
        {
            if (member is FieldInfo field
                && field.IsInitOnly)
            {
                return Expression.Call(
                    Expression.Constant(field),
                    ReflectionUtils.SetValue,
                    Expression.Convert(target, typeof(object)),
                    Expression.Convert(value, typeof(object)));
            }
            return Expression.Assign(
                CreateGetter(target, member),
                value);
        }

        public static Expression CreateSetter(Expression target, Expression value) =>
            Expression.Assign(
                target,
                value);
    }
}
