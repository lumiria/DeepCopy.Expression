using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace DeepCopy.Test
{
    internal static class AssertionExtensions
    {
        public static void IsSameReferenceAs<T>(this T actual, T expected)
        {
            if (IsUnmanagedType<T>()) return;

#pragma warning disable xUnit2005 // Do not use identity check on value type
            Assert.Same(expected, actual);
#pragma warning restore xUnit2005 // Do not use identity check on value type
        }

        public static void IsNotSameReferenceAs<T>(this T actual, T expected)
        {
            if (IsUnmanagedType<T>()) return;

#pragma warning disable xUnit2005 // Do not use identity check on value type
            Assert.NotSame(expected, actual);
#pragma warning restore xUnit2005 // Do not use identity check on value type
        }

        public static void Is<T>(this T actual, T expected)
        {
            Assert.Equal(expected, actual);
        }

        public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.Equal(expected, actual);
        }

        public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            Assert.NotEqual(expected, actual);
        }


        public static void IsNull<T>(this T @object)
        {
            Assert.Null(@object);
        }

        public static void IsTrue(this bool condition)
        {
            Assert.True(condition);
        }

        public static void IsStructuralEqual<T>(this T actual, T expected)
        {
            string? actualJson = null;
            string? expectedJson = null;

            try
            {
                actualJson = JsonSerializer.Serialize(actual);
                expectedJson = JsonSerializer.Serialize(expected);

            }
            catch { };

            if (actualJson != null || expectedJson != null)
            {
                if (actualJson != expectedJson)
                {
                    Assert.Fail($"IsStructuralEqual failed.\nExpected: {expectedJson}\nActual  : {actualJson}");
                }
                return;
            }

            var type = typeof(T);
            var fields = GetFields(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType.IsPrimitive || field.FieldType.IsEnum || field.FieldType == typeof(decimal) || field.FieldType == typeof(string))
                {
                    Assert.Equal(field.GetValue(expected), field.GetValue(actual));
                    return;
                }

                IsStructuralEqual(field.GetValue(expected), field.GetValue(actual));
            }
        }

        private static bool IsUnmanagedType<T>() => typeof(T) switch
        {
            Type t when t == typeof(string) || t == typeof(decimal) => true,
            _ => !RuntimeHelpers.IsReferenceOrContainsReferences<T>()
        };

        private static IEnumerable<FieldInfo> GetFields(Type type, BindingFlags bindingFlags)
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

        private static bool IsEvent(Type type, string fieldName) =>
            type.GetEvent(fieldName) != null;
    }
}
