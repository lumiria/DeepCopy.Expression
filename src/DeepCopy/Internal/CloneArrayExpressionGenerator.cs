using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal
{
    internal static class CloneArrayExpressionGenerator<T, TArray>
    {
        private static readonly Type _type;
        private static readonly Func<TArray, DeepCopyContext, TArray> _delegate;

        static CloneArrayExpressionGenerator()
        {
            _type = typeof(TArray);
            _delegate = Create().Compile();
        }

        public static void Cleanup()
        {
            var field = typeof(CloneArrayExpressionGenerator<T, TArray>).GetField(nameof(_delegate),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field.SetValue(null, null);
        }

        public static Func<TArray, DeepCopyContext, TArray> Delegate => _delegate;

        private static Expression<Func<TArray, DeepCopyContext, TArray>> Create()
        {
            var sourceParameter = Expression.Parameter(_type, "source");
            var contextParameter = Expression.Parameter(typeof(DeepCopyContext), "context");

            var body = CreateCloneExpression(sourceParameter, contextParameter);

            return Expression.Lambda<Func<TArray, DeepCopyContext, TArray>>(
                body,
                sourceParameter, contextParameter);
        }

        private static Expression CreateCloneExpression(ParameterExpression source, ParameterExpression context)
        {
            var destination = Expression.Parameter(_type, "destination");

            return Expression.Block(
                [destination],
                ArrayCloner.Instance.Build(
                    _type,
                    source,
                    destination,
                    context),
                destination);
        }
    }
}
