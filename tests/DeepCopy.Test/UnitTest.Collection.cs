using System;
using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DeepCopy.Test.DataTypes;
using DeepCopy.Test.Inners;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void ValueTypeListTest()
        {
            var list = new List<int> { 1, 2, 3 };
            TestCollection(list);
        }

        [Fact]
        public void NullableValueTypeListTest()
        {
            var list = new List<int?> { 1, null, 3 };
            TestCollection(list);
        }

        [Fact]
        public void StringListTest()
        {
            var list = new List<string> { "A", "B", "C" };
            TestCollection(list);
        }

        [Fact]
        public void NullableStringListTest()
        {
#if NET8_0_OR_GREATER
            var list = new List<string?> { "A", null, "C" };
#else
            var list = new List<string> { "A", null, "C" };
#endif
            TestCollection(list);
        }

        [Fact]
        public void EnumTypeListTest()
        {
            var list = new List<EnumData> { EnumData.C, EnumData.A, EnumData.B };
            TestCollection(list);
        }

        [Fact]
        public void NullableEnumTypeListTest()
        {
            var list = new List<EnumData?> { EnumData.C, null, EnumData.B };
            TestCollection(list);
        }

        [Fact]
        public void TupleListTest()
        {
            var list = new List<(int, string, EnumData)>
            {
                (1, "A", EnumData.C),
                (2, "B", EnumData.A),
                (3, "C", EnumData.B)
            };

            TestCollection(list);
        }

        [Fact]
        public void NullableTupleListTest()
        {
            var list = new List<(int, string, EnumData)?>
            {
                (1, "A", EnumData.C),
                null,
                (3, "C", EnumData.B)
            };

            TestCollection(list);
        }

        [Fact]
        public void StructListTest()
        {
            var list = new List<StructData>
            {
                new StructData { Id = 1, Name = "A" },
                new StructData { Id = 2, Name = "B" },
                new StructData { Id = 3, Name = "C" }
            };

            TestCollection(list);
        }

        [Fact]
        public void NullableStructListTest()
        {
            var list = new List<StructData?>
            {
                new StructData { Id = 1, Name = "A" },
                null,
                new StructData { Id = 3, Name = "C" }
            };

            TestCollection(list);
        }

        [Fact]
        public void ReferenceTypeListTest()
        {
#if NET8_0_OR_GREATER
            var list = new List<Node?>
#else
            var list = new List<Node>
#endif
            {
                new Node { Value = 1 },
                null,
                new Node { Value = 2 },
            };

            TestCollection(list);
        }

        [Fact]
        public void BaseTypeListTest()
        {
#if NET8_0_OR_GREATER
            var list = new List<BaseSample?>
#else
            var list = new List<BaseSample>
#endif
            {
                new SubSample(1, "A"),
                null,
                new SubSample(2, "B"),
            };
            TestCollection(list);
        }

        [Fact]
        public void NestedListTest()
        {
            var list = new List<List<int>>
            {
                new List<int> { 1, 2, 3 },
                new List<int> { 4, 5, 6 },
                new List<int> { 7, 8, 9 }
            };

            TestCollection(list);
        }

        [Fact]
        public void LargeListTest()
        {
            var list = Enumerable.Range(1, 10000).ToList();
            TestCollection(list);
        }

        [Fact]
        public void EmptyListTest()
        {
            var list = new List<int>();
            TestCollection(list);
        }

        [Fact]
        public void ArrayListTest()
        {
            var list = new ArrayList { 1, "A", EnumData.B, new Node { Value = 1 } };
            TestCollection(list);
        }

        [Fact]
        public void StringQueueTest()
        {
            var queue = new Queue<string>();
            queue.Enqueue("A");
            queue.Enqueue("B");
            queue.Enqueue("C");

            TestCollection(queue);

            var cloned = ObjectCloner.Clone(queue);
            Assert.Equal("A", cloned.Dequeue());
        }

        [Fact]
        public void RefrenceTypeQueueTest()
        {
            var queue = new Queue<Node>();
            queue.Enqueue(new Node { Value = 1 });
            queue.Enqueue(new Node { Value = 2 });

            TestCollection(queue);

            var cloned = ObjectCloner.Clone(queue);
            Assert.Equal(1, cloned.Dequeue().Value);
        }

        [Fact]
        public void NullableValueTypeStackTest()
        {
            var stack = new Stack<int?>();
            stack.Push(1);
            stack.Push(null);
            stack.Push(3);

            TestCollection(stack);

            var cloned = ObjectCloner.Clone(stack);
            Assert.Equal(3, cloned.Pop());
        }

        [Fact]
        public void ReferenceTypeStackTest()
        {
            var stack = new Stack<Node>();
            stack.Push(new Node { Value = 1 });
            stack.Push(new Node { Value = 2 });

            TestCollection(stack);

            var cloned = ObjectCloner.Clone(stack);
            Assert.Equal(2, cloned.Pop().Value);
        }

        [Fact]
        public void StructTypeReadOnlyCollectionTest()
        {
            var collection = new ReadOnlyCollection<StructData>(new[]
            {
                new StructData { Value = 1 },
                new StructData { Value = 2 },
            });

            TestCollection(collection);
        }

        [Fact]
        public void ReferenceTypeReadOnlyCollectionTest()
        {
            var collection = new ReadOnlyCollection<Node>(new[]
            {
                new Node { Value = 1 },
                new Node { Value = 2 },
            });

            TestCollection(collection);
        }


        [Fact]
        public void EnumReadOnlyListTest()
        {
            IReadOnlyList<EnumData> list = new List<EnumData>() { EnumData.A, EnumData.B };

            TestCollection(list);
        }

        [Fact]
        public void ReferenceTypeReadOnlyListTest()
        {
            IReadOnlyList<Node> list = new List<Node>() { new Node { Value = 1 }, new Node { Value = 2 } };
            TestCollection(list);
        }

        [Fact]
        public void LinkedListTest()
        {
            var list = new LinkedList<Node>();
            for (int i = 1; i <= 5000; i++)
            {
                list.AddLast(new Node { Value = i });
            }

            TestCollection(list);
        }

