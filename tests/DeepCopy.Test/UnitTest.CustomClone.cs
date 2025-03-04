using System;
using System.Linq.Expressions;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void CustomCloneTest()
        {
            ObjectCloner.RegisterCustomClone(
                typeof(CustomTestClass),
                (Expression source, Expression destination, Expression cache) =>
                {
                    var fields = CustomCloneHelper.BuildCloneFieldsExpression(
                        source.Type, source, destination, cache, "_id");

                    return Expression.Block(
                        fields,
                        CustomCloneHelper.BuildFieldAssignment(
                            destination, "_id", Guid.NewGuid())
                    );
                });

            var instance = new CustomTestClass("Foo", new CustomTestValue());
            var cloned = ObjectCloner.Clone(instance);

            cloned.IsNotSameReferenceAs(instance);
            cloned.Name.Is(instance.Name);
            cloned.Value.IsNotSameReferenceAs(instance.Value);
            cloned.Value.IsStructuralEqual(instance.Value);

            cloned.Id.ToString().IsNot(instance.Id.ToString());
        }

        private sealed class CustomTestClass
        {
            /// <remark>
            /// Ensure that Id is always updated to be unique even when cloned 
            /// </remark>
            private Guid _id;

            public CustomTestClass(string name, CustomTestValue value)
            {
                _id = Guid.NewGuid();
                Name = name;
                Value = value;
            }

            public Guid Id => _id;

            public string Name { get; }

            public CustomTestValue Value { get; set; }

        }

        private sealed class CustomTestValue
        {
            public CustomTestValue()
            {
                var random = new Random(Environment.TickCount);
                Value = random.NextDouble();
            }

            public double Value { get; }
        }
    }
}
