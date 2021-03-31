using System;
using System.Collections.Generic;
using Xunit;

namespace DeepCopy.Test
{
    public class UnitTest
    {
        [Fact]
        public void ValueTypeTest()
        {
            var source = new TestObject();
            var clone = ObjectCloner.Clone(source);

            clone.Is(source);

            clone.IntArray.IsNotSameReferenceAs(source.IntArray);
            clone.IntList.IsNotSameReferenceAs(source.IntList);
            clone.Children.IsNotSameReferenceAs(source.Children);
        }

        [Fact]
        public void AnoymousTypesTest()
        {
            var obj = new
            {
                Id = 1,
                Name = "Hoge",
                Array = new[]
                {
                    new { Name = "Foo" }
                },
                List = new List<int> { 10, 20, 30 }
            };

            var cloned = ObjectCloner.Clone(obj);

            cloned.IsStructuralEqual(obj);
        }

        [Fact]
        public void AttributeTest()
        {
            var obj = new AttributeTestObject();

            var cloned = ObjectCloner.Clone(obj);


            cloned.NotCopyIntValue.Is(0);
            cloned.NotCopyInstance.IsNull();
            cloned.NotCopyArray.IsNull();

            cloned.NotCopyIntValue = obj.NotCopyIntValue;
            cloned.NotCopyInstance = obj.NotCopyInstance;
            cloned.NotCopyArray = obj.NotCopyArray;

            cloned.IsStructuralEqual(obj);

            cloned.DefaultInstance.IsNotSameReferenceAs(obj.DefaultInstance);
            cloned.DefaultArray.IsNotSameReferenceAs(obj.DefaultArray);
            for (int i = 0; i < cloned.DefaultArray.Length; ++i)
                cloned.DefaultArray[i].IsNotSameReferenceAs(obj.DefaultArray[i]);

            cloned.DeepCopyInstance.IsNotSameReferenceAs(obj.DeepCopyInstance);
            cloned.DeepCopyArray.IsNotSameReferenceAs(obj.DeepCopyArray);
            for (int i = 0; i < cloned.DefaultArray.Length; ++i)
                cloned.DeepCopyArray[i].IsNotSameReferenceAs(obj.DeepCopyArray[i]);

            cloned.ShallowCopyInstance.IsNotSameReferenceAs(obj.ShallowCopyInstance);
            cloned.ShallowCopyInstance.Id.IsSameReferenceAs(obj.ShallowCopyInstance.Id);
            cloned.ShallowCopyArray.IsNotSameReferenceAs(obj.ShallowCopyArray);
            for (int i = 0; i < cloned.DefaultArray.Length; ++i)
                cloned.ShallowCopyArray[i].IsSameReferenceAs(obj.ShallowCopyArray[i]);

            cloned.SimpleCopyInstance.IsSameReferenceAs(obj.SimpleCopyInstance);
            cloned.SimpleCopyArray.IsSameReferenceAs(obj.SimpleCopyArray);
        }

        [Fact]
        public void PolymorphicTest()
        {
            PolymoirphicTestObject.I i = new PolymoirphicTestObject.B(5);

            var obj = new
            {
                B = new PolymoirphicTestObject.B(10),
                I = i,
            };

            var cloned = ObjectCloner.Clone(obj);

            cloned.IsStructuralEqual(obj);
            cloned.B.Is(obj.B);
            cloned.I.Is(obj.I);
        }

        [Fact]
        public void ObjectTest()
        {
            var obj = new ObjectTestObject();

            var cloned = ObjectCloner.Clone(obj);

            cloned.Obj.IsNotSameReferenceAs(obj.Obj);
            cloned.Child.Is(obj.Child);
            ((Child)cloned.Child).IntArray.IsNotSameReferenceAs(((Child)obj.Child).IntArray);
        }

        [Fact]
        public void ArrayTest()
        {
            var obj = new ArrayTestObject();

            var cloned = ObjectCloner.Clone(obj);

            cloned.IsStructuralEqual(obj);
            cloned.IntArray.IsNotSameReferenceAs(obj.IntArray);
            cloned.IntArray2.IsNotSameReferenceAs(obj.IntArray2);
            cloned.IntArray3.IsNotSameReferenceAs(obj.IntArray3);
            cloned.IntjaggedArray.IsNotSameReferenceAs(obj.IntjaggedArray);
            cloned.IntjaggedArray[0].IsNotSameReferenceAs(obj.IntjaggedArray[0]);

            cloned.ObjArray.IsNotSameReferenceAs(obj.ObjArray);
            cloned.ObjArray2.IsNotSameReferenceAs(obj.ObjArray2);
            cloned.ObjJaggedArray.IsNotSameReferenceAs(obj.ObjJaggedArray);
            cloned.ObjJaggedArray[0].IsNotSameReferenceAs(obj.ObjJaggedArray[0]);
        }

        [Fact]
        public void DelegateTest()
        {
            var obj = new DelegateTestObject(() => Console.WriteLine("Action"));
            obj.TestEvent += (_, __) => Console.WriteLine("Event");

            var cloned = ObjectCloner.Clone(obj);

            cloned.TestAction.IsSameReferenceAs(obj.TestAction);
            cloned.IsEventEmpty.IsTrue();
        }

