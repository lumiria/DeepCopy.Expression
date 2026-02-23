using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DeepCopy.Test.DataTypes;
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
            var dict = new Dictionary<TestKey, TestValue>()
            {
                [new TestKey() {  Id = 1 }] = new TestValue() { Value = "Foo" },
                [new TestKey() {  Id = 2 }] = new TestValue() { Value = "Bar" },
                [new TestKey() {  Id = 3 }] = new TestValue() { Value = "Baz" },
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
                [new TestKey() { Id = 1, Value = "C" }] = new TestValue() { Value = "Baz" },
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

        [Fact]
        public void ObjectAsDictionaryTest()
        {
            object obj = new Dictionary<TestKey, TestValue>()
            {
                [new TestKey() { Id = 1 }] = new TestValue() { Value = "Foo" },
                [new TestKey() { Id = 2 }] = new TestValue() { Value = "Bar" },
                [new TestKey() { Id = 3 }] = new TestValue() { Value = "Baz" },
            };

            var cloned = ObjectCloner.Clone(obj);

            ValidateDictionary((Dictionary<TestKey, TestValue>)obj, (Dictionary<TestKey, TestValue>)cloned);
        }

        [Fact]
        public void ValueTypeReadOnlyDictionaryTest()
        {
            var dict = new ReadOnlyDictionary<int,string>(
                new Dictionary<int, string>()
                {
                    [0] = "foo",
                    [1] = "bar"
                });
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            ValidateDictionary(dict, cloned);
        }

        [Fact]
        public void ReferenceTypeReadOnlyDictionaryTest()
        {
            var dict = new ReadOnlyDictionary<TestKey, TestValue>(
                new Dictionary<TestKey, TestValue>()
                {
                    [new TestKey() { Id = 1 }] = new TestValue() { Value = "Foo" },
                    [new TestKey() { Id = 2 }] = new TestValue() { Value = "Bar" },
                    [new TestKey() { Id = 3 }] = new TestValue() { Value = "Baz" },
                });
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            ValidateDictionary(dict, cloned);
        }

        [Fact]
        public void DictionaryValueSemanticsTest()
        {
            var dict = new Dictionary<int, Guid>
                {
                    [0] = Guid.NewGuid(),
                    [1] = Guid.NewGuid(),
                    [2] = Guid.NewGuid(),
                };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryNullableValueSemanticsTest()
        {
            var dict = new Dictionary<int, Guid?>
            {
                [0] = Guid.NewGuid(),
                [1] = null,
                [2] = Guid.NewGuid(),
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsKeyTest()
        {
            var dict = new Dictionary<Guid, Uri>
            {
                [Guid.NewGuid()] = new Uri("https://example.com/value-01"),
                [Guid.NewGuid()] = new Uri("https://example.com/value-02"),
                [Guid.NewGuid()] = new Uri("https://example.com/value-03"),
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryNullableValueSemanticsKeyTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<Guid, Uri?>
#else
            var dict = new Dictionary<Guid, Uri>
#endif
            {
                [Guid.NewGuid()] = new Uri("https://example.com/value-01"),
                [Guid.NewGuid()] = null,
                [Guid.NewGuid()] = new Uri("https://example.com/value-03"),
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsKeyAndDividedValueTest()
        {
            var dict = new Dictionary<int, BaseSample>
            {
                [0] = new SubSample(1, "name-1"),
                [1] = new SubSample(2, "name-2"),
                [2] = new SubSample(3, "name-3")
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryNullableValueSemanticsKeyAndDividedValueTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<int, BaseSample?>
#else
            var dict = new Dictionary<int, BaseSample>
#endif
            {
                [0] = new SubSample(1, "name-1"),
                [1] = null,
                [2] = new SubSample(3, "name-3")
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsKeyAndObjectValueTest()
        {
            var dict = new Dictionary<DateTime, object>
            {
                [DateTime.Now] = 1,
                [DateTime.Now] = new Uri("https://example.com/value-02"),
                [DateTime.Now] = Guid.NewGuid(),
                [DateTime.Now] = new object()
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryNullableValueSemanticsKeyAndObjectValueTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<DateTime, object?>
#else
            var dict = new Dictionary<DateTime, object>
#endif
            {
                [DateTime.Now] = 1,
                [DateTime.Now] = new Uri("https://example.com/value-02"),
                [DateTime.Now] = Guid.NewGuid(),
                [DateTime.Now] = null,
                [DateTime.Now] = new object()
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsKeyAndArrayValueTest()
        {
            var dict = new Dictionary<int, string[]>
            {
                [0] = new string[] { "one", "two" },
                [1] = new string[] { "three", "four" },
                [2] = new string[] { "five", "six" },
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryNullableValueSemanticsKeyAndArrayValueTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<int, string[]?>
#else
            var dict = new Dictionary<int, string[]>
#endif
            {
                [0] = new string[] { "one", "two" },
                [1] = null,
                [2] = new string[0],
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsKeyAndArrayRefValueTest()
        {
            var dict = new Dictionary<int, Uri[]>
            {
                [0] = new Uri[] { new Uri("https://example.com/value-01"), new Uri("https://example.com/value-02") },
                [1] = new Uri[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") },
                [2] = new Uri[] { new Uri("https://example.com/value-05"), new Uri("https://example.com/value-06") },
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryNullableValueSemanticsKeyAndArrayRefValueTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<int, Uri[]?>
#else
            var dict = new Dictionary<int, Uri[]>
#endif
            {
                [0] = new Uri[] { new Uri("https://example.com/value-01"), new Uri("https://example.com/value-02") },
                [1] = null,
                [2] = new Uri[0],
            };
            TestDictionaryCore(dict);
        }



        [Fact]
        public void DictionaryValueSemanticsKeyAndObjectArrayValueTest()
        {
            var dict = new Dictionary<int, object[]>
            {
                [0] = new object[] { 0, 1 },
                [1] = new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") },
                [2] = new object[] { "5", new Uri("https://example.com/value-06") },
                [3] = new object[0],
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryNullableValueSemanticsKeyAndObjectArrayValueTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<int, object[]?>
#else
            var dict = new Dictionary<int, object[]>
#endif
            {
                [0] = new object[] { 0, 1 },
                [1] = new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") },
                [2] = new object[] { "5", new Uri("https://example.com/value-06") },
                [3] = new object[0],
                [4] = null,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsValueTest()
        {
            var dict = new Dictionary<Uri, Guid>
            {
                [new Uri("https://example.com/value-01")] = Guid.NewGuid(),
                [new Uri("https://example.com/value-02")] = Guid.NewGuid(),
                [new Uri("https://example.com/value-03")] = Guid.NewGuid(),
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsValueAndDividedKeyTest()
        {
            var dict = new Dictionary<BaseSample, int>
            {
                [new SubSample(1, "name-1")] = 1,
                [new SubSample(2, "name-2")] = 2,
                [new SubSample(3, "name-3")] = 3,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsValueAndObjectKeyTest()
        {
            var dict = new Dictionary<object, DateTime>
            {
                [1] = DateTime.Now,
                [new Uri("https://example.com/value-02")] = DateTime.Now,
                [Guid.NewGuid()] = DateTime.Now,
                [new object()] = DateTime.Now,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsValueAndArrayKeyTest()
        {
            var dict = new Dictionary<string[], int>
            {
                [new string[] { "one", "two" }] = 1,
                [new string[] { "three", "four" }] = 2,
                [new string[] { "five", "six" }] = 3,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsValueAndArrayRefKeyTest()
        {
            var dict = new Dictionary<Uri[], int>
            {
                [new Uri[] { new Uri("https://example.com/value-01"), new Uri("https://example.com/value-02") }] = 1,
                [new Uri[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }] = 2,
                [new Uri[] { new Uri("https://example.com/value-05"), new Uri("https://example.com/value-06") }] = 3,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryValueSemanticsValueAndObjectArrayKeyTest()
        {
            var dict = new Dictionary<object[], int>
            {
                [new object[] { 0, 1 }] = 0,
                [new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }] = 1,
                [new object[] { "5", new Uri("https://example.com/value-06") }] = 2,
                [new object[0]] = 3,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectKeyTest()
        {
            var dict = new Dictionary<object, SubSample>
            {
                [1] = new SubSample(1, "name-1"),
                [new Uri("https://example.com/value-02")] = new SubSample(2, "name-2"),
                [Guid.NewGuid()] = new SubSample(3, "name-3"),
                [new object()] = new SubSample(4, "name-4"),
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectKeyNullableTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<object, BaseSample?>
#else
            var dict = new Dictionary<object, BaseSample>
#endif
            {
                [1] = new SubSample(1, "name-1"),
                [new Uri("https://example.com/value-02")] = new SubSample(2, "name-2"),
                [Guid.NewGuid()] = new SubSample(3, "name-3"),
                [new object()] = new SubSample(4, "name-4"),
                ["key-5"] = null
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectKeyValueTest()
        {
            var dict = new Dictionary<object, object>
            {
                [1] = new object(),
                [new Uri("https://example.com/value-02")] = Guid.NewGuid(),
                [Guid.NewGuid()] = new Uri("https://example.com/value-02"),
                [new object()] = 1,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectKeyValueNullableTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<object, object?>
#else
            var dict = new Dictionary<object, object>
#endif
            {
                [1] = new object(),
                [new Uri("https://example.com/value-02")] = Guid.NewGuid(),
                [Guid.NewGuid()] = new Uri("https://example.com/value-02"),
                [new object()] = 1,
                ["key-5"] = null,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectKeyAndArrayValueTest()
        {
            var dict = new Dictionary<object, BaseSample[]>
            {
                [1] = new SubSample[] { new SubSample(1, "name-1"), new SubSample(2, "name-2") },
                [new Uri("https://example.com/value-02")] = new SubSample[] { new SubSample(3, "name-3"), new SubSample(4, "name-4") },
                [Guid.NewGuid()] = new SubSample[] { new SubSample(5, "name-5"), new SubSample(6, "name-6") },
                [new object()] = new SubSample[] { new SubSample(7, "name-7"), new SubSample(8, "name-8") },
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectKeyAndArrayValueNullableTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<object, BaseSample[]?>
#else
            var dict = new Dictionary<object, BaseSample[]>
#endif
            {
                [1] = new SubSample[] { new SubSample(1, "name-1"), new SubSample(2, "name-2") },
                [new Uri("https://example.com/value-02")] = new SubSample[] { new SubSample(3, "name-3"), new SubSample(4, "name-4") },
                [Guid.NewGuid()] = new SubSample[] { new SubSample(5, "name-5"), new SubSample(6, "name-6") },
                [new object()] = new SubSample[] { new SubSample(7, "name-7"), new SubSample(8, "name-8") },
                ["key-5"] = null,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectArrayKeyTest()
        {
            var dict = new Dictionary<object[], SubSample>
            {
                [new object[] { 0, 1 }] = new SubSample(1, "name-1"),
                [new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }] = new SubSample(2, "name-2"),
                [new object[] { "5", new Uri("https://example.com/value-06") }] = new SubSample(3, "name-3"),
                [new object[0]] = new SubSample(4, "name-4"),
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectArrayKeyNullableTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<object[], SubSample?>
#else
            var dict = new Dictionary<object[], SubSample>
#endif
            {
                [new object[] { 0, 1 }] = new SubSample(1, "name-1"),
                [new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }] = new SubSample(2, "name-2"),
                [new object[] { "5", new Uri("https://example.com/value-06") }] = new SubSample(3, "name-3"),
                [new object[0]] = new SubSample(4, "name-4"),
                [new object[] { 2, 3 }] = null,
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectArrayKeyValueTest()
        {
            var dict = new Dictionary<object[], object[]>
            {
                [new object[] { 0, 1 }] = new object[] { 2, 0 },
                [new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }] = new object[] { new Uri("https://example.com/value-04"), new Uri("https://example.com/value-02") },
                [new object[] { "5", new Uri("https://example.com/value-06") }] = new object[] { "6", new Uri("https://example.com/value-05") },
                [new object[0]] = new object[0]
            };
            TestDictionaryCore(dict);
        }

        [Fact]
        public void DictionaryObjectArrayKeyValueNullableTest()
        {
#if NET8_0_OR_GREATER
            var dict = new Dictionary<object[], object[]?>
#else
            var dict = new Dictionary<object[], object[]>
#endif
            {
                [new object[] { 0, 1 }] = new object[] { 2, 0 },
                [new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }] = new object[] { new Uri("https://example.com/value-04"), new Uri("https://example.com/value-02") },
                [new object[] { "5", new Uri("https://example.com/value-06") }] = new object[] { "6", new Uri("https://example.com/value-05") },
                [new object[0]] = new object[0],
                [new object[] { 2, 3 }] = null,
            };
            TestDictionaryCore(dict);
        }

        private void TestDictionaryCore<TKey, TValue>(Dictionary<TKey, TValue> dict)
#if NET8_0_OR_GREATER
            where TKey : notnull
#endif
        {
            var clonedDict = ObjectCloner.Clone(dict);
            ValidateDictionary(dict, clonedDict);

            var obj = new
            {
                Dict = dict,
            };
            var clonedObj = ObjectCloner.Clone(obj);
            clonedObj.IsNotSameReferenceAs(obj);
            ValidateDictionary(obj.Dict, clonedObj.Dict);

            var readonlyDict = new ReadOnlyDictionary<TKey, TValue>(dict);
            var clonedReadonlyDict = ObjectCloner.Clone(readonlyDict);
            ValidateDictionary(readonlyDict, clonedReadonlyDict);

            var readonlyObj = new
            {
                Dict = readonlyDict
            };
            var clonedReadonlyObj = ObjectCloner.Clone(readonlyObj);
            clonedReadonlyObj.IsNotSameReferenceAs(readonlyObj);
            ValidateDictionary(readonlyObj.Dict, clonedReadonlyObj.Dict);
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
                if (originalValue != null && !TypeUtils.IsUnmanagedType(originalValue.GetType()))
                    cloned[clonedKey].IsNotSameReferenceAs(originalValue);
                if (originalValue?.GetType().IsArray == true)
                {
                    var array = (Array)(object)originalValue;
#if NET8_0_OR_GREATER
                    var arrayCloned = (Array)(object)cloned[clonedKey]!;
#else
                    var arrayCloned = (Array)(object)cloned[clonedKey];
#endif
                    for (int i = 0; i < array.Length; i++)
                    {
                        var orgValue = array.GetValue(i);
                        var clonedValue = arrayCloned.GetValue(i);
                        if (orgValue != null && !TypeUtils.IsUnmanagedType(orgValue.GetType()))
                            orgValue.IsNotSameReferenceAs(clonedValue);

                        orgValue.IsStructuralEqual(clonedValue);
                    }
                }
                else
                {
                    cloned[clonedKey].IsStructuralEqual(originalValue);
                }
            }

            cloned.IsNotSameReferenceAs(original);
        }


        private void ValidateObjectKeyDictionary<T>(T original, T cloned)
            where T : IDictionary<TestKey, TestValue>
        {
            int index = 0;
            foreach (var clonedKey in cloned.Keys)
            {
                var originalKey = original.Keys.FirstOrDefault(
                    key => key.Id == clonedKey.Id && key.Value == clonedKey.Value);
                clonedKey.IsNotSameReferenceAs(originalKey);
                clonedKey.IsStructuralEqual(originalKey);

                var originalValue = original[originalKey];
                cloned[clonedKey].IsNotSameReferenceAs(originalValue);
                cloned[clonedKey].IsStructuralEqual(originalValue);
            }
        }

        private void ValidateDictionary<TKey, TValue>(IDictionary<TKey, TValue> original, IDictionary<TKey, TValue> cloned)
#if NET8_0_OR_GREATER

    where TKey : notnull
#endif
        {
            int index = 0;
            foreach (var clonedKey in cloned.Keys)
            {
                var originalKey = original.Keys.Skip(index).First();
                if (!originalKey.GetType().IsValueType && originalKey.GetType() != typeof(string))
                    clonedKey.IsNotSameReferenceAs(originalKey);
                clonedKey.IsStructuralEqual(originalKey);

                var originalValue = original.Values.Skip(index++).First();
                var valueType = originalValue?.GetType();
                if (valueType != null && !TypeUtils.IsUnmanagedType(valueType))
                    cloned[clonedKey].IsNotSameReferenceAs(originalValue);
                if (originalValue != null && valueType?.IsArray == true)
                {
                    var array = (Array)(object)originalValue;
#if NET8_0_OR_GREATER
                    var arrayCloned = (Array)(object)cloned[clonedKey]!;
#else
                    var arrayCloned = (Array)(object)cloned[clonedKey];
#endif
                    for (int i = 0; i < array.Length; i++)
                    {
                        var orgValue = array.GetValue(i);
                        var clonedValue = arrayCloned.GetValue(i);
                        if (orgValue != null && !TypeUtils.IsUnmanagedType(orgValue.GetType()))
                            orgValue.IsNotSameReferenceAs(clonedValue);
                        orgValue.IsStructuralEqual(clonedValue);
                    }
                }
                else
                {
                    cloned[clonedKey].IsStructuralEqual(originalValue);
                }
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
