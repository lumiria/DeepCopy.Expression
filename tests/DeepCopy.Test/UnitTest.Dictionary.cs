using System;
using System.Collections.Concurrent;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                [new TestKey() { Id = 1 }] = new TestValue() { Value = "Foo" },
                [new TestKey() { Id = 2 }] = new TestValue() { Value = "Bar" },
                [new TestKey() { Id = 3 }] = new TestValue() { Value = "Baz" },
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
                [new TestKey[] { new TestKey() }] = new TestValue[] { new TestValue() },
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
            var dict = new ReadOnlyDictionary<int, string>(
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

        [Fact]
        public void SortedDictionaryTest()
        {
            var dict = new SortedDictionary<int, SubSample>
            {
                [0] = new SubSample(0, $"Name0"),
                [5] = new SubSample(5, $"Name5"),
                [2] = new SubSample(2, $"Name2"),
            };
            TestDictionaryCore<SortedDictionary<int, SubSample>, int, SubSample>(dict);
        }

        [Fact]
        public void ReferenceTypeSortedDictionaryTest()
        {
            var dict = new SortedDictionary<SortableKey, TestValue>()
            {
                [new SortableKey() { Id = 1 }] = new TestValue() { Value = "Foo" },
                [new SortableKey() { Id = 2 }] = new TestValue() { Value = "Bar" },
                [new SortableKey() { Id = 3 }] = new TestValue() { Value = "Baz" },
            };
            var keys = dict.Keys; // access ...

            TestDictionaryCore<SortedDictionary<SortableKey, TestValue>, SortableKey, TestValue>(dict);
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(0, Guid.NewGuid()),
                Pair(1, Guid.NewGuid()),
                Pair(2, Guid.NewGuid()),
            });
        }

        [Fact]
        public void XDictionaryNullableValueSemanticsKeyAndNullableValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(0, (Guid?)Guid.NewGuid()),
                Pair(1, (Guid?)null),
                Pair(2, (Guid?)Guid.NewGuid()),
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(Guid.NewGuid(), new Uri("https://example.com/value-01")),
                Pair(Guid.NewGuid(), new Uri("https://example.com/value-02")),
                Pair(Guid.NewGuid(), new Uri("https://example.com/value-03")),
            });
        }

        [Fact]
        public void XDictionaryNullableValueSemanticsKeyTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<Guid, Uri?>(Guid.NewGuid(), new Uri("https://example.com/value-01")),
                Pair<Guid, Uri?>(Guid.NewGuid(), null),
                Pair<Guid, Uri?>(Guid.NewGuid(), new Uri("https://example.com/value-03")),
#else
                Pair<Guid, Uri>(Guid.NewGuid(), new Uri("https://example.com/value-01")),
                Pair<Guid, Uri>(Guid.NewGuid(), null),
                Pair<Guid, Uri>(Guid.NewGuid(), new Uri("https://example.com/value-03")),
#endif
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndDividedValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair<int, BaseSample>(2, new SubSample(2, "name-2")),
                Pair<int, BaseSample>(1, new SubSample(1, "name-1")),
                Pair<int, BaseSample>(3, new SubSample(3, "name-3")),
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndNullableDividedValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<int, BaseSample?>(2, new SubSample(2, "name-2")),
                Pair<int, BaseSample?>(1, null),
                Pair<int, BaseSample?>(3, new SubSample(3, "name-3")),
#else
                Pair<int, BaseSample>(2, new SubSample(2, "name-2")),
                Pair<int, BaseSample>(1, null),
                Pair<int, BaseSample>(3, new SubSample(3, "name-3")),
