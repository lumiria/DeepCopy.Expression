using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
            object?[] nullableObjectArray = [new(), null, new()];
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
            TestObject?[] nullableObjectArray = [new(), null, new()];
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
                if (structArray[i] != null)
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
                obj2 = (object)(new object?[] { new(), null, new() }),
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
                obj6 = (object)(new TestObject?[] { new(), null, new() }),
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


        [Fact]
        private void JaggedArrayTest()
        {
            var jagged2 = new[] { new[] { 1 }, new[] { 3, 4 } };
            var cloned2 = ObjectCloner.Clone(jagged2);
            cloned2.IsNotSameReferenceAs(jagged2);
            cloned2.IsStructuralEqual(jagged2);
            TraverseJagged(jagged2, cloned2);

            var copied2 = (int[][])jagged2.Clone();
            ObjectCloner.CopyTo(jagged2, copied2);
            copied2.IsNotSameReferenceAs(jagged2);
            copied2.IsStructuralEqual(jagged2);
            TraverseJagged(jagged2, copied2);

            _ = ValidateCloneJaggedArray(jagged2);

            var jagged3 = new[] { new[] { new[] { new Node { Value = 1 } } }, new[] { new[] { new Node { Value = 2 }, new Node { Value = 3 } } } };
            var cloned3 = ObjectCloner.Clone(jagged3);
            cloned3.IsNotSameReferenceAs(jagged3);
            cloned3.IsStructuralEqual(jagged3);
            TraverseJagged(jagged3, cloned3);

            var copied3 = (Node[][][])jagged3.Clone();
            ObjectCloner.CopyTo(jagged3, copied3);
            copied3.IsNotSameReferenceAs(jagged3);
            copied3.IsStructuralEqual(jagged3);
            TraverseJagged(jagged3, copied3);

            _ = ValidateCloneJaggedArray(jagged3);

            var jagged4 = new[] { new[] { new[] { new[] { "a" } } }, new[] { new[] { new[] { "b", "c" } } } };
            var cloned4 = ObjectCloner.Clone(jagged4);
            cloned4.IsNotSameReferenceAs(jagged4);
            cloned4.IsStructuralEqual(jagged4);
            TraverseJagged(jagged4, cloned4);

            var copied4 = (string[][][][])jagged4.Clone();
            ObjectCloner.CopyTo(jagged4, copied4);
            copied4.IsNotSameReferenceAs(jagged4);
            copied4.IsStructuralEqual(jagged4);
            TraverseJagged(jagged4, copied4);

            _ = ValidateCloneJaggedArray(jagged4);

            var jagged5 = new[] { new[] { new[] { new[] { new[] { Guid.NewGuid() } } } }, new[] { new[] { new[] { new[] { Guid.NewGuid(), Guid.NewGuid() } } } } };
            var cloned5 = ObjectCloner.Clone(jagged5);
            cloned5.IsNotSameReferenceAs(jagged5);
            cloned5.IsStructuralEqual(jagged5);
            TraverseJagged(jagged5, cloned5);

            var copied5 = (Guid[][][][][])jagged5.Clone();
            ObjectCloner.CopyTo(jagged5, copied5);
            copied5.IsNotSameReferenceAs(jagged5);
            copied5.IsStructuralEqual(jagged5);
            TraverseJagged(jagged5, copied5);

            _ = ValidateCloneJaggedArray(jagged5);

            var JaggedNullable = new[] { new int?[] { 1 }, new int?[] { 3, 4, null }, null };
            var clonedNullable = ObjectCloner.Clone(JaggedNullable);
            clonedNullable.IsNotSameReferenceAs(JaggedNullable);
            clonedNullable.IsStructuralEqual(JaggedNullable);
            TraverseJagged(JaggedNullable, clonedNullable);

            var copiedNullable = (int?[][])JaggedNullable.Clone();
            ObjectCloner.CopyTo(JaggedNullable, copiedNullable);
            copiedNullable.IsNotSameReferenceAs(JaggedNullable);
            copiedNullable.IsStructuralEqual(JaggedNullable);
            TraverseJagged(JaggedNullable, copiedNullable);

            _ = ValidateCloneJaggedArray(JaggedNullable);
        }

        [Fact]
        private void MatrixArrayTest()
        {
            var matrix23 = new[,] { { 1, 2, 3 }, { 4, 5, 6 } };
            var cloned23 = ObjectCloner.Clone(matrix23);
            cloned23.IsNotSameReferenceAs(matrix23);
            cloned23.IsStructuralEqual(matrix23);
            TraverseMatrix(matrix23, cloned23);

            var copied23 = new int[2, 3];
            ObjectCloner.CopyTo(matrix23, copied23);
            copied23.IsNotSameReferenceAs(matrix23);
            copied23.IsStructuralEqual(matrix23);
            TraverseMatrix(matrix23, copied23);

            _ = ValidateCloneMatrixArray(matrix23);

            var matrix322 = new[, ,] {
                { { new Node { Value = 1 }, new Node { Value = 2 } }, { new Node { Value = 3 }, new Node { Value = 4 } } },
                { { new Node { Value = 5 }, new Node { Value = 6 } }, { new Node { Value = 7 }, new Node { Value = 8 } } },
                { { new Node { Value = 9 }, new Node { Value = 10 } }, { new Node { Value = 11 }, new Node { Value = 12 } } }
            };
            var cloned322 = ObjectCloner.Clone(matrix322);
            cloned322.IsNotSameReferenceAs(matrix322);
            cloned322.IsStructuralEqual(matrix322);
            TraverseMatrix(matrix322, cloned322);

            var copied232 = new Node[3, 2, 2];
            ObjectCloner.CopyTo(matrix322, copied232);
            copied232.IsNotSameReferenceAs(matrix322);
            copied232.IsStructuralEqual(matrix322);
            TraverseMatrix(matrix322, cloned322);

            _ = ValidateCloneMatrixArray(matrix322);

            var matrix2322 = new[, , ,] {
                {
                    { { "1", "2" }, { "3", "4" } },
                    { { "5", "6" }, { "7", "8" } },
                    { { "9", "10" }, { "11", "12" } },
                },
                {
                    { { "13", "14" }, { "15", "16" } },
                    { { "17", "18" }, { "19", "20" } },
                    { { "21", "22" }, { "23", "24" } }
                }
            };
            var cloned2232 = ObjectCloner.Clone(matrix2322);
            cloned2232.IsNotSameReferenceAs(matrix2322);
            cloned2232.IsStructuralEqual(matrix2322);
            TraverseMatrix(matrix2322, cloned2232);

            var copied2322 = new string[2, 3, 2, 2];
            ObjectCloner.CopyTo(matrix2322, copied2322);
            copied2322.IsNotSameReferenceAs(matrix2322);
            copied2322.IsStructuralEqual(matrix2322);
            TraverseMatrix(matrix2322, copied2322);

            _ = ValidateCloneMatrixArray(matrix2322);

            var matrix22322 = new[, , , ,] {
                {
                    {
                        { { Guid.NewGuid(), Guid.NewGuid() }, { Guid.NewGuid(), Guid.NewGuid() } },
                        { { Guid.NewGuid(), Guid.NewGuid() }, { Guid.NewGuid(), Guid.NewGuid() } },
                        { { Guid.NewGuid(), Guid.NewGuid() }, { Guid.NewGuid(), Guid.NewGuid() } },
                    },
                },
                {
                    {
                        { { Guid.NewGuid(), Guid.NewGuid() }, { Guid.NewGuid(), Guid.NewGuid() } },
                        { { Guid.NewGuid(), Guid.NewGuid() }, { Guid.NewGuid(), Guid.NewGuid() } },
                        { { Guid.NewGuid(), Guid.NewGuid() }, { Guid.NewGuid(), Guid.NewGuid() } }
                    },
                }
            };
            var cloned22322 = ObjectCloner.Clone(matrix22322);
            cloned22322.IsNotSameReferenceAs(matrix22322);
            cloned22322.IsStructuralEqual(matrix22322);
            TraverseMatrix(matrix22322, cloned22322);

            var copied22322 = new Guid[2, 1, 3, 2, 2];
            ObjectCloner.CopyTo(matrix22322, copied22322);
            copied22322.IsNotSameReferenceAs(matrix22322);
            copied22322.IsStructuralEqual(matrix22322);
            TraverseMatrix(matrix22322, copied22322);

            _ = ValidateCloneMatrixArray(matrix22322);
        }

        [Fact]
        private void ObjectWithMultiDimArrayTest()
        {
            var obj = new
            {
                Jagged2 = new[] { new[] { 1 }, new[] { 3, 4 } },
                ObjectCastedJagged2 = (object)(new[] { new[] { 1 }, new[] { 3, 4 } }),
                CastedJagged2 = (Array)(new[] { new[] { 1 }, new[] { 3, 4 } }),
                NullableJagged2 = new[] { new int?[] { 1 }, new int?[] { 3, 4, null }, null },
                CastedNullableJagged2 = (Array)(new[] { new int?[] { 1 }, new int?[] { 3, 4, null }, null }),
                Jagged3 = new[] { new[] { new[] { new Node { Value = 1 } } }, new[] { new[] { new Node { Value = 2 }, new Node { Value = 3 } } } },
                ObjectCastedJagged3 = (object)(new[] { new[] { new[] { new Node { Value = 1 } } }, new[] { new[] { new Node { Value = 2 }, new Node { Value = 3 } } } }),
                CastedJagged3 = (Array)(new[] { new[] { new[] { new Node { Value = 1 } } }, new[] { new[] { new Node { Value = 2 }, new Node { Value = 3 } } } }),
                NullableJagged3 = new[] { new[] { new Node[] { new Node { Value = 1 } } }, new[] { new Node[] { new Node { Value = 2 }, new Node { Value = 3 }, null }, null }, null },
                CastedNullableJagged3 = (Array)(new[] { new[] { new Node[] { new Node { Value = 1 } } }, new[] { new Node[] { new Node { Value = 2 }, new Node { Value = 3 }, null }, null }, null }),
                Jagged5 = new[] { new[] { new[] { new[] { new[] { Guid.NewGuid() } } } }, new[] { new[] { new[] { new[] { Guid.NewGuid(), Guid.NewGuid() } } } } },
                ObjectCastedJagged5 = (object)(new[] { new[] { new[] { new[] { new[] { Guid.NewGuid() } } } }, new[] { new[] { new[] { new[] { Guid.NewGuid(), Guid.NewGuid() } } } } }),
                CastedJagged5 = (Array)(new[] { new[] { new[] { new[] { new[] { Guid.NewGuid() } } } }, new[] { new[] { new[] { new[] { Guid.NewGuid(), Guid.NewGuid() } } } } }),
                NullableJagged5 = new[] { new[] { new[] { new[] { new Guid?[] { Guid.NewGuid() } } } }, new[] { new[] { new[] { new Guid?[] { Guid.NewGuid(), Guid.NewGuid(), null }, null }, null }, null } },
                CastedNullableJagged5 = (Array)(new[] { new[] { new[] { new[] { new Guid?[] { Guid.NewGuid() } } } }, new[] { new[] { new[] { new Guid?[] { Guid.NewGuid(), Guid.NewGuid(), null }, null }, null }, null } }),
                Matrix23 = new[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                ObjectCastedMatrix23 = (object)new[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                CastedMatrix23 = (Array)new[,] { { 1, 2, 3 }, { 4, 5, 6 } },
                NullableMatrix23 = new int?[,] { { 1, null, 3 }, { 4, 5, 6 } },
                CastedNullableMatrix23 = (Array)new int?[,] { { 1, null, 3 }, { 4, 5, 6 } },
                Matrix322 = new[, ,] {
                    { { new Node { Value = 1 }, new Node { Value = 2 } }, { new Node { Value = 3 }, new Node { Value = 4 } } },
                    { { new Node { Value = 5 }, new Node { Value = 6 } }, { new Node { Value = 7 }, new Node { Value = 8 } } },
                    { { new Node { Value = 9 }, new Node { Value = 10 } }, { new Node { Value = 11 }, new Node { Value = 12 } } }
                },
                ObjectCastedMatrix322 = (object)new[, ,] {
                    { { new Node { Value = 1 }, new Node { Value = 2 } }, { new Node { Value = 3 }, new Node { Value = 4 } } },
                    { { new Node { Value = 5 }, new Node { Value = 6 } }, { new Node { Value = 7 }, new Node { Value = 8 } } },
                    { { new Node { Value = 9 }, new Node { Value = 10 } }, { new Node { Value = 11 }, new Node { Value = 12 } } }
                },
                CastedMatrix322 = (Array)new[, ,] {
                    { { new Node { Value = 1 }, new Node { Value = 2 } }, { new Node { Value = 3 }, new Node { Value = 4 } } },
                    { { new Node { Value = 5 }, new Node { Value = 6 } }, { new Node { Value = 7 }, new Node { Value = 8 } } },
                    { { new Node { Value = 9 }, new Node { Value = 10 } }, { new Node { Value = 11 }, new Node { Value = 12 } } }
                },
                NullableMatrix322 = new[, ,] {
                    { { new Node { Value = 1 }, null }, { new Node { Value = 3 }, new Node { Value = 4 } } },
                    { { new Node { Value = 5 }, new Node { Value = 6 } }, { new Node { Value = 7 }, new Node { Value = 8 } } },
                    { { new Node { Value = 9 }, new Node { Value = 10 } }, { new Node { Value = 11 }, null } }
                },
                CastedNullableMatrix322 = (Array)new[, ,] {
                    { { new Node { Value = 1 }, null }, { new Node { Value = 3 }, new Node { Value = 4 } } },
                    { { new Node { Value = 5 }, new Node { Value = 6 } }, { new Node { Value = 7 }, new Node { Value = 8 } } },
                    { { new Node { Value = 9 }, new Node { Value = 10 } }, { new Node { Value = 11 }, null } }
                },
            };

            var cloned = ValidateCloneObject(obj);
            cloned.IsNotSameReferenceAs(obj);
            cloned.IsStructuralEqual(obj);

            TraverseJagged(obj.Jagged2, cloned.Jagged2);
            TraverseJagged((Array)obj.ObjectCastedJagged2, (Array)cloned.ObjectCastedJagged2);
            TraverseJagged(obj.CastedJagged2, cloned.CastedJagged2);
            TraverseJagged(obj.NullableJagged2, cloned.NullableJagged2);
            TraverseJagged(obj.CastedNullableJagged2, cloned.CastedNullableJagged2);

            TraverseJagged(obj.Jagged3, cloned.Jagged3);
            TraverseJagged((Array)obj.ObjectCastedJagged3, (Array)cloned.ObjectCastedJagged3);
            TraverseJagged(obj.CastedJagged3, cloned.CastedJagged3);
            TraverseJagged(obj.NullableJagged3, cloned.NullableJagged3);
            TraverseJagged(obj.CastedNullableJagged3, cloned.CastedNullableJagged3);

            TraverseJagged(obj.Jagged5, cloned.Jagged5);
            TraverseJagged((Array)obj.ObjectCastedJagged5, (Array)cloned.ObjectCastedJagged5);
            TraverseJagged(obj.CastedJagged5, cloned.CastedJagged5);
            TraverseJagged(obj.NullableJagged5, cloned.NullableJagged5);
            TraverseJagged(obj.CastedNullableJagged5, cloned.CastedNullableJagged5);

            TraverseMatrix(obj.Matrix23, cloned.Matrix23);
            TraverseMatrix((Array)obj.ObjectCastedMatrix23, (Array)cloned.ObjectCastedMatrix23);
            TraverseMatrix(obj.CastedMatrix23, cloned.CastedMatrix23);
            TraverseMatrix(obj.NullableMatrix23, cloned.NullableMatrix23);
            TraverseMatrix(obj.CastedNullableMatrix23, cloned.CastedNullableMatrix23);

            TraverseMatrix(obj.Matrix322, cloned.Matrix322);
            TraverseMatrix((Array)obj.ObjectCastedMatrix322, (Array)cloned.ObjectCastedMatrix322);
            TraverseMatrix(obj.CastedMatrix322, cloned.CastedMatrix322);
            TraverseMatrix(obj.NullableMatrix322, cloned.NullableMatrix322);
            TraverseMatrix(obj.CastedNullableMatrix322, cloned.CastedNullableMatrix322);

#if NET10_0_OR_GREATER
            var copied = cloned with
            {
                Jagged2 = [],
                CastedJagged2 = (Array)(Array.Empty<int[]>()),
                ObjectCastedJagged2 = (object)(Array.Empty<int[]>()),
                NullableJagged2 = [],
                CastedNullableJagged2 = (Array)(Array.Empty<int?[]>()),
                Jagged3 = [],
                ObjectCastedJagged3 = (object)(Array.Empty<Node[][]>()),
                CastedJagged3 = (Array)(new Node[0][][]),
                NullableJagged3 = [],
                CastedNullableJagged3 = (Array)(Array.Empty<Node?[][]>()),
                Jagged5 = [],
                ObjectCastedJagged5 = (object)(Array.Empty<Guid[][][][]>()),
                CastedJagged5 = (Array)(new Guid[0][][][][]),
                NullableJagged5 = [],
                CastedNullableJagged5 = (Array)(Array.Empty<Guid?[][][]>()),
                Matrix23 = new int[0, 0],
                ObjectCastedMatrix23 = (object)new int[0, 0],
                CastedMatrix23 = (Array)new int[0, 0],
                NullableMatrix23 = new int?[0, 0],
                CastedNullableMatrix23 = (Array)new int?[0, 0],
                Matrix322 = new Node[0, 0, 0],
                ObjectCastedMatrix322 = (object)new Node[0, 0, 0],
                CastedMatrix322 = (Array)new Node[0, 0, 0],
                NullableMatrix322 = new Node?[0, 0, 0],
                CastedNullableMatrix322 = (Array)new Node?[0, 0, 0]
            };

            ObjectCloner.CopyTo(obj, copied);
            copied.IsNotSameReferenceAs(obj);
            copied.IsStructuralEqual(obj);

            TraverseJagged(obj.Jagged2, copied.Jagged2);
            TraverseJagged((Array)obj.ObjectCastedJagged2, (Array)copied.ObjectCastedJagged2);
            TraverseJagged(obj.CastedJagged2, copied.CastedJagged2);
            TraverseJagged(obj.NullableJagged2, copied.NullableJagged2);
            TraverseJagged(obj.CastedNullableJagged2, copied.CastedNullableJagged2);

            TraverseJagged(obj.Jagged3, copied.Jagged3);
            TraverseJagged((Array)obj.ObjectCastedJagged3, (Array)copied.ObjectCastedJagged3);
            TraverseJagged(obj.CastedJagged3, copied.CastedJagged3);
            TraverseJagged(obj.NullableJagged3, copied.NullableJagged3);
            TraverseJagged(obj.CastedNullableJagged3, copied.CastedNullableJagged3);

            TraverseJagged(obj.Jagged5, copied.Jagged5);
            TraverseJagged((Array)obj.ObjectCastedJagged5, (Array)copied.ObjectCastedJagged5);
            TraverseJagged(obj.CastedJagged5, copied.CastedJagged5);
            TraverseJagged(obj.NullableJagged5, copied.NullableJagged5);
            TraverseJagged(obj.CastedNullableJagged5, copied.CastedNullableJagged5);

            TraverseMatrix(obj.Matrix23, copied.Matrix23);
            TraverseMatrix((Array)obj.ObjectCastedMatrix23, (Array)copied.ObjectCastedMatrix23);
            TraverseMatrix(obj.CastedMatrix23, copied.CastedMatrix23);
            TraverseMatrix(obj.NullableMatrix23, copied.NullableMatrix23);
            TraverseMatrix(obj.CastedNullableMatrix23, copied.CastedNullableMatrix23);

            TraverseMatrix(obj.Matrix322, copied.Matrix322);
            TraverseMatrix((Array)obj.ObjectCastedMatrix322, (Array)copied.ObjectCastedMatrix322);
            TraverseMatrix(obj.CastedMatrix322, copied.CastedMatrix322);
            TraverseMatrix(obj.NullableMatrix322, copied.NullableMatrix322);
            TraverseMatrix(obj.CastedNullableMatrix322, copied.CastedNullableMatrix322);
#endif

        }

        [Fact]
        private void ArrayExceptionTest()
        {
            int[] array = new int[] { 1, 2, 3 };

            var copied = new int[] { 3, 4 };
            Assert.Throws<ArgumentException>(() => ObjectCloner.CopyTo(array, copied));

            var copied2 = new int[,] { { 1, 2 }, { 3, 4 } };
            Assert.Throws<RankException>(() => ObjectCloner.CopyTo(array, copied2));
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
            var cloned1 = ObjectCloner.Clone(array);
            cloned1.IsNotSameReferenceAs(array);
            cloned1.IsStructuralEqual(array);

            for (int i = 0; i < array.Length; i++)
            {
                if (!(array[i]?.GetType().IsValueType ?? true) && array[i]?.GetType() != typeof(string))
                    cloned1[i].IsNotSameReferenceAs(array[i]);
            }

            var cloned2 = ObjectCloner.Clone(array, true);
            cloned2.IsNotSameReferenceAs(array);
            cloned2.IsStructuralEqual(array);

            for (int i = 0; i < array.Length; i++)
            {
                if (!(array[i]?.GetType().IsValueType ?? true) && array[i]?.GetType() != typeof(string))
                    cloned2[i].IsNotSameReferenceAs(array[i]);
            }

            T[] cloned3 = new T[array.Length];
            ObjectCloner.CopyTo(array, cloned3);
            cloned3.IsNotSameReferenceAs(array);
            cloned3.IsStructuralEqual(array);

            for (int i = 0; i < array.Length; i++)
            {
                if (!(array[i]?.GetType().IsValueType ?? true) && array[i]?.GetType() != typeof(string))
                    cloned3[i].IsNotSameReferenceAs(array[i]);
            }

            return cloned1;
        }

        //private T[][] ValidateCloneJaggedArray<T>(in T[][] array)
        //{
        //    var cloned1 = ObjectCloner.Clone(array);
        //    cloned1.IsNotSameReferenceAs(array);
        //    cloned1.IsStructuralEqual(array);

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        for (int j = 0; j < array[i].Length; j++)
        //        {
        //            if (!(array[i][j]?.GetType().IsValueType ?? true) && array[i][j]?.GetType() != typeof(string))
        //                cloned1[i][j].IsNotSameReferenceAs(array[i][j]);
        //        }
        //    }

        //    var cloned2 = ObjectCloner.Clone(array, true);
        //    cloned2.IsNotSameReferenceAs(array);
        //    cloned2.IsStructuralEqual(array);

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        for (int j = 0; j < array[i].Length; j++)
        //        {
        //            if (!(array[i][j]?.GetType().IsValueType ?? true) && array[i][j]?.GetType() != typeof(string))
        //                cloned2[i][j].IsNotSameReferenceAs(array[i][j]);
        //        }
        //    }

        //    return cloned1;
        //}

        private Array ValidateCloneJaggedArray(in Array array)
        {
            var cloned1 = ObjectCloner.Clone(array);
            cloned1.IsNotSameReferenceAs(array);
            cloned1.IsStructuralEqual(array);

            TraverseJagged(array, cloned1);

            var cloned2 = ObjectCloner.Clone(array, true);
            cloned2.IsNotSameReferenceAs(array);
            cloned2.IsStructuralEqual(array);

            TraverseJagged(array, cloned2);

            var cloned3 = (Array)array.Clone();
            ObjectCloner.CopyTo(array, cloned3);
            cloned3.IsNotSameReferenceAs(array);
            cloned3.IsStructuralEqual(array);

            TraverseJagged(array, cloned3);

            return cloned1;
        }

        private Array ValidateCloneMatrixArray(in Array array)
        {
            var cloned1 = ObjectCloner.Clone(array);
            cloned1.IsNotSameReferenceAs(array);
            cloned1.IsStructuralEqual(array);

            TraverseMatrix(array, cloned1);

            var cloned2 = ObjectCloner.Clone(array, true);
            cloned2.IsNotSameReferenceAs(array);
            cloned2.IsStructuralEqual(array);

            TraverseMatrix(array, cloned2);

            var cloned3 = (Array)array.Clone();
            ObjectCloner.CopyTo(array, cloned3);
            cloned3.IsNotSameReferenceAs(array);
            cloned3.IsStructuralEqual(array);

            TraverseMatrix(array, cloned3);

            return cloned1;
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

        private static void TraverseJagged(Array source, Array cloned)
        {
            cloned.IsNotSameReferenceAs(source);
            TraverseCore(source, cloned, new List<int>());

#if NET8_0_OR_GREATER
            void TraverseCore(object? current, object? clonedCurrent, List<int> indices)
#else
            void TraverseCore(object current, object clonedCurrent, List<int> indices)
#endif
            {
                clonedCurrent?.IsStructuralEqual(current);
                if (current?.GetType() != typeof(string))
                    clonedCurrent?.IsNotSameReferenceAs(current);

                if (current is Array currentArray &&
                    clonedCurrent is Array clonedArray)
                {
                    for (int i = 0; i < currentArray.Length; i++)
                    {
                        indices.Add(i);
                        TraverseCore(currentArray.GetValue(i), clonedArray.GetValue(i), indices);
                    }

                    return;
                }
            }
        }

        private static void TraverseMatrix(Array source, Array cloned)
        {
            var rank = source.Rank;
            int[] indices = new int[rank];
            var lengthes = Enumerable.Range(0, rank).Select(i => source.GetLength(i)).ToArray();

            while (true)
            {
                var sourceValue = source.GetValue(indices);
                var clonedValue = cloned.GetValue(indices);

                clonedValue?.IsStructuralEqual(sourceValue);
                if (!(sourceValue?.GetType().IsValueType ?? true) && sourceValue?.GetType() != typeof(string))
                    clonedValue.IsNotSameReferenceAs(sourceValue);

                int dim = rank - 1;

                while (dim >= 0)
                {
                    indices[dim]++;
                    if (indices[dim] < source.GetLength(dim))
                        break;

                    indices[dim] = 0;
                    dim--;
                }

                if (dim < 0)
                    break;
            }
        }
    }
}