        [Fact]
        public void NullReferenceTest()
        {
            var obj = new NullReferenceTestObject();
            var cloned = ObjectCloner.Clone(obj);

            cloned.Value2 = new List<int>(new[] { 1 });
            cloned.Value3 = new System.Collections.ObjectModel.ObservableCollection<Child>();
            cloned.Value3.Add(new Child());

            ObjectCloner.CopyTo(cloned, obj);

            cloned.Value2.IsNotSameReferenceAs(obj.Value2);
            cloned.Value2[0].IsNotSameReferenceAs(obj.Value2[0]);
            cloned.Value3.IsNotSameReferenceAs(obj.Value3);
            cloned.Value3[0].IsNotSameReferenceAs(obj.Value3[0]);
        }

        [Fact]
        public void CrossReferenceTest()
        {
            var obj = new CrossReferenceObject();
            var cloned = ObjectCloner.Clone(obj, true);

            cloned.IsNotSameReferenceAs(obj);
            cloned.A.IsNotSameReferenceAs(obj.A);
            cloned.B.IsNotSameReferenceAs(obj.B);

            cloned.A.IsSameReferenceAs(cloned.B.A);
            cloned.B.IsSameReferenceAs(cloned.A.B);
        }

        [Fact]
        public void DirectArrayTest()
        {
            var array1 = new int[] { 1, 2, 3, 4, 5 };
            var cloned1 = ObjectCloner.Clone(array1);
            cloned1.IsStructuralEqual(array1);
            cloned1.IsNotSameReferenceAs(array1);
            IsElementNotSameReference(array1, cloned1);

            var array2 = new int[][]
            {
                new [] { 1, 2, 3, 4, 5},
                new [] { 6, 7, 8, 9 },
                new [] { 10, 11, 12}
            };
            var cloned2 = ObjectCloner.Clone(array2);
            cloned2.IsStructuralEqual(array2);
            cloned2.IsNotSameReferenceAs(array2);
            IsElementNotSameReference(array2, cloned2);

            var array3 = new int[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            };
            var cloned3 = ObjectCloner.Clone(array3);
            cloned3.IsStructuralEqual(array3);
            cloned3.IsNotSameReferenceAs(array3);
            IsElementNotSameReference(array3, cloned3);

            var array4 = new int[2, 3, 4]
            {
                { {1, 2, 3, 4 }, {2, 3, 4, 5 }, {3, 4, 5, 6 } },
                { {4, 5, 6, 7 }, {5, 6, 7, 8 }, {6, 7, 8, 9 } }
            };
            var cloned4 = ObjectCloner.Clone(array4);
            cloned4.IsStructuralEqual(array4);
            cloned4.IsNotSameReferenceAs(array4);
            IsElementNotSameReference(array4, cloned4);

            var arrayA = new[] { new TestObject(), new TestObject() };
            var clonedA = ObjectCloner.Clone(arrayA);
            clonedA.IsStructuralEqual(arrayA);
            clonedA.IsNotSameReferenceAs(arrayA);
            IsElementNotSameReference(arrayA, clonedA);

            var arrayB = new TestObject[][]
            {
                new [] { new TestObject(), new TestObject(), new TestObject() },
                new [] { new TestObject(), new TestObject() },
                new [] { new TestObject(), new TestObject(), new TestObject() },
            };
            var clonedB = ObjectCloner.Clone(arrayB);
            clonedB.IsStructuralEqual(arrayB);
            clonedB.IsNotSameReferenceAs(arrayB);
            IsElementNotSameReference(arrayB, clonedB);

            var arrayC = new TestObject[2, 3]
            {
                { new TestObject(), new TestObject(), new TestObject() },
                { new TestObject(), new TestObject(), new TestObject() }
            };
            var clonedC = ObjectCloner.Clone(arrayC);
            clonedC.IsStructuralEqual(arrayC);
            clonedC.IsNotSameReferenceAs(arrayC);
            IsElementNotSameReference(arrayC, clonedC);

            var arrayD = new TestObject[2, 3, 2]
            {
                { {new TestObject(), new TestObject() }, {new TestObject(), new TestObject() }, {new TestObject(), new TestObject() } },
                { {new TestObject(), new TestObject() }, {new TestObject(), new TestObject() }, {new TestObject(), new TestObject() } }
            };
            var clonedD = ObjectCloner.Clone(arrayD);
            clonedD.IsStructuralEqual(arrayD);
            clonedD.IsNotSameReferenceAs(arrayD);
            IsElementNotSameReference(arrayD, clonedD);

        }


        private static void IsElementNotSameReference<T>(T[] source, T[] cloned)
        {
            for (int i = 0; i < cloned.Length; ++i)
            {
                cloned[i].IsNotSameReferenceAs(source[i]);
            }
        }

        private static void IsElementNotSameReference<T>(T[,] source, T[,] cloned)
        {
            for (int i = 0; i < cloned.GetLength(0); ++i)
            {
                for (int j = 0; j < cloned.GetLength(1); ++j)
                {
                    cloned[i, j].IsNotSameReferenceAs(source[i, j]);
                }
            }
        }

        private static void IsElementNotSameReference<T>(T[,,] source, T[,,] cloned)
        {
            for (int i = 0; i < cloned.GetLength(0); ++i)
            {
                for (int j = 0; j < cloned.GetLength(1); ++j)
                {
                    for (int k = 0; k < cloned.GetLength(2); ++k)
                    {
                        cloned[i, j, k].IsNotSameReferenceAs(source[i, j, k]);
                    }
                }
            }
        }
    }
}