#endif
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndObjectValueTest()
        {
            var date = DateTime.Now;
            TestDictionaryCore(new[]
            {
                Pair<DateTime, object>(date, 2),
                Pair<DateTime, object>(date.AddMinutes(-1), new Uri("https://example.com/value-01")),
                Pair<DateTime, object>(date.AddMinutes(2), Guid.NewGuid()),
                Pair<DateTime, object>(date.AddMinutes(3), new object())
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndNullableObjectValueTest()
        {
            var date = DateTime.Now;
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<DateTime, object?>(date, 2),
                Pair<DateTime, object?>(date.AddMinutes(-1), new Uri("https://example.com/value-01")),
                Pair<DateTime, object?>(date.AddMinutes(2), Guid.NewGuid()),
                Pair<DateTime, object?>(date.AddMinutes(3), null),
                Pair<DateTime, object?>(date.AddMinutes(4), new object())
#else
                Pair<DateTime, object>(date, 2),
                Pair<DateTime, object>(date.AddMinutes(-1), new Uri("https://example.com/value-01")),
                Pair<DateTime, object>(date.AddMinutes(2), Guid.NewGuid()),
                Pair<DateTime, object>(date.AddMinutes(3), null),
                Pair<DateTime, object>(date.AddMinutes(4), new object())
#endif
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(1, new[] { "one", "two" }),
                Pair(0, new[] { "three", "four" }),
                Pair(2, new[] { "five", "six" }),
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndNullableArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<int, string[]?>(1, new[] { "one", "two" }),
                Pair<int, string[]?>(0, null),
                Pair<int, string[]?>(2, new[] { "five", "six" }),
#else
                Pair<int, string[]>(1, new[] { "one", "two" }),
                Pair<int, string[]>(0, null),
                Pair<int, string[]>(2, new[] { "five", "six" }),
#endif
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndArrayRefValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(1, new [] { new Uri("https://example.com/value-01"), new Uri("https://example.com/value-02")}),
                Pair(0, new [] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04")}),
                Pair(2, new [] { new Uri("https://example.com/value-05"), new Uri("https://example.com/value-06")}),
            });
        }


        [Fact]
        public void XDictionaryValueSemanticsKeyAndNullableArrayRefValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<int, Uri[]?>(1, new [] { new Uri("https://example.com/value-01"), new Uri("https://example.com/value-02")}),
                Pair<int, Uri[]?>(0, null),
                Pair<int, Uri[]?>(2, Array.Empty<Uri>()),
#else
                Pair<int, Uri[]>(1, new [] { new Uri("https://example.com/value-01"), new Uri("https://example.com/value-02")}),
                Pair<int, Uri[]>(0, null),
                Pair<int, Uri[]>(2, Array.Empty<Uri>()),
#endif
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndObjectArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(1, new object[] { 0, 1 }),
                Pair(0, new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }),
                Pair(2, new object[] { "5", new Uri("https://example.com/value-06") }),
                Pair(3, new object[0]),
            });
        }

        [Fact]
        public void XDictionaryValueSemanticsKeyAndNullableObjectArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<int, object[]?>(1, new object[] { 0, 1 }),
                Pair<int, object[]?>(0, null),
                Pair<int, object[]?>(2, new object[] { "5", new Uri("https://example.com/value-06") }),
                Pair<int, object[]?>(3, Array.Empty<object>()),
#else
                Pair<int, object[]>(1, new object[] { 0, 1 }),
                Pair<int, object[]>(0, null),
                Pair<int, object[]>(2, new object[] { "5", new Uri("https://example.com/value-06") }),
                Pair<int, object[]>(3, Array.Empty<object>()),
