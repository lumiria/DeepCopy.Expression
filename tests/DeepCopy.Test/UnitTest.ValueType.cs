using System;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void PrimitiveValueTypeTest()
        {
            int intValue = 10;
            var clonedIntValue = ObjectCloner.Clone(intValue);
            clonedIntValue.Is(intValue);

            int clonedIntValue2 = default;
            ObjectCloner.CopyTo(intValue, ref clonedIntValue2);
            clonedIntValue2.Is(intValue);

            double doubleValue = 123.45;
            var clonedDoubleValue = ObjectCloner.Clone(doubleValue);
            clonedDoubleValue.Is(doubleValue);

            double clonedDoubleValue2 = default;
            ObjectCloner.CopyTo(doubleValue, ref clonedDoubleValue2);
            clonedDoubleValue2.Is(doubleValue);

            decimal decimalValue = 1000.999M;
            var clonedDecimalValue = ObjectCloner.Clone(decimalValue);
            clonedDecimalValue.Is(decimalValue);

            decimal clonedDecimalValue2 = default;
            ObjectCloner.CopyTo(decimalValue, ref clonedDecimalValue2);
            clonedDecimalValue2.Is(decimalValue);

            char charValue = 'a';
            var clonedCharValue = ObjectCloner.Clone(charValue);
            clonedCharValue.Is(charValue);


            char clonedCharValue2 = default;
            ObjectCloner.CopyTo(charValue, ref clonedCharValue2);
            clonedCharValue2.Is(charValue);
        }

        [Fact]
        public void NullablePrimitiveValueTypeTest()
        {
            int? intValue = 9;
            var clonedIntValue = ObjectCloner.Clone(intValue);
            clonedIntValue.Is(intValue);

            int? clonedIntValue1 = default;
            ObjectCloner.CopyTo(intValue, ref clonedIntValue1);
            clonedIntValue1.Is(intValue);

            intValue = null;
            clonedIntValue = ObjectCloner.Clone(intValue);
            clonedIntValue.Is(intValue);
            ObjectCloner.CopyTo(intValue, ref clonedIntValue1);
            clonedIntValue1.Is(intValue);

            double? doubleValue = 122.45;
            var clonedDoubleValue = ObjectCloner.Clone(doubleValue);
            clonedDoubleValue.Is(doubleValue);

            double? clonedDoubleValue1 = default;
            ObjectCloner.CopyTo(doubleValue, ref clonedDoubleValue1);
            clonedDoubleValue1.Is(doubleValue);

            doubleValue = null;
            clonedDoubleValue = ObjectCloner.Clone(doubleValue);
            clonedDoubleValue.Is(doubleValue);
            ObjectCloner.CopyTo(doubleValue, ref clonedDoubleValue1);
            clonedDoubleValue1.Is(doubleValue);

            decimal? decimalValue = 999.999M;
            var clonedDecimalValue = ObjectCloner.Clone(decimalValue);
            clonedDecimalValue.Is(decimalValue);

            decimal? clonedDecimalValue1 = default;
            ObjectCloner.CopyTo(decimalValue, ref clonedDecimalValue1);
            clonedDecimalValue1.Is(decimalValue);

            decimalValue = null;
            clonedDecimalValue = ObjectCloner.Clone(decimalValue);
            clonedDecimalValue.Is(decimalValue);
            ObjectCloner.CopyTo(decimalValue, ref clonedDecimalValue1);
            clonedDecimalValue1.Is(decimalValue);

            char? charValue = 'a';
            var clonedCharValue = ObjectCloner.Clone(charValue);
            clonedCharValue.Is(charValue);

            char? clonedCharValue1 = default;
            ObjectCloner.CopyTo(charValue, ref clonedCharValue1);
            clonedCharValue1.Is(charValue);

            charValue = null;
            clonedCharValue = ObjectCloner.Clone(charValue);
            clonedCharValue.Is(charValue);
            ObjectCloner.CopyTo(charValue, ref clonedCharValue1);
            clonedCharValue1.Is(charValue);
        }

        [Fact]
        public void StructValueTypeTest()
        {
            var value = new StructData { Id = 1, Name = "foo", Value = new TestObject()};
            var clonedValue = ObjectCloner.Clone(value);
            clonedValue.IsStructuralEqual(value);
            clonedValue.Value.IsNotSameReferenceAs(value.Value);

            StructData clonedValue2 = default;
            ObjectCloner.CopyTo(value, ref clonedValue2);
            clonedValue2.IsStructuralEqual(value);
            clonedValue2.Value.IsNotSameReferenceAs(value.Value);
        }

        [Fact]
        public void NullableStructValueTypeTest()
        {
            StructData? value = new StructData { Id = 1, Name = "foo", Value = new TestObject() };
            var clonedValue = ObjectCloner.Clone(value);
            clonedValue.IsStructuralEqual(value);
            clonedValue.Value.IsNotSameReferenceAs(value.Value);

            StructData? clonedValue2 = default;
            ObjectCloner.CopyTo(value, ref clonedValue2);
            clonedValue2.IsStructuralEqual(value);
            clonedValue2.Value.IsNotSameReferenceAs(value.Value);

            value = new StructData { Id = null, Name = "var", Value = null };
            clonedValue = ObjectCloner.Clone(value);
            clonedValue.IsStructuralEqual(value);
            clonedValue.Value.IsNotSameReferenceAs(value.Value);
            ObjectCloner.CopyTo(value, ref clonedValue2);
            clonedValue2.IsStructuralEqual(value);
            clonedValue2.Value.IsNotSameReferenceAs(value.Value);

            value = null;
            clonedValue = ObjectCloner.Clone(value);
            clonedValue.Is(null);
            ObjectCloner.CopyTo(value, ref clonedValue2);
            clonedValue.Is(null);
        }

        [Fact]
        public void HasStructValueTypeTest()
        {
            var cls = new
            {
                Guid = Guid.NewGuid(),
                //StructData = new StructData { Id = 1, Name = "foo", Value = new TestObject() }
            };
            var clonedValue = ObjectCloner.Clone(cls);
            clonedValue.IsStructuralEqual(cls);
            //clonedValue.StructData.Value.IsNotSameReferenceAs(cls.StructData.Value);
        }

        [Fact]
        public void IStructValueTypeTest()
        {
            IStructData value = new StructData { Id = 1, Name = "foo", Value = new TestObject() };
            var clonedValue = ObjectCloner.Clone(value);
            clonedValue.IsStructuralEqual(value);
            ((StructData)clonedValue).Value.IsNotSameReferenceAs(((StructData)value).Value);

            StructData clonedValue2 = default;
            ObjectCloner.CopyTo((StructData)value, ref clonedValue2);
            clonedValue2.IsStructuralEqual(value);
            ((StructData)clonedValue2).Value.IsNotSameReferenceAs(((StructData)value).Value);

            var cls = new
            {
                Data = (IStructData)(new StructData { Id = 1, Name = "foo", Value = new TestObject() })
            };
            var clonedValue3 = ObjectCloner.Clone(cls);
            clonedValue3.IsStructuralEqual(cls);

        }

        [Fact]
        public void EnumValueTypeTest()
        {
            var value = EnumData.C;
            var clonedValue = ObjectCloner.Clone(value);
            clonedValue.Is(value);

            EnumData clonedValue2 = default;
            ObjectCloner.CopyTo(value, ref clonedValue2);
            clonedValue2.Is(value);
        }

        [Fact]
        public void TupleTest()
        {
            var tuple1 = (1, "foo", new TestObject());
            var cloned1 = ObjectCloner.Clone(tuple1);

            cloned1.Item3.IsNotSameReferenceAs(tuple1.Item3);
            cloned1.IsStructuralEqual(tuple1);

            var tuple2 = (Id: 2, Name: "bar", Value: new TestObject());
            var cloned2 = ObjectCloner.Clone(tuple2);
            cloned2.Value.IsNotSameReferenceAs(tuple2.Value);
            cloned2.IsStructuralEqual(tuple2);

            var tuple3 = tuple1 with { Item1 = 3, Item2 = "baz", Item3 = null };
            ObjectCloner.CopyTo(tuple1, ref tuple3);
            tuple3.Item3.IsNotSameReferenceAs(tuple1.Item3);
            tuple3.IsStructuralEqual(tuple1);
        }

        interface IStructData
        {
            string Write() => "This is SttructData";
        }

        internal struct StructData : IStructData
        {
            public int? Id;
            public string Name;
            public object Value;
        }

        internal enum EnumData
        {
            A,
            B,
            C
        }
    }
}
