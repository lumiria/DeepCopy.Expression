#nullable enable

using System;
using System.Linq.Expressions;

namespace DeepCopy.Internal.FixedCloners.Core
{
    internal static class ReferenceTypeCloneExpressionBuilder<T>
    {
        public static Expression Build(
            Expression source,
            Expression context)
        {
            return ReferenceTypeCloneExpressionBuilder.CreateCloneExpression(typeof(T), source, context);
        }
    }

    internal static class ReferenceTypeCloneExpressionBuilder
    {
        public static Expression Build(
            Type declaredType,
            Expression source,
            Expression context)
        {
            return CreateCloneExpression(declaredType, source, context);
        }


        internal static BlockExpression CreateCloneExpression(Type declaredType, Expression source, Expression context)
        {
            var typeVariable = Expression.Variable(typeof(Type), "type");
            var instanceVariable = Expression.Variable(declaredType, "instance");

            var returnLabel = Expression.Label(declaredType);

            var getTypeCall = Expression.Assign(
                typeVariable,
                Expression.Call(source, typeof(object).GetMethod("GetType")!)
            );

            var tryGetOrCacheCall = Expression.Call(
                Expression.Property(context, nameof(DeepCopyContext.Cache)),
                typeof(ObjectReferencesCache).GetMethod("TryGetOrCache")!.MakeGenericMethod(declaredType),
                typeVariable,
                source,
                instanceVariable
            );

            var copyToCall = CreateCopyToExpression(
                declaredType,
                typeVariable,
                source,
                instanceVariable,
                context);

            var removeLatestCall = Expression.Call(
                Expression.Property(context, nameof(DeepCopyContext.Cache)),
                typeof(ObjectReferencesCache).GetMethod(nameof(DeepCopyContext.Cache.RemoveLatest))!
            );

            return Expression.Block(
                new[] { typeVariable, instanceVariable },

                Expression.IfThen(
                    Expression.Equal(source, Expression.Constant(null, declaredType)),
                    Expression.Return(returnLabel, Expression.Default(declaredType))
                ),

                getTypeCall,

                Expression.IfThen(
                    tryGetOrCacheCall,
                    Expression.Return(returnLabel, instanceVariable)
                ),

                copyToCall,

                removeLatestCall,

                Expression.Label(returnLabel, instanceVariable)
            );
        }

        static ConditionalExpression CreateCopyToExpression(
            Type declaredType,
            ParameterExpression type,
            Expression source,
            ParameterExpression destintion,
            Expression context)
        {
            var genericType = typeof(ReferenceTypeCloneDelegateGenerator<>).MakeGenericType(declaredType);

            var delegateGenericType = genericType.GetNestedType(
                nameof(ReferenceTypeCloneDelegateGenerator<>.ReferenceTypeCloneDelegate))!
                .MakeGenericType(declaredType);
            var delegateNonGenericType = typeof(Action<object, object, DeepCopyContext>);

            var clonerGenericVarible = Expression.Variable(
                delegateGenericType, "cloner");

            var clonerNonGenericVarible = Expression.Variable(
                delegateNonGenericType, "cloner");


            var ifTrue = Expression.Block(
                new[] { clonerGenericVarible },

                Expression.Assign(
                    clonerGenericVarible,
                    Expression.Property(
                        null,
                        genericType,
                        nameof(ReferenceTypeCloneDelegateGenerator<>.Delegate)
                    )
                ),

                Expression.Invoke(clonerGenericVarible, source, destintion, context)
            );

            var ifFalse = Expression.Block(
                new[] { clonerNonGenericVarible },

                Expression.Assign(
                    clonerNonGenericVarible,
                    Expression.Call(
                        null,
                        typeof(ReferenceTypeCloneDelegateGenerator).GetMethod(nameof(ReferenceTypeCloneDelegateGenerator.CreateDelegate))!,
                        type
                    )
                ),

                Expression.Invoke(clonerNonGenericVarible, source, destintion, context)
            );

            return Expression.IfThenElse(
                Expression.Equal(type, Expression.Constant(declaredType)),
                ifTrue,
                ifFalse
            );
        }
    }
}