#endif
            });
        }

        [Fact]
        public void XDictionaryReferenceKeyAndValueSemanticsValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(new Uri("https://example.com/value-01"), Guid.NewGuid()),
                Pair(new Uri("https://example.com/value-02"), Guid.NewGuid()),
                Pair(new Uri("https://example.com/value-03"), Guid.NewGuid()),
            },
            comparer: UriKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryDividedKeyAndValueSemanticsValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair<BaseSample, int>(new SubSample(2, "name-2"), 2),
                Pair<BaseSample, int>(new SubSample(1, "name-1"), 1),
                Pair<BaseSample, int>(new SubSample(3, "name-3"), 3),
            },
            comparer: BaseSampleKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndValueSemanticsValueTest()
        {
            var date = DateTime.Now;

            TestDictionaryCore(new[]
            {
                Pair<object, DateTime>(1, date),
                Pair<object, DateTime>(new Uri("https://example.com/value-02"), date.AddMinutes(-1)),
                Pair<object, DateTime>(Guid.NewGuid(), date.AddMinutes(2)),
                Pair<object, DateTime>(new object(), date.AddMinutes(3)),
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryArrayKeyAndValueSemanticsValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(new string[] { "one", "two" }, 1),
                Pair(new string[] { "five", "six" }, 3),
                Pair(new string[] { "three", "four" }, 2),
            },
            comparer: new ArrayKeyComparer<string>(StringComparer.Ordinal));
        }

        [Fact]
        public void XDictionaryArrayRefKeyAndValueSemanticsValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(new Uri[] { new Uri("https://example.com/value-01"), new Uri("https://example.com/value-02") }, 1),
                Pair(new Uri[] { new Uri("https://example.com/value-05"), new Uri("https://example.com/value-06") }, 3),
                Pair(new Uri[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, 2),
            },
            comparer: new ArrayKeyComparer<Uri>(UriKeyComparer.Instance));
        }

        [Fact]
        public void XDictionaryObjectArrayKeyAndValueSemanticsValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(new object[] { 0, 1 }, new object[] { 2, 0 }),
                Pair(new object[] { "5", new Uri("https://example.com/value-06") }, new object[] { "6", new Uri("https://example.com/value-05") }),
                Pair(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, new object[] { new Uri("https://example.com/value-04"), new Uri("https://example.com/value-02") }),
                Pair(new object[0], new object[0]),
            },
            comparer: new ArrayKeyComparer<object>(ObjectKeyComparer.Instance));
        }

        [Fact]
        public void XDictionaryObjectKeyAndDividedValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair<object, BaseSample>(1, new SubSample(1, "name-1")),
                Pair<object, BaseSample>(new Uri("https://example.com/value-02"), new SubSample(2, "name-2")),
                Pair<object, BaseSample>(new object(), new SubSample(4, "name-4")),
                Pair<object, BaseSample>(Guid.NewGuid(), new SubSample(3, "name-3")),
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndNullableDividedValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<object, BaseSample?>(1, new SubSample(1, "name-1")),
                Pair<object, BaseSample?>(new Uri("https://example.com/value-02"), new SubSample(2, "name-2")),
                Pair<object, BaseSample?>(new object(), new SubSample(4, "name-4")),
                Pair<object, BaseSample?>(Guid.NewGuid(), new SubSample(3, "name-3")),
                Pair<object, BaseSample?>("a", null),
#else
                Pair<object, BaseSample>(1, new SubSample(1, "name-1")),
                Pair<object, BaseSample>(new Uri("https://example.com/value-02"), new SubSample(2, "name-2")),
                Pair<object, BaseSample>(new object(), new SubSample(4, "name-4")),
                Pair<object, BaseSample>(Guid.NewGuid(), new SubSample(3, "name-3")),
                Pair<object, BaseSample>("a", null),
#endif
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndObjectValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair<object, object>(1, new object()),
                Pair<object, object>(new Uri("https://example.com/value-02"), Guid.NewGuid()),
                Pair<object, object>(new object(), 1),
                Pair<object, object>(Guid.NewGuid(), new Uri("https://example.com/value-02")),
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndNullableObjectValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<object, object?>(1, new object()),
                Pair<object, object?>(new Uri("https://example.com/value-02"), Guid.NewGuid()),
                Pair<object, object?>(new object(), 1),
                Pair<object, object?>(Guid.NewGuid(), new Uri("https://example.com/value-02")),
#else
                Pair<object, object>(1, new object()),
                Pair<object, object>(new Uri("https://example.com/value-02"), Guid.NewGuid()),
                Pair<object, object>(new object(), 1),
                Pair<object, object>(Guid.NewGuid(), new Uri("https://example.com/value-02")),
#endif
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndObjectArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair<object, object[]>(1, new object[] { 0, 1 }),
                Pair<object, object[]>(new Uri("https://example.com/value-02"), new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }),
                Pair<object, object[]>(new object(), new object[] { "5", new Uri("https://example.com/value-06") }),
                Pair<object, object[]>(Guid.NewGuid(), new object[0]),
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndNullableObjectArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<object, object?[]>(1, new object?[] { 0, 1 }),
                Pair<object, object?[]>(new Uri("https://example.com/value-02"), new object?[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }),
                Pair<object, object?[]>(new object(), new object?[] { "5", new Uri("https://example.com/value-06") }),
                Pair<object, object?[]>(Guid.NewGuid(), new object?[0]),
