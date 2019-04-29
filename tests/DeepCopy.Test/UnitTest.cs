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

        //[Fact]
        //public void DirectArrayTest()
        //{
        //    var array = new int[] { 1, 2, 3, 4, 5 };

        //    var cloned = ObjectCloner.Clone(array);

        //    cloned.IsStructuralEqual(array);
        //}
    }
}
