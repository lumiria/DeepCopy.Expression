using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void RecordClassTest()
        {
            var source = new PersonRecord("John", new PersonChild { Name = "Child1" });

            var cloned = ObjectCloner.Clone(source);

            Assert.NotSame(source, cloned);
            Assert.Equal(source.Name, cloned.Name);
            Assert.NotSame(source.Child, cloned.Child);
            Assert.Equal(source.Child.Name, cloned.Child.Name);

            cloned.Child.Name = "changed";

            Assert.Equal("Child1", source.Child.Name);
            Assert.Equal("changed", cloned.Child.Name);
        }

        [Fact]
        public void RecordStructTest()
        {
            var source = new PersonRecord("John", new PersonChild { Name = "Child1" });

            var cloned = ObjectCloner.Clone(source);

            cloned.IsStructuralEqual(source);
            Assert.NotSame(source.Child, cloned.Child);

            cloned.Child.Name = "changed";

            Assert.Equal("Child1", source.Child.Name);
            Assert.Equal("changed", cloned.Child.Name);
        }


        private record PersonRecord(
            string Name,
            PersonChild Child);

        private record struct PersonRecordStruct(
            string Name,
            PersonChild Child);

        private sealed class PersonChild
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