#if NET8_0_OR_GREATER
        [Fact]
        public void NullableStructTypeImmutableListTest()
        {
            ImmutableList<TestStruct?> list = [new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" }];
            TestCollection(list, false);
        }

        [Fact]
        public void ReferenceTypeImmutableListTest()
        {
            ImmutableList<Node?> list = [new Node { Value = 1 }, null, new Node { Value = 2 }];
            TestCollection(list, false);
        }

        [Fact]
        public void NullableStructTypeImmutableArrayTest()
        {
            ImmutableArray<TestStruct?> array = [new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" }];
            TestCollection(array);

            ImmutableArray<TestStruct?> copied = [];
            ObjectCloner.CopyTo(array, ref copied);
            ValidateCollection(array, copied);
        }

        [Fact]
        public void ReferenceTypeImmutableArrayTest()
        {
            ImmutableArray<Node?> array = [new Node { Value = 1 }, null, new Node { Value = 2 }];
            TestCollection(array);

            ImmutableArray<Node?> copied = [];
            ObjectCloner.CopyTo(array, ref copied);
            ValidateCollection(array, copied);
        }

        [Fact]
        public void StructTypeImmutableHashSetTest()
        {
            ImmutableHashSet<TestStruct> array = [new TestStruct { Value = "A" }, new TestStruct { Value = "C" }];
            TestSet<ImmutableHashSet<TestStruct>, TestStruct>(array, false);
        }

        [Fact]
        public void NullableStructTypeImmutableHashSetTest()
        {
            ImmutableHashSet<TestStruct?> array = [new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" }];
            TestSet<ImmutableHashSet<TestStruct?>, TestStruct?>(array, false);
        }

        [Fact]
        public void ReferenceTypeImmutableHashSetTest()
        {
            ImmutableHashSet<Node?> array = [new Node { Value = 1 }, null, new Node { Value = 2 }];
            TestSet(array, false, new NodeEqualityComparer());
        }


        [Fact]
        public void ImmutableSortedSetTest()
        {
            ImmutableSortedSet<int> array = ImmutableSortedSet.Create(3, 1, 2);
            TestCollection(array, false, false);
        }

        [Fact]
        public void ReferenceTypeImmutableSortedSetTest()
        {
            ImmutableSortedSet<Node?> array = ImmutableSortedSet.Create(new NodeComparer(), new Node { Value = 1 }, null, new Node { Value = 2 });
            TestCollection(array, false, false);
        }


        [Fact]
        public void NullableStructTypeImmutableStackTest()
        {
            ImmutableStack<TestStruct?> array = [new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" }];
            TestCollection(array, false);
        }

        [Fact]
        public void ReferenceTypeImmutableStackTest()
        {
            ImmutableStack<Node?> array = [new Node { Value = 1 }, null, new Node { Value = 2 }];
            TestCollection(array, false);
        }

        [Fact]
        public void NullableStructTypeImmutableQueueTest()
        {
            ImmutableQueue<TestStruct?> array = [new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" }];
            TestCollection(array, false, false);
        }

        [Fact]
        public void ReferenceTypeImmutableQueueTest()
        {
            ImmutableQueue<Node?> array = [new Node { Value = 1 }, null, new Node { Value = 2 }];
            TestCollection(array, false, false);
        }
#endif

        [Fact]
        public void NullableStructIListTest()
        {
            IList list = new List<TestStruct?> { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } };
            TestCollection(list);
        }

        [Fact]
        public void ReferenceTypeIListTest()
        {
            IList list = new List<Node> { new Node { Value = 1 }, null, new Node { Value = 2 } };
            TestCollection(list);
        }

        [Fact]
        public void NullableStructIEnumerableTest()
        {
            IEnumerable list = new List<TestStruct?> { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } };
            TestCollection(list);
        }

        [Fact]
        public void RefenreceTypeIEnumerableTest()
        {
            IEnumerable list = new List<Node> { new Node { Value = 1 }, null, new Node { Value = 2 } };
            TestCollection(list);
        }

        [Fact]
        public void CollectionMemberTest()
        {
            var obj = new
            {
                ValueList = new List<int> { 1, 2, 3 },
                NullableValueList = new List<int?> { 1, null, 3 },
                StringList = new List<string> { "A", "B", "C" },
#if NET8_0_OR_GREATER
                NullableStringList = new List<string?> { "A", null, "C" },
#else
                NullableStringList = new List<string> { "A", null, "C" },
#endif
                EnumList = new List<EnumData> { EnumData.C, EnumData.A, EnumData.B },
                NullableEnumList = new List<EnumData?> { EnumData.C, null, EnumData.B },
                TupleList = new List<(int, string, EnumData)>
                {
                    (1, "A", EnumData.C),
                    (2, "B", EnumData.A),
                    (3, "C", EnumData.B)
                },
                NullableTupleList = new List<(int, string, EnumData)?>
                {
                    (1, "A", EnumData.C),
                    null,
                    (3, "C", EnumData.B)
                },
                StructList = new List<StructData>
                {
                    new StructData { Id = 1, Name = "A" },
                    new StructData { Id = 2, Name = "B" },
                    new StructData { Id = 3, Name = "C" }
                },
                NullableStructList = new List<StructData?>
                {
                    new StructData { Id = 1, Name = "A" },
                    null,
                    new StructData { Id = 3, Name = "C" }
                },
#if NET8_0_OR_GREATER
                ReferenceList = new List<Node?>
#else
                ReferenceList = new List<Node>
#endif
                {
                    new Node { Value = 1 },
                    null,
                    new Node { Value = 2 },
                },
#if NET8_0_OR_GREATER
                BaseTypeList = new List<BaseSample?>
#else
                BaseTypeList = new List<BaseSample>
#endif
                {
                    new SubSample(1, "A"),
                    null,
                    new SubSample(2, "B"),
                },
                NestedList = new List<List<int>>
                {
                    new List<int> { 1, 2, 3 },
                    new List<int> { 4, 5, 6 },
                    new List<int> { 7, 8, 9 }
                },
                LargeList = Enumerable.Range(1, 10000).ToList(),
                EmptyList = new List<int>(),
                ArrayList = new ArrayList { 1, "A", EnumData.B, new Node { Value = 1 } },
                StringQueue = new Queue<string>(new[] { "A", "B", "C" }),
                ReferenceQueue = new Queue<Node>(new[] { new Node { Value = 1 }, new Node { Value = 2 } }),
                NullableValueStack = new Stack<int?>(new int?[] { 1, null, 3 }),
                ReferenceStack = new Stack<Node>(new[] { new Node { Value = 1 }, new Node { Value = 2 } }),
                StructTypeReadOnlyCollection = new ReadOnlyCollection<StructData>(new[]
                    {
                    new StructData { Value = 1 },
                    new StructData { Value = 2 },
                }),
                ReferenceTypeReadOnlyCollection = new ReadOnlyCollection<Node>(new[]
                    {
                    new Node { Value = 1 },
                    new Node { Value = 2 },
                }),
                EnumReadOnlyList = new List<EnumData>() { EnumData.A, EnumData.B },
                ReferenceTypeReadOnlyList = new List<Node>() { new Node { Value = 1 }, new Node { Value = 2 } },
#if NET8_0_OR_GREATER
                NullableStructTypeImmutableList = ImmutableList.Create(new TestStruct?[] { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                ReferenceTypeImmutableList = ImmutableList.Create(new Node?[] { new Node { Value = 1 }, null, new Node { Value = 2 } }),
                NullableStructTypeImmutableArray = ImmutableArray.Create(new TestStruct?[] { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                ReferenceTypeImmutableArray = ImmutableArray.Create(new Node?[] { new Node { Value = 1 }, null, new Node { Value = 2 } }),
                NullableStructTypeImmutableHashSet = ImmutableHashSet.Create(new TestStruct?[] { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                ReferenceTypeImmutableHashSet = ImmutableHashSet.Create(new Node?[] { new Node { Value = 1 }, null, new Node { Value = 2 } }),
                NullableStructTypeImmutableStack = ImmutableStack.Create(new TestStruct?[] { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                ReferenceTypeImmutableStack = ImmutableStack.Create(new TestStruct?[] { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                NullableStructTypeImmutableQueue = ImmutableQueue.Create(new TestStruct?[] { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                ReferenceTypeImmutableQueue = ImmutableQueue.Create(new Node[] { new Node { Value = 1 },  new Node { Value = 2 } }),
#endif
                NullableStructTypeIList = (IList<TestStruct?>)(new List<TestStruct?> { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                ReferenceTypeIList = (IList<Node>)(new List<Node> { new Node { Value = 1 }, null, new Node { Value = 2 } }),
                NullableStructTypeIEnumerable = (IEnumerable<TestStruct?>)(new List<TestStruct?> { new TestStruct { Value = "A" }, null, new TestStruct { Value = "C" } }),
                ReferenceTypeIEnumerable = (IEnumerable<Node>)(new List<Node> { new Node { Value = 1 }, null, new Node { Value = 2 } }),
            };

            var cloned = ObjectCloner.Clone(obj);
            cloned.IsNotSameReferenceAs(obj);

            ValidateCollection(obj.ValueList, cloned.ValueList);
            ValidateCollection(obj.NullableValueList, cloned.NullableValueList);
            ValidateCollection(obj.StringList, cloned.StringList);
            ValidateCollection(obj.NullableStringList, cloned.NullableStringList);
            ValidateCollection(obj.EnumList, cloned.EnumList);
            ValidateCollection(obj.NullableEnumList, cloned.NullableEnumList);
            ValidateCollection(obj.TupleList, cloned.TupleList);
            ValidateCollection(obj.NullableTupleList, cloned.NullableTupleList);
            ValidateCollection(obj.StructList, cloned.StructList);
            ValidateCollection(obj.NullableStructList, cloned.NullableStructList);
            ValidateCollection(obj.ReferenceList, cloned.ReferenceList);
            ValidateCollection(obj.BaseTypeList, cloned.BaseTypeList);
            ValidateCollection(obj.NestedList, cloned.NestedList);
            ValidateCollection(obj.LargeList, cloned.LargeList);
            ValidateCollection(obj.EnumList, cloned.EnumList);
            ValidateCollection(obj.LargeList, cloned.LargeList);
            ValidateCollection(obj.EmptyList, cloned.EmptyList);
            ValidateCollection(obj.StringQueue, cloned.StringQueue);
            ValidateCollection(obj.ReferenceQueue, cloned.ReferenceQueue);
            ValidateCollection(obj.NullableValueStack, cloned.NullableValueStack);
            ValidateCollection(obj.ReferenceStack, cloned.ReferenceStack);
            ValidateCollection(obj.StructTypeReadOnlyCollection, cloned.StructTypeReadOnlyCollection);
            ValidateCollection(obj.ReferenceTypeReadOnlyCollection, cloned.ReferenceTypeReadOnlyCollection);
            ValidateCollection(obj.EnumReadOnlyList, cloned.EnumReadOnlyList);
            ValidateCollection(obj.ReferenceTypeReadOnlyList, cloned.ReferenceTypeReadOnlyList);
#if NET8_0_OR_GREATER
            ValidateCollection(obj.NullableStructTypeImmutableList, cloned.NullableStructTypeImmutableList);
            ValidateCollection(obj.ReferenceTypeImmutableList, cloned.ReferenceTypeImmutableList);
            ValidateCollection(obj.NullableStructTypeImmutableArray, cloned.NullableStructTypeImmutableArray);
            ValidateCollection(obj.ReferenceTypeImmutableArray, cloned.ReferenceTypeImmutableArray);
            ValidateSet(obj.NullableStructTypeImmutableHashSet, cloned.NullableStructTypeImmutableHashSet, null);
            ValidateSet(obj.ReferenceTypeImmutableHashSet, cloned.ReferenceTypeImmutableHashSet, new NodeEqualityComparer());
            ValidateCollection(obj.NullableStructTypeImmutableStack, cloned.NullableStructTypeImmutableStack, false);
            ValidateCollection(obj.ReferenceTypeImmutableStack, cloned.ReferenceTypeImmutableStack, false);
            ValidateCollection(obj.NullableStructTypeImmutableQueue, cloned.NullableStructTypeImmutableQueue, false);
            ValidateCollection(obj.ReferenceTypeImmutableQueue, cloned.ReferenceTypeImmutableQueue, false);
#endif
            ValidateCollection(obj.NullableStructTypeIList, cloned.NullableStructTypeIList);
            ValidateCollection(obj.ReferenceTypeIList, cloned.ReferenceTypeIList);
            ValidateCollection(obj.NullableStructTypeIEnumerable, cloned.NullableStructTypeIEnumerable);
            ValidateCollection(obj.ReferenceTypeIEnumerable, cloned.ReferenceTypeIEnumerable);
        }

        private void TestCollection<T>(T source, bool copyable = true, bool validateMembers = true)
            where T : IEnumerable
        {

            var cloned = ObjectCloner.Clone(source);
            ValidateCollection(source, cloned, validateMembers);

            cloned = ObjectCloner.Clone(source, true);
            ValidateCollection(source, cloned, validateMembers);

            if (!typeof(T).IsValueType)
            {
#if NET8_0_OR_GREATER
                var copied = (T)RuntimeHelpers.GetUninitializedObject(source.GetType());
#else
                var copied = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(source.GetType());
#endif

                if (copyable)
                {
                    ObjectCloner.CopyTo(source, copied);
                    ValidateCollection(source, copied, validateMembers);
                }
                else
                {
                    Assert.Throws<NotSupportedException>(() => ObjectCloner.CopyTo(source, copied));
                }
            }
        }

        private void ValidateCollection<T>(T source, T destination, bool validateMembers = true)
            where T : IEnumerable
        {
            destination.IsNotSameReferenceAs(source);
            destination.IsStructuralEqual(source);

            var type = source.GetType();
            var elementType = type.IsArray
                ? type.GetElementType()
                : type.IsGenericType ? type.GetGenericArguments()[0] : null;
            var isReferenceType = elementType != null && !TypeUtils.IsUnmanagedType(elementType);

            var sourceEnumerator = source.GetEnumerator();
            foreach (var item in destination)
            {
                Assert.True(sourceEnumerator.MoveNext());
                item.IsStructuralEqual(sourceEnumerator.Current);
                if (isReferenceType && item != null)
                {
                    item.IsNotSameReferenceAs(sourceEnumerator.Current);
                }
            }

            var defaultEqualityComparer = elementType != null
                ? typeof(EqualityComparer<>)
                    .MakeGenericType(elementType)
                    .GetProperty("Default", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    ?.GetValue(null)
                : null;

            if (!validateMembers)
                return;

            foreach (var field in source.GetType()
                .GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(f => !TypeUtils.IsUnmanagedType(f.FieldType) && f.GetValue(source) != null))
            {
                var sourceValue = field.GetValue(source);
                var destinationValue = field.GetValue(destination);

                if (sourceValue == defaultEqualityComparer)
                    continue;

                destinationValue.IsNotSameReferenceAs(sourceValue);
            }
        }

#if NET8_0_OR_GREATER
        private void TestSet<T, E>(T source, bool copyable = true, IEqualityComparer<E>? equalityComparer = null)
#else
        private void TestSet<T, E>(T source, bool copyable = true, IEqualityComparer<E> equalityComparer = null)
#endif
            where T : ISet<E>
        {
            var cloned = ObjectCloner.Clone(source);
            ValidateSet(source, cloned, equalityComparer);

            cloned = ObjectCloner.Clone(source, true);
            ValidateSet(source, cloned, equalityComparer);

            if (!typeof(T).IsValueType)
            {
#if NET8_0_OR_GREATER
                var copied = (ISet<E>)RuntimeHelpers.GetUninitializedObject(source.GetType());
#else
                var copied = (ISet<E>)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(source.GetType());
#endif

                if (copyable)
                {
                    ObjectCloner.CopyTo(source, copied);
                    ValidateSet(source, copied, equalityComparer);
                }
                else
                {
                    Assert.Throws<NotSupportedException>(() => ObjectCloner.CopyTo(source, copied));
                }
            }
        }

#if NET8_0_OR_GREATER
        private void ValidateSet<T>(ISet<T> source, ISet<T> destination, IEqualityComparer<T>? comparer)
#else
        private void ValidateSet<T>(ISet<T> source, ISet<T> destination, IEqualityComparer<T> comparer)
#endif
        {
            var type = typeof(T);
            var isReferenceType = !TypeUtils.IsUnmanagedType(type);

            destination.IsNotSameReferenceAs(source);

            if (!isReferenceType)
            {
                Assert.True(destination.SetEquals(source));
                return;
            }

            Assert.Equal(source.Count, destination.Count);
            Assert.All(destination, item => Assert.Contains(item, source, comparer ?? EqualityComparer<T>.Default));
        }

#if NET8_0_OR_GREATER
        class NodeEqualityComparer : IEqualityComparer<Node?>
#else
        class NodeEqualityComparer : IEqualityComparer<Node>
#endif
        {
#if NET8_0_OR_GREATER
            public bool Equals(Node? x, Node? y)
#else
            public bool Equals(Node x, Node y)
#endif
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Value == y.Value;
            }

            public int GetHashCode(Node obj)
            {
                return obj?.Value.GetHashCode() ?? 0;
            }
        }

#if NET8_0_OR_GREATER
        class NodeComparer : IComparer<Node?>
#else
        class NodeComparer : IComparer<Node>
#endif
        {
#if NET8_0_OR_GREATER
            public int Compare(Node? x, Node? y)
#else
            public int Compare(Node x, Node y)
#endif
            {
                return Comparer<int>.Default.Compare(x?.Value ?? 0, y?.Value ?? 0);
            }
        }
    }
}