#else
                Pair<object, object[]>(1, new object[] { 0, 1 }),
                Pair<object, object[]>(new Uri("https://example.com/value-02"), new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }),
                Pair<object, object[]>(new object(), new object[] { "5", new Uri("https://example.com/value-06") }),
                Pair<object, object[]>(Guid.NewGuid(), new object[0]),
#endif
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndDividedArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair<object, BaseSample[]>(1, new SubSample[] { new SubSample(1, "name-1"), new SubSample(2, "name-2") }),
                Pair<object, BaseSample[]>(new Uri("https://example.com/value-02"), new SubSample[] { new SubSample(3, "name-3"), new SubSample(4, "name-4") }),
                Pair<object, BaseSample[]>(new object(), new SubSample[] { new SubSample(7, "name-7"), new SubSample(8, "name-8") }),
                Pair<object, BaseSample[]>(Guid.NewGuid(), new SubSample[] { new SubSample(5, "name-5"), new SubSample(6, "name-6") }),
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectKeyAndNullableDividedArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<object, BaseSample?[]>(1, new BaseSample?[] { new SubSample(1, "name-1"), new SubSample(2, "name-2") }),
                Pair<object, BaseSample?[]>(new Uri("https://example.com/value-02"), new BaseSample?[] { new SubSample(3, "name-3"), new SubSample(4, "name-4") }),
                Pair<object, BaseSample?[]>(new object(), new BaseSample?[] { new SubSample(7, "name-7"), new SubSample(8, "name-8") }),
                Pair<object, BaseSample?[]>(Guid.NewGuid(), new BaseSample?[] { new SubSample(5, "name-5"), new SubSample(6, "name-6") }),
#else
                Pair<object, BaseSample[]>(1, new BaseSample[] { new SubSample(1, "name-1"), new SubSample(2, "name-2") }),
                Pair<object, BaseSample[]>(new Uri("https://example.com/value-02"), new BaseSample[] { new SubSample(3, "name-3"), new SubSample(4, "name-4") }),
                Pair<object, BaseSample[]>(new object(), new BaseSample[] { new SubSample(7, "name-7"), new SubSample(8, "name-8") }),
                Pair<object, BaseSample[]>(Guid.NewGuid(), new BaseSample[] { new SubSample(5, "name-5"), new SubSample(6, "name-6") }),
#endif
            },
            comparer: ObjectKeyComparer.Instance);
        }

        [Fact]
        public void XDictionaryObjectArrayKeyAndObjectValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair<object[], object>(new object[] { 0, 1 }, new object()),
                Pair<object[], object>(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, Guid.NewGuid()),
                Pair<object[], object>(new object[] { "5", new Uri("https://example.com/value-06") }, 1),
                Pair<object[], object>(new object[0], new Uri("https://example.com/value-02")),
            },
            comparer: new ArrayKeyComparer<object>(ObjectKeyComparer.Instance));
        }

        [Fact]
        public void XDictionaryObjectArrayKeyAndObjectArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(new object[] { 0, 1 }, new object[] { 2, 0 }),
                Pair(new object[] { "5", new Uri("https://example.com/value-06") }, new object[] { "6", new Uri("https://example.com/value-05") }),
                Pair(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, new object[] { new Uri("https://example.com/value-04"), new Uri("https://example.com/value-02") }),
                Pair(new object[0], new object[0]),
            },
            comparer: new ArrayKeyComparer<object>(ObjectKeyComparer.Instance));
        }

        [Fact]
        public void XDictionaryObjectArrayKeyAndNullableArrayValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<object[], object[]?>(new object[] { 0, 1 }, new object[] { 2, 0 }),
                Pair<object[], object[]?>(new object[] { "5", new Uri("https://example.com/value-06") }, new object[] { "6", new Uri("https://example.com/value-05") }),
                Pair<object[], object[]?>(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, new object[] { new Uri("https://example.com/value-04"), new Uri("https://example.com/value-02") }),
                Pair<object[], object[]?>(new object[] { new Uri("https://example.com/value-07"), new Uri("https://example.com/value-08") }, null),
                Pair<object[], object[]?>(new object[0], new object[0]),
