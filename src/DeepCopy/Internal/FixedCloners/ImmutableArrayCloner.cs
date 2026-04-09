#if NET8_0_OR_GREATER
#nullable enable
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal.FixedCloners
{
    internal static class ImmutableArrayCloner
    {
        private static MethodInfo? _createMethod;

        private static MethodInfo CreateMethod
            => _createMethod ??= typeof(ImmutableArray)
                        .GetMethods()
                        .First(m =>
                            m.Name == "Create" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 3 &&
                            m.GetParameters()[0].ParameterType.IsArray);

        public static BlockExpression Build(
            Expression source,
            Expression destination,
            Expression cache)
        {
            var (method, elementType, arrayType) = GetCloneMethod(source.Type);
            var length = Expression.Property(source, "Length");

            var sourceArray = GetSourceArray(source);
            var destinationArray = Expression.Parameter(arrayType, "destinationArray");

            return TypeUtils.IsAssignableType(elementType)
                ? Expression.Block(
                    Expression.Assign(destination, Expression.Call(method, sourceArray, Expression.Constant(0), length))
                )
                : Expression.Block(
                    [destinationArray],
                    Expression.Assign(
                        destinationArray,
                        Expression.NewArrayBounds(elementType, length)
                    ),
                    ArrayCloner.Instance.Build(CopyPolicy.DeepCopy, arrayType, sourceArray, destinationArray, cache),
                    Expression.Assign(
                        destination,
                        Expression.Call(
                            method,
                            destinationArray,
                            Expression.Constant(0),
                            length
                        )
                    )
                );
        }

        private static (MethodInfo cloneMethod, Type elementType, Type arrayType) GetCloneMethod(Type type)
        {
            var argTypes = type.GetGenericArguments()!;

            var arrayType = argTypes[0].MakeArrayType();
            var elementType = arrayType.GetElementType()!;

            var method = CreateMethod;
            return (method.MakeGenericMethod(elementType), elementType, arrayType);
        }

        private static MemberExpression GetSourceArray(Expression source)
        {
            var field = source.Type.GetField("array", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? source.Type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(f => f.FieldType.IsArray);
            return Expression.MakeMemberAccess(source, field);
        }
    }
}
#endif
