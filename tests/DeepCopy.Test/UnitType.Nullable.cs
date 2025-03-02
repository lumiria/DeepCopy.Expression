using System.Text;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void NullableValueTest()
        {
            int? nullableInt = 10;
            ValidateNullableValue(nullableInt);

            int? nullInt = null;
            ValidateNullableValue(nullInt);
        }

        [Fact]
        public void NullableValueStruct()
        {
            ValueStruct? nullableValue = new ValueStruct(10, "Test");
            ValidateNullableValue(nullableValue);

            ValueStruct? nullValue = null;
            ValidateNullableValue(nullValue);
        }

        [Fact]
        public void NullableMixedStruct()
        {
            MixedStruct? nullableValue = new MixedStruct(10, Encoding.UTF8);
            ValidateNullableValue(nullableValue);

            MixedStruct? nullValue = null;
            ValidateNullableValue(nullValue);
        }

        private void ValidateNullableValue<T>(T? value)
            where T : struct
        {
            var cloned = ObjectCloner.Clone(value);
            if (value != null)
                cloned.IsNotSameReferenceAs(value);
            cloned.Is(value);
        }

        private struct ValueStruct
        {
            public ValueStruct(int id, string value)
            {
                Id = id;
                Value = value;
            }

            int Id { get; set; }

            string Value { get; set; }
        }

        private struct MixedStruct
        {
            public MixedStruct(int id, Encoding encoding)
            {
                Id = id;
                Encoding = encoding;
            }

            int Id { get; set; }

            Encoding Encoding { get; set; }
        }
    }
}
