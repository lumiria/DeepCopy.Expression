using System;
using System.Linq;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void PrimitiveValueTypeArrayTest()
        {
            int[] intArray = new int[] { 10, 20, 30 };
            var clonedIntArray = ObjectCloner.Clone(intArray);
            clonedIntArray.SequenceEqual(intArray);

            double[] doubleArray = new double[] { 123.45, 234.56, 345.67 };
            var clonedDoubleArray = ObjectCloner.Clone(doubleArray);
            clonedDoubleArray.Is(doubleArray);

            decimal[] decimalArray = new decimal[] { 1000.999M, 2M, 3M };
            var clonedDecimalArray = ObjectCloner.Clone(decimalArray);
            clonedDecimalArray.Is(decimalArray);

            char[] charArray = new char[] { 'a', 'b', 'c' };
            var clonedCharArrayValue = ObjectCloner.Clone(charArray);
            clonedCharArrayValue.Is(charArray);
        }

        [Fact]
        public void NullablePrimitiveValueTypeArrayTest()
        {
            int?[] intArray = { 9, null, 7 };
            var clonedIntArrayValue = ObjectCloner.Clone(intArray);
            clonedIntArrayValue.Is(intArray);

            double?[] doubleArray = { 122.45, null, 1.23 };
            var clonedDoubleArrayValue = ObjectCloner.Clone(doubleArray);
            clonedDoubleArrayValue.Is(doubleArray);

            decimal?[] decimalArray = { 999.999M, null, 3M };
            var clonedDecimalArray = ObjectCloner.Clone(decimalArray);
            clonedDecimalArray.Is(decimalArray);

            char?[] charArray = { 'a', null, 'c' };
            var clonedCharArray = ObjectCloner.Clone(charArray);
            clonedCharArray.Is(charArray);
        }

        [Fact]
        public void StructValueTypeArrayTest()
        {
            StructData[] structArray = {
                new StructData { Id = 1, Name = "foo", Value = new TestObject() },
                default,
                new StructData { Id = 2, Name = "bar", Value = null }
            };
            var clonedValue = ObjectCloner.Clone(structArray);
            clonedValue.IsStructuralEqual(structArray);
            for (int i = 0; i<clonedValue.Length; i++)
            {
                if (clonedValue[i].Value == null)
                {
                    clonedValue[i].Value.Is(structArray[i].Value);
                }
                else
                {
                    clonedValue[i].Value.IsNotSameReferenceAs(structArray[i].Value);
                } 
            }
        }

        [Fact]
        public void NullableStructValueTypArrayTest()
        {
            StructData?[] structArray = {
                new StructData { Id = 1, Name = "foo", Value = new TestObject() },
                default,
                new StructData { Id = 2, Name = "bar", Value = null }
            };
            var clonedValue = ObjectCloner.Clone(structArray);
            clonedValue.IsStructuralEqual(structArray);
            for (int i = 0; i < clonedValue.Length; i++)
            {
                if (clonedValue[i]?.Value != null)
                {
                    clonedValue[i].Value.IsNotSameReferenceAs(structArray[i].Value);
                }
            }
        }

        [Fact]
        public void NestedStructValueTypeArrayTest()
        {
            var cls = new
            {
                Id = 1,
                StructArray = new StructData[] {
                    new StructData { Id = 1, Name = "foo", Value = new TestObject() },
                    default,
                    new StructData { Id = 2, Name = "bar", Value = null }
                }
            };
            var clonedValue = ObjectCloner.Clone(cls);
            clonedValue.IsStructuralEqual(cls);
            for (int i = 0; i < clonedValue.StructArray.Length; i++)
            {
                if (clonedValue.StructArray[i].Value == null)
                {
                    clonedValue.StructArray[i].Value.Is(cls.StructArray[i].Value);
                }
                else
                {
                    clonedValue.StructArray[i].Value.IsNotSameReferenceAs(cls.StructArray[i].Value);
                }
            }
        }

        [Fact]
        public void EnumValueTypeArrayTest()
        {
            EnumData[] value = { EnumData.C, default, EnumData.A };
            var clonedValue = ObjectCloner.Clone(value);
            clonedValue.Is(value);
        }

        [Fact]
        public void TupleArrayTest()
        {
            (int Id, string Name, TestObject Value)[] tuple = {
                (1, "foo", new TestObject()),
                (Id: 2, Name: "bar", Value: new TestObject()),
                default
            };

            var cloned = ObjectCloner.Clone(tuple);

            cloned.IsStructuralEqual(tuple);
            for (int i=0; i<cloned.Length; i++)
            {
                if (cloned[i].Item3 != null)
                {
                    cloned[i].Item3.IsNotSameReferenceAs(tuple[i].Item3);
                }
            }
        }
    }
}
