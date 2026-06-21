using System;
using System.Xml;
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
            var value = new StructData { Id = 1, Name = "foo", Value = new TestObject() };
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

#if NET8_0_OR_GREATER
            var tuple3 = tuple1 with { Item1 = 3, Item2 = "baz", Item3 = null };
#else
            var tuple3 = (3, "baz", (TestObject)null);
#endif
            ObjectCloner.CopyTo(tuple1, ref tuple3);
            tuple3.Item3.IsNotSameReferenceAs(tuple1.Item3);
            tuple3.IsStructuralEqual(tuple1);
        }

        [Fact]
        public void DateTimeTest()
        {
            var obj = new { DateTime = DateTime.Now };
            var cloned = ObjectCloner.Clone(obj);

            cloned.IsNotSameReferenceAs(obj);
            cloned.IsStructuralEqual(obj);
        }

        [Fact]
        public void ValueTypeObjectTest()
        {
            var obj = new ValueTypeObject();
            var cloned = ObjectCloner.Clone(obj);

            cloned.IsNotSameReferenceAs<ValueTypeObject>(obj);
            cloned.IsStructuralEqual(obj);

            cloned.StructWithRef.Ref.IsNotSameReferenceAs(obj.StructWithRef.Ref);
            cloned.NullableStructWithRef?.Ref.IsNotSameReferenceAs(obj.NullableStructWithRef?.Ref);
            cloned.ReadonlyStructWithRef.Ref.IsNotSameReferenceAs(obj.ReadonlyStructWithRef.Ref);
            cloned.NullableReadonlyStructWithRef?.Ref.IsNotSameReferenceAs(obj.NullableReadonlyStructWithRef?.Ref);
#if NET8_0_OR_GREATER
            cloned.RecordStructWithRef.Ref.IsNotSameReferenceAs(obj.RecordStructWithRef.Ref);
            cloned.NullableRecordStructWithRef?.Ref.IsNotSameReferenceAs(obj.NullableRecordStructWithRef?.Ref);
#endif
        }

        interface IStructData
        {
#if NET8_0_OR_GREATER
            string Write() => "This is StructData";
#else
            string Write();
#endif
        }

        internal struct StructData : IStructData
        {
            public int? Id;
            public string Name;
#if NET8_0_OR_GREATER
            public object? Value;
#else
            public object Value;

            public string Write() => "This is StructData";
#endif

        }

        internal enum EnumData
        {
            A,
            B,
            C
        }

        private sealed class ValueTypeObject
        {
            private int _intValue;

            private long _longValue;

            private float _floatValue;

            private double _doubleValue;

            private bool _booleanValue;

            private Decimal _decimalValue;

            private DateTime _dateTime;

            private Guid _guid;

            private string _stringValue;

            private TestEnum _enum;

            private StructWithRef _structWithRef;

            private ReadonlyStructWithRef _readonlyStructWithRef;

#if NET8_0_OR_GREATER
            private RecordStructWithRef _recordStructWithRef;
#endif

            private int? _nullableIntValue;

            private long? _nullableLongValue;

            private float? _nullableFloatValue;

            private double? _nullableDoubleValue;

            private bool? _nullableBoolValue1;

            private bool? _nullableBoolValue2;

            private Decimal? _nullableDecimalValue1;

            private Decimal? _nullableDecimalValue2;

            private DateTime? _nullableDateTimeValue;

            private Guid? _nullableGuidValue;

#if NET8_0_OR_GREATER
            private string? _nullableStringValue1;

            private string? _nullableStringValue2;
#else
            private string _nullableStringValue1;

            private string _nullableStringValue2;
#endif

            private TestEnum? _nullableEnum1;

            private TestEnum? _nullableEnum2;

            private StructWithRef? _nullableStructWithRef1;

            private StructWithRef? _nullableStructWithRef2;

            private ReadonlyStructWithRef? _nullableReadonlyStructWithRef1;

            private ReadonlyStructWithRef? _nullableReadonlyStructWithRef2;

#if NET8_0_OR_GREATER
            private RecordStructWithRef? _nullableRecordStructWithRef1;

            private RecordStructWithRef? _nullableRecordStructWithRef2;
#endif

            public ValueTypeObject()
            {
                var random = new Random(Environment.TickCount);

                _intValue = random.Next();
                _longValue = random.Next();
                _floatValue = (float)random.NextDouble();
                _doubleValue = random.NextDouble();
                _booleanValue = random.Next(0, 1) == 1;
                _decimalValue = new Decimal(random.NextDouble());
                _dateTime = DateTime.Now;
                _guid = Guid.NewGuid();
                _stringValue = Guid.NewGuid().ToString();
                _enum = TestEnum.B;
                _structWithRef = new StructWithRef(new Node { Value = 1 }, 10);
                _readonlyStructWithRef = new ReadonlyStructWithRef(new Node { Value = 2 });
#if NET8_0_OR_GREATER
                _recordStructWithRef = new RecordStructWithRef(new Node { Value = 3 }, "x");
#endif

                _nullableIntValue = random.Next();
                _nullableFloatValue = null;
                _nullableFloatValue = (float)random.NextDouble();
                _nullableDoubleValue = null;
                _nullableBoolValue1 = random.Next(0, 1) == 1;
                _nullableBoolValue2 = null;
                _nullableDecimalValue1 = new Decimal(random.NextDouble());
                _nullableDecimalValue2 = null;
                _nullableDateTimeValue = DateTime.Now;
                _nullableGuidValue = null;
                _nullableStringValue1 = Guid.NewGuid().ToString();
                _nullableStringValue2 = null;
                _nullableEnum1 = TestEnum.C;
                _nullableEnum2 = null;
                _nullableStructWithRef1 = new StructWithRef(new Node { Value = 4 }, 10);
                _nullableStructWithRef2 = null;
                _nullableReadonlyStructWithRef1 = new ReadonlyStructWithRef(new Node { Value = 5 });
                _nullableReadonlyStructWithRef2 = null;
#if NET8_0_OR_GREATER
                _nullableRecordStructWithRef1 = new RecordStructWithRef(new Node { Value = 6 }, "x");
                _nullableRecordStructWithRef2 = null;
#endif
            }

            internal StructWithRef StructWithRef => _structWithRef;

            internal ReadonlyStructWithRef ReadonlyStructWithRef => _readonlyStructWithRef;

#if NET8_0_OR_GREATER
            internal RecordStructWithRef RecordStructWithRef => _recordStructWithRef;
#endif

            internal StructWithRef? NullableStructWithRef => _nullableStructWithRef1;

            internal ReadonlyStructWithRef? NullableReadonlyStructWithRef => _nullableReadonlyStructWithRef1;

#if NET8_0_OR_GREATER
            internal RecordStructWithRef? NullableRecordStructWithRef => _nullableRecordStructWithRef1;
#endif
        }

        internal enum TestEnum
        {
            A, B, C, D, E
        }

#if NET8_0_OR_GREATER
        internal readonly record struct RecordStructWithRef(Node Ref, string Name);
#endif

        internal readonly struct ReadonlyStructWithRef
        {
            public ReadonlyStructWithRef(Node @ref)
            {
                Ref = @ref;
            }

            public Node Ref { get; }
        }

        internal struct StructWithRef
        {
            public StructWithRef(Node @ref, int number)
            {
                Ref = @ref;
                Number = number;
            }

            public Node Ref { get; set; }

            public int Number { get; set; }
        }
    }
}
