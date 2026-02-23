using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DeepCopy.Test.Inners
{
    internal class TypeUtils
    {
#if NET8_0_OR_GREATER
        public static bool IsUnmanagedType<T>() => typeof(T) switch
        {
            Type t when t == typeof(string) || t == typeof(decimal) => true,
            _ => !RuntimeHelpers.IsReferenceOrContainsReferences<T>()
        };

        public static bool IsUnmanagedType(Type t) => t switch
        {
            _ when t == typeof(string) || t == typeof(decimal) => true,
            _ when t.IsPrimitive || t.IsEnum => true,
            _ => IsAssignableType(t, [t])
        };
#else
        public static bool IsUnmanagedType<T>() => IsUnmanagedType(typeof(T));

        public static bool IsUnmanagedType(Type type)
        {
            return type == typeof(string) || type == typeof(decimal)
                || type.IsPrimitive || type.IsEnum
                || IsAssignableType(type, new HashSet<Type> { type });
        }
#endif

        private static bool IsEvent(Type type, string fieldName) =>
            type.GetEvent(fieldName) != null;

        private static bool IsAssignableType(Type type, HashSet<Type> cache) =>
            type != null
            && (
                type.IsPrimitive || type.IsEnum || type == typeof(decimal) || type == typeof(string)
                || IsFullyAssignableType(type, cache)
            );

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


        private static bool IsFullyAssignableType(Type type, HashSet<Type> cache) =>
            type.IsValueType &&
            GetFields(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(field => field.FieldType)
            .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(prop => prop.PropertyType))
            .All(x => !cache.Add(x) || IsAssignableType(x, cache));
    }
}
