#nullable enable

using System;
using System.Linq.Expressions;
using System.Reflection;
using DeepCopy.Internal;

namespace DeepCopy
{
    /// <summary>
    /// Represents a delegate that builds a custom clone expression.
    /// </summary>
    /// <param name="source"><see cref="Expression"/> indicating the source instance.</param>
    /// <param name="desitination"><see cref="Expression"/>  indicating the destination instance.</param>
    /// <param name="cache"><see cref="Expression"/>  indicating the reference caches.</param>
    /// <returns>The builded custom clone <see cref="Expression"/>.</returns>
    public delegate BlockExpression CustomCloneBuilder(
        Expression source, Expression desitination, Expression cache);

    /// <summary>
    /// Provides a helper for building custom clones.
    /// </summary>
    public static class CustomCloneHelper
    {
        /// <summary>
        /// Builds an <see cref="Expression"/> that clones fields.
        /// </summary>
        /// <param name="type">Indicates the <see cref="Type"/> of the target object.</param>
        /// <param name="source"><see cref="Expression"/> indicating the source instance.</param>
        /// <param name="desitination"><see cref="Expression"/>  indicating the destination instance.</param>
        /// <param name="cache"><see cref="Expression"/>  indicating the reference caches.</param>
        /// <param name="ignoreFields">Indicates the field names to exclude from the clone target.</param>
        /// <returns>The built <see cref="Expression"/> for cloning fields.</returns>
        public static Expression BuildCloneFieldsExpression(
            Type type, Expression source, Expression destination, Expression cache,
            params string[] ignoreFields
        ) => CoreCloneExpressionGenerator.CreateCloneExpressionInner(
            type, source, destination, cache, ignoreFields);

        /// <summary>
        /// Builds an <see cref="Expression"/> to assign value for specified field.
        /// </summary>
        /// <typeparam name="T">The type of the value to be assigned.</typeparam>
        /// <param name="desitination"><see cref="Expression"/>  indicating the destination instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="value">The value to be assigned.</param>
        /// <returns>The built <see cref="Expression"/> for assigning a field.</returns>
        public static Expression BuildFieldAssignment<T>(
            Expression destination, string fieldName, T value
        ) => Expression.Assign(
                Expression.MakeMemberAccess(
                    destination,
                    destination.Type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!),
                Expression.Constant(value, typeof(T))
            );

        /// <summary>
        /// Builds an <see cref="Expression"/> to assign value for specified field.
        /// </summary>
        /// <typeparam name="T">The type of the value to be assigned.</typeparam>
        /// <param name="desitination"><see cref="Expression"/>  indicating the destination instance.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="value"><see cref="Expression"/>  indicating the value to be assigned.</param>
        /// <returns>The built <see cref="Expression"/> for assigning a field.</returns>
        public static Expression BuildFieldAssignment(
            Expression destination, string fieldName, Expression value
        ) => Expression.Assign(
                Expression.MakeMemberAccess(
                    destination,
                    destination.Type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!),
                value
            );
    }

    public sealed class InvalidCloneBuilderException: Exception
    {
        public InvalidCloneBuilderException(Type targetType, Exception innerException)
            : base(innerException.Message, innerException)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; }
    }
}
