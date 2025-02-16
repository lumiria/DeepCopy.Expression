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


        //[Fact]
        //public void ReferenceTypeHashSetTest()
        //{
        //    var dict = new Dictionary<object, object>()
        //    {
        //        [1] = new TestObject(),
        //        [2] = new TestObject(),
        //        [new object()] = new object(),
        //    };
        //    var keys = dict.Keys; // access ...

        //    var cloned = ObjectCloner.Clone(dict);

        //    ValidateHashSet(dict, cloned);
        //}



        //[Fact]
        //public void ValueTypeArrayHashSetTest()
        //{
        //    var dict = new Dictionary<int[], int[]>()
        //    {
        //        [[1, 2, 3]] = [4, 5, 6],
        //        [[2, 4, 6]] = [1, 2, 4]
        //    };
        //    var keys = dict.Keys; // access ...

        //    var cloned = ObjectCloner.Clone(dict);

        //    ValidateHashSet(dict, cloned);
        //}

        //[Fact]
        //public void ReferenceTypeArrayHashSetTest()
        //{
        //    var dict = new Dictionary<TestKey[], TestValue[]>()
        //    {
        //        [[new(), new()]] = [new(), new()],
        //        [[new()]] = [new()],
        //    };
        //    var keys = dict.Keys; // access ...

        //    var cloned = ObjectCloner.Clone(dict);

        //    ValidateHashSet(dict, cloned);
        //}

        //[Fact]
        //public void NestedHashSetTest()
        //{
        //    var dict = new Dictionary<object, Dictionary<int, string>>()
        //    {
        //        [1] = new Dictionary<int, string>
        //        {
        //            [0] = "foo",
        //            [1] = "bar",
        //        },
        //        [2] = new Dictionary<int, string>
        //        {
        //            [0] = "foo",
        //            [1] = "bar",
        //            [2] = "baz"
        //        }
        //    };
        //    var keys = dict.Keys; // access ...
        //    foreach (var key in keys)
        //    {
        //        var _ = dict[key].Keys; // access ...
        //    }

        //    var cloned = ObjectCloner.Clone(dict);

        //    cloned.StructuralEquals(dict);
        //}

        //[Fact]
        //public void ObjectKeyHashSetTest()
        //{
        //    var dict = new Dictionary<TestKey, TestValue>()
        //    {
        //        [new TestKey() { Id = -1, Value = "A" }] = new TestValue() { Value = "Foo" },
        //        [new TestKey() { Id = 0, Value = "B" }] = new TestValue() { Value = "Bar" },
        //    };

        //    var cloned = ObjectCloner.Clone(dict);

        //    ValidateObjectKeyHashSet(dict, cloned);
        //}

        //[Fact]
        //public void ObjectKeyConcurrentHashSetTest()
        //{
        //    var dict = new ConcurrentDictionary<TestKey, TestValue>()
        //    {
        //        [new TestKey() { Id = -1, Value = "A" }] = new TestValue() { Value = "Foo" },
        //        [new TestKey() { Id = 0, Value = "B" }] = new TestValue() { Value = "Bar" },
        //    };

        //    var cloned = ObjectCloner.Clone(dict);

        //    ValidateObjectKeyHashSet(dict, cloned);
        //}

        //[Fact]
        //public void InnerHashSetTest()
        //{
        //    var cls = new
        //    {
        //        Id = 1,
        //        Dict = new Dictionary<object, Dictionary<int, string>>()
        //        {
        //            [1] = new Dictionary<int, string>
        //            {
        //                [0] = "foo",
        //                [1] = "bar",
        //            },
        //            [2] = new Dictionary<int, string>
        //            {
        //                [0] = "foo",
        //                [1] = "bar",
        //                [2] = "baz"
        //            }
        //        }
        //    };
        //    var keys = cls.Dict.Keys; // access ...

        //    var cloned = ObjectCloner.Clone(cls);

        //    cloned.Id.Is(cls.Id);
        //    cloned.Dict.StructuralEquals(cls.Dict);
        //    cloned.Dict.IsNotSameReferenceAs(cls.Dict);
        //}



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


        //private void ValidateObjectKeyHashSet<T>(T original, T cloned)
        //    where T : IDictionary<TestKey, TestValue>
        //{
        //    int index = 0;
        //    foreach (var clonedKey in cloned.Keys)
        //    {
        //        var originalKey = original.Keys.Skip(index).First();
        //        clonedKey.IsNotSameReferenceAs(originalKey);
        //        clonedKey.Id.Is(originalKey.Id);
        //        clonedKey.Value.Is(originalKey.Value);

        //        var originalValue = original.Values.Skip(index++).First();
        //        cloned[clonedKey].IsNotSameReferenceAs(originalValue);
        //        cloned[clonedKey].Is(originalValue);
        //    }
        //}

        //internal class TestKey
        //{
        //    public int Id { get; set; }
        //    public string? Value { get; set; }
        //}

        //internal record class TestValue
        //{
        //    public string? Value { get; set; }
        //}
    }
}
