using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class CloneArrayExpressionGenerator<T>
    {
        private static readonly Type _type;
        private static Func<T[], T[]> _delegate;

        static CloneArrayExpressionGenerator()
        {
            _type = typeof(T[]);
        }

        public static void Clearnup() =>
            _delegate = null;

        public static Func<T[], T[]> CreateDelegate() =>
            _delegate ?? (_delegate = Create().Compile());

        private static Expression<Func<T[], T[]>> Create()
        {
            var sourceParameter = Expression.Parameter(_type, "source");

            var body = CreateCloneExpression(sourceParameter);

            return Expression.Lambda<Func<T[], T[]>>(
                body,
                sourceParameter);
        }

        private static Expression CreateCloneExpression(ParameterExpression source)
        {
            var destination = Expression.Parameter(_type, "destination");

            return Expression.Block(
                new[] { destination },
                ArrayCloner.Instance.Build(
                    typeof(T),//_type,
                    source,
                    destination),
                destination);
        }
    }
}