#else
                Pair<object[], object[]>(new object[] { 0, 1 }, new object[] { 2, 0 }),
                Pair<object[], object[]>(new object[] { "5", new Uri("https://example.com/value-06") }, new object[] { "6", new Uri("https://example.com/value-05") }),
                Pair<object[], object[]>(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, new object[] { new Uri("https://example.com/value-04"), new Uri("https://example.com/value-02") }),
                Pair<object[], object[]>(new object[] { new Uri("https://example.com/value-07"), new Uri("https://example.com/value-08") }, null),
                Pair<object[], object[]>(new object[0], new object[0]),
#endif
            },
            comparer: new ArrayKeyComparer<object>(ObjectKeyComparer.Instance));
        }

        [Fact]
        public void XDictionaryObjectArrayKeyAndNullableObjectValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<object[], object?>(new object[] { 0, 1 }, new object()),
                Pair<object[], object?>(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, Guid.NewGuid()),
                Pair<object[], object?>(new object[] { "5", new Uri("https://example.com/value-06") }, 1),
                Pair<object[], object?>(new object[0], new Uri("https://example.com/value-02")),
#else
                Pair<object[], object>(new object[] { 0, 1 }, new object()),
                Pair<object[], object>(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, Guid.NewGuid()),
                Pair<object[], object>(new object[] { "5", new Uri("https://example.com/value-06") }, 1),
                Pair<object[], object>(new object[0], new Uri("https://example.com/value-02")),
