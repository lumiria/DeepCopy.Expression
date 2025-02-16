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

            ValidateDictionary(dict, cloned);
        }

        [Fact]
        public void ReferenceTypeDictionaryTest()
        {
            var dict = new Dictionary<object, object>()
            {
                [1] = new TestObject(),
                [2] = new TestObject(),
                [new object()] = new object(),
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            ValidateDictionary(dict, cloned);
        }

        [Fact]
        public void ObjectDictionaryTest()
        {
            var dict = new Dictionary<object, object>()
            {
                [1] = 12.3,
                [new TestKey() { Id = 1, Value = "A" }] = new TestValue(),
                [new object()] = new object(),
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            ValidateDictionary(dict, cloned);
        }

        [Fact]
        public void ValueTypeArrayDictionaryTest()
        {
            var dict = new Dictionary<int[], int[]>()
            {
                [new int[] { 1, 2, 3 }] = new int[] { 4, 5, 6 },
                [new int[] { 2, 4, 6 }] = new int[] { 1, 2, 4 }
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            ValidateDictionary(dict, cloned);
        }

        [Fact]
        public void ReferenceTypeArrayDictionaryTest()
        {
            var dict = new Dictionary<TestKey[], TestValue[]>()
            {
                [new TestKey[] { new TestKey(), new TestKey() }] = new TestValue[] { new TestValue(), new TestValue() },
                [new TestKey[] { new TestKey()}] = new TestValue[] { new TestValue() },
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            ValidateDictionary(dict, cloned);
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
        public void ObjectKeyDictionaryTest()
        {
            var dict = new Dictionary<TestKey, TestValue>()
            {
                [new TestKey() { Id = -1, Value = "A" }] = new TestValue() { Value = "Foo" },
                [new TestKey() { Id = 0, Value = "B" }] = new TestValue() { Value = "Bar" },
            };

            var cloned = ObjectCloner.Clone(dict);

            ValidateObjectKeyDictionary(dict, cloned);
        }

        [Fact]
        public void ObjectKeyConcurrentDictionaryTest()
        {
            var dict = new ConcurrentDictionary<TestKey, TestValue>()
            {
                [new TestKey() { Id = -1, Value = "A" }] = new TestValue() { Value = "Foo" },
                [new TestKey() { Id = 0, Value = "B" }] = new TestValue() { Value = "Bar" },
            };

            var cloned = ObjectCloner.Clone(dict);

            ValidateObjectKeyDictionary(dict, cloned);
        }

        [Fact]
        public void InnerDictionaryTest()
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



        private void ValidateDictionary<TKey, TValue>(Dictionary<TKey, TValue> original, Dictionary<TKey, TValue> cloned)
#if NET8_0_OR_GREATER

            where TKey: notnull
#endif
        {
            cloned.StructuralEquals(original);

            int index = 0;
            foreach (var clonedKey in cloned.Keys)
            {
                var originalKey = original.Keys.Skip(index).First();
                if (!originalKey.GetType().IsValueType && originalKey.GetType() != typeof(string))
                    clonedKey.IsNotSameReferenceAs(originalKey);
                clonedKey.IsStructuralEqual(originalKey);

                var originalValue = original.Values.Skip(index++).First();
                if (!(originalValue?.GetType()?.IsValueType ?? false) && originalValue?.GetType() != typeof(string))
                    cloned[clonedKey].IsNotSameReferenceAs(originalValue);
                cloned[clonedKey].IsStructuralEqual(originalValue);
            }
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
                cloned[clonedKey].IsStructuralEqual(originalValue);
            }
        }

        internal class TestKey
        {
            public int Id { get; set; }
#if NET8_0_OR_GREATER
            public string? Value { get; set; }
#else
            public string Value { get; set; }
#endif
        }


#if NET8_0_OR_GREATER
        internal record class TestValue
        {
            public string? Value { get; set; }
        }
#else
        internal class TestValue
        {
            public TestValue() : this(null) {}

            public TestValue(string value)
            {
                Value = value;
            }

            public string Value { get; set; }
        }
#endif
    }
}
