using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DeepCopy.Test.Inners;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void ValueTypeDictionaryTest()
        {
            var dict = new Dictionary<int, string>()
            {
                [0] = "foo",
                [1] = "bar"
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            cloned.StructuralEquals(dict);
        }

        [Fact]
        public void ReferenceTypeDictionaryTest()
        {
            var dict = new Dictionary<object, object>()
            {
                [1] = new TestObject(),
                [2] = new TestObject()
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            cloned.StructuralEquals(dict);
        }

        [Fact]
        public void NestedDictionaryTest()
        {
            var dict = new Dictionary<object, Dictionary<int, string>>()
            {
                [1] = new Dictionary<int, string>
                {
                    [0] = "foo",
                    [1] = "bar",
                },
                [2] = new Dictionary<int, string>
                {
                    [0] = "foo",
                    [1] = "bar",
                    [2] = "baz"
                }
            };
            var keys = dict.Keys; // access ...
            foreach (var key in keys)
            {
                var _ = dict[key].Keys; // access ...
            }

            var cloned = ObjectCloner.Clone(dict);

            cloned.StructuralEquals(dict);
        }

        [Fact]
        public void HasDictionaryTest()
        {
            var cls = new
            {
                Id = 1,
                Dict = new Dictionary<object, Dictionary<int, string>>()
                {
                    [1] = new Dictionary<int, string>
                    {
                        [0] = "foo",
                        [1] = "bar",
                    },
                    [2] = new Dictionary<int, string>
                    {
                        [0] = "foo",
                        [1] = "bar",
                        [2] = "baz"
                    }
                }
            };
            var keys = cls.Dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(cls);

            cloned.Id.Is(cls.Id);
            cloned.Dict.StructuralEquals(cls.Dict);
            cloned.Dict.IsNotSameReferenceAs(cls.Dict);
        }

        [Fact]
        public void ObjectKeyDictionaryTest()
        {
            var dict = new Dictionary<TestKey, TestValue>()
            {
                [new TestKey() { Id = 0, Value = "A" }] = new TestValue() { Value = "Foo" },
                [new TestKey() { Id = 1, Value = "B" }] = new TestValue() { Value = "Bar" },
            };
            var _keys = dict.Keys; // Create inner KeyCollection
            var _values = dict.Values; // Create inner ValueCollection

            var cloned = ObjectCloner.Clone(dict);

            ValidateObjectKeyDictionary(dict, cloned);
        }

        [Fact]
        public void ObjectKeyConcurrentDictionaryTest()
        {
            var dict = new ConcurrentDictionary<TestKey, TestValue>()
            {
                [new TestKey() { Id = 0, Value = "A" }] = new TestValue() { Value = "Foo" },
                [new TestKey() { Id = 1, Value = "B" }] = new TestValue() { Value = "Bar" },
            };
            //var _keys = dict.Keys; // Create inner KeyCollection
            //var _values = dict.Values; // Create inner ValueCollection

            var cloned = ObjectCloner.Clone(dict);

            ValidateObjectKeyDictionary(dict, cloned);
        }


        private void ValidateObjectKeyDictionary<T>(T original, T cloned)
            where T : IDictionary<TestKey, TestValue>
        {
            int index = 0;
            foreach (var clonedKey in cloned.Keys)
            {
                var originalKey = original.Keys.Skip(index).First();
                clonedKey.IsNotSameReferenceAs(originalKey);
                clonedKey.Id.Is(originalKey.Id);
                clonedKey.Value.Is(originalKey.Value);

                var originalValue = original.Values.Skip(index++).First();
                cloned[clonedKey].IsNotSameReferenceAs(originalValue);
                cloned[clonedKey].Is(originalValue);
            }
        }

        internal class TestKey
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        internal record class TestValue
        {
            public string Value { get; set; }
        }
    }
}