#endif
            },
            comparer: new ArrayKeyComparer<object>(ObjectKeyComparer.Instance));
        }

        [Fact]
        public void XDictionaryObjectArrayKeyAndDividedValueTest()
        {
            TestDictionaryCore(new[]
            {
                Pair(new object[] { 0, 1 }, new SubSample[] { new SubSample(1, "name-1"), new SubSample(2, "name-2") }),
                Pair(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, new SubSample[] { new SubSample(3, "name-3"), new SubSample(4, "name-4") }),
                Pair(new object[] { "5", new Uri("https://example.com/value-06") }, new SubSample[] { new SubSample(5, "name-5"), new SubSample(6, "name-6") }),
                Pair(new object[0], new SubSample[] { new SubSample(7, "name-7"), new SubSample(8, "name-8") }),
            },
            comparer: new ArrayKeyComparer<object>(ObjectKeyComparer.Instance));
        }

        [Fact]
        public void XDictionaryObjectArrayKeyAndNullableDividedValueTest()
        {
            TestDictionaryCore(new[]
            {
#if NET8_0_OR_GREATER
                Pair<object[], BaseSample?>(new object[] { 0, 1 },  new SubSample(1, "name-1")),
                Pair<object[], BaseSample?>(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, new SubSample(3, "name-3")),
                Pair<object[], BaseSample?>(new object[] { "5", new Uri("https://example.com/value-06") },  new SubSample(5, "name-5")),
                Pair<object[], BaseSample?>(Array.Empty<object>(), new SubSample(4, "name-4")),
                Pair<object[], BaseSample?>(new object[] { 2, 3 }, null),
#else
                Pair<object[], BaseSample>(new object[] { 0, 1 },  new SubSample(1, "name-1")),
                Pair<object[], BaseSample>(new object[] { new Uri("https://example.com/value-03"), new Uri("https://example.com/value-04") }, new SubSample(3, "name-3")),
                Pair<object[], BaseSample>(new object[] { "5", new Uri("https://example.com/value-06") },  new SubSample(5, "name-5")),
                Pair<object[], BaseSample>(Array.Empty<object>(), new SubSample(4, "name-4")),
                Pair<object[], BaseSample>(new object[] { 2, 3 }, null),
#endif
            },
            comparer: new ArrayKeyComparer<object>(ObjectKeyComparer.Instance));
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

            where TKey : notnull
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
            //int index = 0;
            foreach (var clonedKey in cloned.Keys)
            {
                var originalKey = original.Keys.First(x => StructuralEquals(x, clonedKey));
                //var originalKey = original.Keys.Skip(index).First();
                if (!originalKey.GetType().IsValueType && originalKey.GetType() != typeof(string))
                    clonedKey.IsNotSameReferenceAs(originalKey);
                clonedKey.IsStructuralEqual(originalKey);

                var originalValue = original[originalKey];
                //var originalValue = original.Values.Skip(index++).First();
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

        //        private void TestDictionaryBase<TDictionary, TKey, TValue>(TDictionary dict)
        //            where TDictionary : IDictionary<TKey, TValue>
        //#if NET8_0_OR_GREATER
        //            where TKey : notnull
        //#endif
        //        {
        //            var clonedDict = ObjectCloner.Clone(dict);
        //            ValidateDictionaryBase<TDictionary, TKey, TValue>(dict, clonedDict);

        //            var obj = new
        //            {
        //                Dict = dict,
        //            };
        //            var clonedObj = ObjectCloner.Clone(obj);
        //            clonedObj.IsNotSameReferenceAs(obj);
        //            ValidateDictionaryBase<TDictionary, TKey, TValue>(obj.Dict, clonedObj.Dict);

        //            var readonlyDict = new ReadOnlyDictionary<TKey, TValue>(dict);
        //            var clonedReadonlyDict = ObjectCloner.Clone(readonlyDict);
        //            ValidateDictionaryBase<ReadOnlyDictionary<TKey, TValue>, TKey, TValue>(readonlyDict, clonedReadonlyDict);

        //            var readonlyObj = new
        //            {
        //                Dict = readonlyDict
        //            };
        //            var clonedReadonlyObj = ObjectCloner.Clone(readonlyObj);
        //            clonedReadonlyObj.IsNotSameReferenceAs(readonlyObj);
        //            ValidateDictionaryBase<ReadOnlyDictionary<TKey, TValue>, TKey, TValue>(readonlyObj.Dict, clonedReadonlyObj.Dict);
        //        }



        private void TestDictionaryCore<TKey, TValue>(
            IEnumerable<KeyValuePair<TKey, TValue>> items,
#if NET8_0_OR_GREATER
            IComparer<TKey>? comparer = null,
            IEqualityComparer<TKey>? equalityComparer = null)
            where TKey : notnull
#else
            IComparer<TKey> comparer = null,
            IEqualityComparer<TKey> equalityComparer = null)
#endif
        {
            var materializedItems = items.ToArray();

            var sortedDict = comparer == null
                ? new SortedDictionary<TKey, TValue>()
                : new SortedDictionary<TKey, TValue>(comparer);

            var concurrentDict = equalityComparer == null
                ? new ConcurrentDictionary<TKey, TValue>()
                : new ConcurrentDictionary<TKey, TValue>(equalityComparer);

#if NET8_0_OR_GREATER
            var orderedDict = equalityComparer == null
                ? new OrderedDictionary<TKey, TValue>()
                : new OrderedDictionary<TKey, TValue>(equalityComparer);

            var immutableBuilder = equalityComparer == null
                ? ImmutableDictionary.CreateBuilder<TKey, TValue>()
                : ImmutableDictionary.CreateBuilder<TKey, TValue>(equalityComparer);
#endif

            foreach (var item in materializedItems)
            {
                sortedDict.Add(item.Key, item.Value);
                concurrentDict.TryAdd(item.Key, item.Value);
#if NET8_0_OR_GREATER
                orderedDict.Add(item.Key, item.Value);
                immutableBuilder[item.Key] = item.Value;
#endif
            }

#if NET8_0_OR_GREATER
            var immutableDict = immutableBuilder.ToImmutable();

            var frozenDict = equalityComparer == null
                ? materializedItems.ToFrozenDictionary()
                : materializedItems.ToFrozenDictionary(equalityComparer);
#endif

            TestDictionaryCore<SortedDictionary<TKey, TValue>, TKey, TValue>(sortedDict);
            TestDictionaryCore<ConcurrentDictionary<TKey, TValue>, TKey, TValue>(concurrentDict);
#if NET8_0_OR_GREATER
            TestDictionaryCore<OrderedDictionary<TKey, TValue>, TKey, TValue>(orderedDict);
            TestDictionaryCore<ImmutableDictionary<TKey, TValue>, TKey, TValue>(immutableDict);
            TestDictionaryCore<FrozenDictionary<TKey, TValue>, TKey, TValue>(frozenDict);
#endif
        }

        private void TestDictionaryCore<TDictionary, TKey, TValue>(TDictionary dict)
            where TDictionary : IDictionary<TKey, TValue>
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

        //        private void ValidateDictionary<TDictionary, TKey, TValue>(TDictionary original, TDictionary cloned)
        //    where TDictionary : IDictionary<TKey, TValue>
        //#if NET8_0_OR_GREATER
        //    where TKey : notnull
        //#endif
        //        {
        //            cloned.StructuralEquals(original);

        //            int index = 0;
        //            foreach (var clonedKey in cloned.Keys)
        //            {
        //                TKey originalKey = original.Keys.Skip(index).First();
        //                if (!originalKey.GetType().IsValueType && originalKey.GetType() != typeof(string))
        //                    clonedKey.IsNotSameReferenceAs(originalKey);
        //                clonedKey.IsStructuralEqual(originalKey);

        //                var originalValue = original.Values.Skip(index++).First();
        //                if (originalValue != null && !TypeUtils.IsUnmanagedType(originalValue.GetType()))
        //                    cloned[clonedKey].IsNotSameReferenceAs(originalValue);
        //                if (originalValue?.GetType().IsArray == true)
        //                {
        //                    var array = (Array)(object)originalValue;
        //#if NET8_0_OR_GREATER
        //                    var arrayCloned = (Array)(object)cloned[clonedKey]!;
        //#else
        //                    var arrayCloned = (Array)(object)cloned[clonedKey];
        //#endif
        //                    for (int i = 0; i < array.Length; i++)
        //                    {
        //                        var orgValue = array.GetValue(i);
        //                        var clonedValue = arrayCloned.GetValue(i);
        //                        if (orgValue != null && !TypeUtils.IsUnmanagedType(orgValue.GetType()))
        //                            orgValue.IsNotSameReferenceAs(clonedValue);

        //                        orgValue.IsStructuralEqual(clonedValue);
        //                    }
        //                }
        //                else
        //                {
        //                    cloned[clonedKey].IsStructuralEqual(originalValue);
        //                }
        //            }

        //            cloned.IsNotSameReferenceAs(original);
        //        }

        private static bool StructuralEquals<T>(T actual, T expected)
        {
            if (actual == null)
                return expected == null;

            if (actual.GetType() != expected?.GetType())
                return false;

            var fields = GetFields(actual.GetType(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .ToArray();
            foreach (var field in fields)
            {
                var expectedValue = field.GetValue(expected);
                var actualValue = field.GetValue(actual);

                if (field.FieldType.IsPrimitive || field.FieldType.IsEnum || field.FieldType == typeof(decimal) || field.FieldType == typeof(string))
                {
                    if (!(expectedValue?.Equals(actualValue) ?? actualValue == null))
                    {
                        return false;
                    }
                }
                else if (!StructuralEquals(expectedValue, actualValue))
                {
                    return false;
                }
            }
            if (fields.Length > 0)
                return true;

            if (typeof(T).IsArray)
            {
                var expectedArray = expected as Array;
                var actualArray = actual as Array;
                if (expectedArray?.Length != actualArray?.Length)
                    return false;
                for (int i = 0; i < expectedArray?.Length; i++)
                {
                    if (!StructuralEquals(expectedArray.GetValue(i), actualArray?.GetValue(i)))
                        return false;
                }

                return true;
            }

            if (expected?.GetType() == typeof(object))
            {
                return actual?.GetType() == typeof(object);
            }

            return EqualityComparer<T>.Default.Equals(expected, actual);

            IEnumerable<FieldInfo> GetFields(Type type, BindingFlags bindingFlags)
            {
                var baseType = type.BaseType;
                while (baseType != null && !baseType.IsInterface)
                {
                    foreach (var t in GetFields(baseType, bindingFlags))
                        yield return t;

                    baseType = baseType.BaseType;
                }

                foreach (var t in type.GetFields(bindingFlags))
                    if (!IsEvent(type, t.Name)) yield return t;
            }

            bool IsEvent(Type type, string fieldName) =>
                type.GetEvent(fieldName) != null;
        }


        private static KeyValuePair<TKey, TValue> Pair<TKey, TValue>(TKey key, TValue value) =>
            new KeyValuePair<TKey, TValue>(key, value);

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

        internal sealed class SortableKey : IComparable
        {
            public int Id { get; set; }
#if NET8_0_OR_GREATER
            public string? Value { get; set; }

            public int CompareTo(object? obj)
#else
            public string Value { get; set; }

            public int CompareTo(object obj)
#endif
            {
                if (obj is SortableKey other)
                {
                    int idComparison = Id.CompareTo(other.Id);
                    if (idComparison != 0)
                        return idComparison;
                    return string.Compare(Value, other.Value, StringComparison.Ordinal);
                }
                return 0;
            }
        }

        private sealed class UriKeyComparer : IComparer<Uri>
        {
            public static UriKeyComparer Instance { get; } = new UriKeyComparer();

#if NET8_0_OR_GREATER
            public int Compare(Uri? x, Uri? y)
#else
            public int Compare(Uri x, Uri y)
#endif
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x is null) return -1;
                if (y is null) return 1;

                return StringComparer.Ordinal.Compare(x.AbsoluteUri, y.AbsoluteUri);
            }
        }

        private sealed class BaseSampleKeyComparer : IComparer<BaseSample>
        {
            public static BaseSampleKeyComparer Instance { get; } = new BaseSampleKeyComparer();

#if NET8_0_OR_GREATER
            public int Compare(BaseSample? x, BaseSample? y)
#else
            public int Compare(BaseSample x, BaseSample y)
#endif
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x is null) return -1;
                if (y is null) return 1;

                var xSub = x as SubSample;
                var ySub = y as SubSample;
                if (xSub != null && ySub != null)
                {
                    int idComparison = xSub.Id.CompareTo(ySub.Id);
                    if (idComparison != 0)
                        return idComparison;
                }

                var typeComparison = string.Compare(x.GetType().FullName, y.GetType().FullName,
                    StringComparison.Ordinal);
                if (typeComparison != 0)
                    return typeComparison;

                return StringComparer.Ordinal.Compare(x.ToString(), y.ToString());
            }
        }

        private sealed class ObjectKeyComparer : IComparer<object>
        {
            public static ObjectKeyComparer Instance { get; } = new ObjectKeyComparer();

#if NET8_0_OR_GREATER
            public int Compare(object? x, object? y)
#else
            public int Compare(object x, object y)
#endif
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x is null) return -1;
                if (y is null) return 1;

                var typeComparison = StringComparer.Ordinal.Compare(
                    x.GetType().FullName, y.GetType().FullName);
                if (typeComparison != 0)
                    return typeComparison;

                if (x is IComparable xComparable && y is IComparable yComparable)
                {
                    return xComparable.CompareTo(yComparable);
                }

                var textComparison = StringComparer.Ordinal.Compare(x.ToString(), y.ToString());
                if (textComparison != 0)
                    return textComparison;

                return RuntimeHelpers.GetHashCode(x).CompareTo(RuntimeHelpers.GetHashCode(y));
            }
        }

        private sealed class ArrayKeyComparer<T> : IComparer<T[]>
        {
            private readonly IComparer<T> _elementComparer;

            public ArrayKeyComparer(IComparer<T> elementComparer)
            {
                _elementComparer = elementComparer;
            }

#if NET8_0_OR_GREATER
            public int Compare(T[]? x, T[]? y)
#else
            public int Compare(T[] x, T[] y)
#endif
            {
                if (ReferenceEquals(x, y)) return 0;
                if (x is null) return -1;
                if (y is null) return 1;

                if (x.Length != y.Length) return x.Length.CompareTo(y.Length);

                for (int i = 0; i < x.Length; i++)
                {
                    int elementComparison = _elementComparer.Compare(x[i], y[i]);
                    if (elementComparison != 0)
                        return elementComparison;
                }

                return 0;
            }
        }
    }
}
