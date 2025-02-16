using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void DirectPrimitiveValueTypeArrayTest()
        {
            int[] intArray = new int[] { 10, 20, 30 };
            _ = ValidateCloneArray(intArray);

            double[] doubleArray = new double[] { 123.45, 234.56, 345.67 };
            _ = ValidateCloneArray(doubleArray);

            decimal[] decimalArray = new decimal[] { 1000.999M, 2M, 3M };
            _ = ValidateCloneArray(decimalArray);

            char[] charArray = new char[] { 'a', 'b', 'c' };
            _ = ValidateCloneArray(charArray);
        }

        [Fact]
        public void DirectNullablePrimitiveValueTypeArrayTest()
        {
            int?[] intArray = new int?[] { 9, null, 7 };
            _ = ValidateCloneArray(intArray);

            double?[] doubleArray = new double?[] { 122.45, null, 1.23 };
            _ = ValidateCloneArray(doubleArray);

            decimal?[] decimalArray = new decimal?[] { 999.999M, null, 3M };
            _ = ValidateCloneArray(decimalArray);

            char?[] charArray = new char?[] { 'a', null, 'c' };
            _ = ValidateCloneArray(charArray);
        }

        [Fact]
        public void DirectObjectArrayTest()
        {
            object[] objectArray = new object[] { new object(), new object(), new object() };
            _ = ValidateCloneArray(objectArray);

#if NET8_0_OR_GREATER
            object?[] nullableObjectArray = [new (), null, new ()];
#else
            object[] nullableObjectArray = new object[] { new object(), null, new object()};
#endif
            _ = ValidateCloneArray(nullableObjectArray);
        }

        [Fact]
        public void DirectObjectBaseArrayTest()
        {
            object[] objectArray = new object[] { 1, 21.3, new TestObject() };
            _ = ValidateCloneArray(objectArray);

#if NET8_0_OR_GREATER
            object?[] nullableObjectArray = [1, null, new TestObject()];
#else
            object[] nullableObjectArray = new object[]{1, null, new TestObject()};
#endif
            _ = ValidateCloneArray(nullableObjectArray);
        }

        [Fact]
        public void DirectClassArrayTest()
        {
            TestObject[] objectArray = new TestObject[] { new TestObject(), new TestObject(), new TestObject() };
            _ = ValidateCloneArray(objectArray);

#if NET8_0_OR_GREATER
            TestObject?[] nullableObjectArray = [new (), null, new ()];
#else
            TestObject[] nullableObjectArray = new TestObject[]{new TestObject(), null, new TestObject()};
#endif
            _ = ValidateCloneArray(nullableObjectArray);
        }

        [Fact]
        public void DirectStructValueTypeArrayTest()
        {
            StructData[] structArray = new StructData[] {
                new StructData { Id = 1, Name = "foo", Value = new TestObject() },
                default,
                new StructData { Id = 2, Name = "bar", Value = null }
            };
            var cloned = ValidateCloneArray(structArray);

            for (int i = 0; i < cloned.Length; i++)
            {
                cloned[i].IsNotSameReferenceAs(structArray[i]);
                ValidateValue(structArray[i].Value, cloned[i].Value);
            }
        }

        [Fact]
        public void DirectNullableStructValueTypArrayTest()
        {
            StructData?[] structArray = new StructData?[] {
                new StructData { Id = 1, Name = "foo", Value = new TestObject() },
                default,
                new StructData { Id = 2, Name = "bar", Value = null }
            };
            var cloned = ValidateCloneArray(structArray);

            for (int i = 0; i < cloned.Length; i++)
            {
                cloned[i].IsNotSameReferenceAs(structArray[i]);
                ValidateValue(structArray[i]?.Value, cloned[i]?.Value);
            }
        }



        [Fact]
        public void DirectEnumValueTypeArrayTest()
        {
            EnumData[] value = new EnumData[] { EnumData.C, default, EnumData.A };
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
            for (int i = 0; i < cloned.Length; i++)
            {
                if (cloned[i].Item3 != null)
                {
                    cloned[i].Item3.IsNotSameReferenceAs(tuple[i].Item3);
                }
            }
        }

        [Fact]
        public void DirectArrayAsObjectTest()
        {
            object obj = new object[] { new object(), new object(), new object() };
            // Since the actual type should be known, please cast explicitly.
            _ = ValidateCloneArray((object[])obj);
        }

        [Fact]
        public void ReadonlyPrimitiveValueTypeArrayTest()
        {
            var cls = new
            {
                intArray = new int[] { 10, 20, 30 },
                doubleArray = new double[] { 123.45, 234.56, 345.67 },
                decimalArray = new decimal[] { 1000.999M, 2M, 3M },
                charArray = new char[] { 'a', 'b', 'c' }
            };
            var cloned = ValidateCloneObject(cls);

            cloned.intArray.IsNotSameReferenceAs(cls.intArray);
            cloned.doubleArray.IsNotSameReferenceAs(cls.doubleArray);
            cloned.decimalArray.IsNotSameReferenceAs(cls.decimalArray);
            cloned.charArray.IsNotSameReferenceAs(cls.charArray);
        }

        [Fact]
        public void ReadonlyNullablePrimitiveValueTypeArrayTest()
        {
            var cls = new
            {
                intArray = new int?[] { 10, null, 30 },
                doubleArray = new double?[] { 123.45, null, 345.67 },
                decimalArray = new decimal?[] { 1000.999M, null, 3M },
                charArray = new char?[] { 'a', null, 'c' }
            };
            var cloned = ValidateCloneObject(cls);

            cloned.intArray.IsNotSameReferenceAs(cls.intArray);
            cloned.doubleArray.IsNotSameReferenceAs(cls.doubleArray);
            cloned.decimalArray.IsNotSameReferenceAs(cls.decimalArray);
            cloned.charArray.IsNotSameReferenceAs(cls.charArray);
        }

        [Fact]
        public void ReadonlyObjectArrayTest()
        {
            var cls = new
            {
                objectArray = new object[] { new object(), new object(), new object() },
#if NET8_0_OR_GREATER

                nullableObjectArray = new object?[] { new(), null, new() },
#else
                nullableObjectArray = new object[] { new object(), null, new object() },
#endif
                objectBaseArray = new object[] { 1, 21.3, new TestObject() },
#if NET8_0_OR_GREATER
                nullableObjectBaseArray = new object?[] { 1, null, new TestObject() },
#else
                nullableObjectBaseArray = new object[] { 1, null, new TestObject() },
#endif
                classArray = new TestObject[] { new TestObject(), new TestObject(), new TestObject() },
#if NET8_0_OR_GREATER
                nullableClassArray = new TestObject?[] { new(), null, new() },
#else
                nullableClassArray = new TestObject[] { new TestObject(), null, new TestObject() },
#endif
            };
            var cloned = ValidateCloneObject(cls);

            cloned.objectArray.IsNotSameReferenceAs(cls.objectArray);
            cloned.nullableObjectArray.IsNotSameReferenceAs(cls.nullableObjectArray);
            cloned.objectBaseArray.IsNotSameReferenceAs(cls.objectBaseArray);
            cloned.nullableObjectBaseArray.IsNotSameReferenceAs(cls.nullableObjectBaseArray);
            cloned.classArray.IsNotSameReferenceAs(cls.classArray);
            cloned.nullableClassArray.IsNotSameReferenceAs(cls.nullableClassArray);
        }

        [Fact]
        public void ReadonlyStructContainReferenceArrayTest()
        {
            var cls = new
            {
                Id = 1,
                StructArray = new StructData[] {
                    new StructData() { Id = 1, Name = "foo", Value = new TestObject() },
                    default,
                    new StructData() { Id = 2, Name = "bar", Value = null }
                }
            };
            var cloned = ValidateCloneObject(cls);

            for (int i = 0; i < cloned.StructArray.Length; i++)
            {
                ValidateValue(cls.StructArray[i].Value, cloned.StructArray[i].Value);
            }
        }

        [Fact]
        public void ReadonlyArrayAsObjectTest()
        {
            var cls = new
            {
                obj1 = (object)(new object[] { new object(), new object(), new object() }),
#if NET8_0_OR_GREATER
                obj2 = (object)(new object?[] { new (), null, new () }),
#else
                obj2 = (object)(new object[] { new object(), null, new object() }),
#endif
                obj3 = (object)(new object[] { 1, 21.3, new TestObject() }),
#if NET8_0_OR_GREATER
                obj4 = (object)(new object?[] { 1, null, new TestObject() }),
#else
                obj4 = (object)(new object[] { 0, null, new TestObject() }),
#endif
                obj5 = (object)(new TestObject[] { new TestObject(), new TestObject(), new TestObject() }),
#if NET8_0_OR_GREATER
                obj6 = (object)(new TestObject?[] { new (), null, new() }),
#else
                obj6 = (object)(new TestObject[] { new TestObject(), null, new TestObject() }),
#endif
            };
            var cloned = ValidateCloneObject(cls);

            cloned.obj1.IsNotSameReferenceAs(cls.obj1);
            cloned.obj2.IsNotSameReferenceAs(cls.obj2);
            cloned.obj3.IsNotSameReferenceAs(cls.obj3);
            cloned.obj4.IsNotSameReferenceAs(cls.obj4);
            cloned.obj5.IsNotSameReferenceAs(cls.obj5);
            cloned.obj6.IsNotSameReferenceAs(cls.obj6);
        }

        private T ValidateCloneObject<T>(in T @object)
        {
            var cloned = ObjectCloner.Clone(@object);
            cloned.IsNotSameReferenceAs(@object);
            cloned.IsStructuralEqual(@object);

            return cloned;
        }

        private T[] ValidateCloneArray<T>(in T[] array)
        {
            var cloned = ObjectCloner.Clone(array);
            cloned.IsNotSameReferenceAs(array);
            cloned.IsStructuralEqual(array);

            for (int i=0; i<array.Length; i++)
            {
                if (!(array[i]?.GetType().IsValueType ?? true) && array[i]?.GetType() != typeof(string))
                cloned[i].IsNotSameReferenceAs(array[i]);
            }

            return cloned;
        }

#if NET8_0_OR_GREATER
        private void ValidateValue<T>(in T? value, in T? cloned)
#else
        private void ValidateValue<T>(in T value, in T cloned)
#endif
        {
            cloned.IsStructuralEqual(value);

            if (cloned == null) return;
            cloned.IsNotSameReferenceAs(value);
        }
    }
}
