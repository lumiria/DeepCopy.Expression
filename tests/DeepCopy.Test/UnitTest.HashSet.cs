using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DeepCopy.Test.DataTypes;
using DeepCopy.Test.Inners;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void ValueTypeHashSetTest()
        {
            var hashsetInt = new HashSet<int> { 1, 2, 3 };
            ValidateToCloneHashSet(hashsetInt);

            var hashsetString = new HashSet<string> { "AAA", "BBB", "CCC" };
            ValidateToCloneHashSet(hashsetString);

            var hashsetStruct = new HashSet<TestStruct> { new TestStruct(1, "A"), new TestStruct(2, "B") };
            ValidateToCloneHashSet(hashsetStruct);
        }

        [Fact]
        public void ReferenceTypeHashSetTest()
        {
            var hashset = new HashSet<TestKey> {
                new TestKey() { Id=1, Value = "A"}, new TestKey() { Id=2, Value = "B" }
            };

            ValidateToCloneHashSet(hashset);
        }

        [Fact]
        public void ObjectHashSetTest()
        {
            var hashset = new HashSet<object> {
                -1,  12.3, new TestStruct(1, "A"), new TestValue(), new object()
            };

            ValidateToCloneHashSet(hashset);
        }

        [Fact]
        public void ValueTypeSortedSetTest()
        {
            var setInt = new SortedSet<int> { 3, 2, 1 };
            ValidateToCloneSortedSet(setInt);

            var setString = new SortedSet<string> { "CCC", "BBB", "AAA" };
            ValidateToCloneSortedSet(setString);

            var setStruct = new SortedSet<TestStruct>(new TestStructComparer()) { new TestStruct(2, "A"), new TestStruct(1, "B") };
            ValidateToCloneSortedSet(setStruct);
        }


        private void ValidateToCloneHashSet<T>(HashSet<T> original)
#if NET8_0_OR_GREATER
            where T : notnull
#endif
        {
            var cloned = ObjectCloner.Clone(original);
            cloned.IsNotSameReferenceAs(original);

            int index = 0;
            foreach (var value in cloned)
            {
                cloned.TryGetValue(value, out var cloedValue).Is(true);

                var originalValue = original.Skip(index++).First();
                if (!originalValue.GetType().IsValueType && originalValue.GetType() != typeof(string))
                    value.IsNotSameReferenceAs(originalValue);
                cloedValue.IsStructuralEqual(originalValue);
            }
        }

        private void ValidateToCloneSortedSet<T>(SortedSet<T> original)
#if NET8_0_OR_GREATER
    where T : notnull
#endif
        {
            var cloned = ObjectCloner.Clone(original);
            cloned.IsNotSameReferenceAs(original);

            int index = 0;
            foreach (var value in cloned)
            {
                cloned.TryGetValue(value, out var cloedValue).Is(true);

                var originalValue = original.Skip(index++).First();
                if (!originalValue.GetType().IsValueType && originalValue.GetType() != typeof(string))
                    value.IsNotSameReferenceAs(originalValue);
                cloedValue.IsStructuralEqual(originalValue);
            }
        }


        private class TestStructComparer : IComparer<TestStruct>
        {
            public int Compare(TestStruct x, TestStruct y)
            {
                return x.Id - x.Id;
            }
        }
    }
}
