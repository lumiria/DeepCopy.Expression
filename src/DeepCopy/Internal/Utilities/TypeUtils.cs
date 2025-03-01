using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeepCopy.Internal.Utilities
{
    internal static class TypeUtils
    {
        public static bool IsValueType(Type type) =>
            type.IsValueType || type == typeof(string);

        public static bool IsArrayType(Type type) =>
            type.IsArray;

        public static bool IsDelegate(Type type) =>
            type.IsSubclassOf(typeof(Delegate)) || type.Equals(typeof(Delegate));

        public static bool IsEvent(Type type, string fieldName) =>
            type.GetEvent(fieldName) != null;

        public static bool IsObjectOrValueType(Type type) =>
            type == typeof(object) || IsValueType(type);

        public static bool IsNullable(Type type) =>
             !IsValueType(type) || Nullable.GetUnderlyingType(type) != null;

        public static bool IsAssignableType(Type type) =>
            IsAssignableType(type, [typeof(Type)]);

        public static IEnumerable<FieldInfo> GetFields(Type type, BindingFlags bindingFlags)
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

        private static bool IsAssignableType(Type type, HashSet<Type> cache) =>
            type is not null
            && (
                type.IsPrimitive || type.IsEnum || type == typeof(decimal) || type == typeof(string)
                || IsFullyAssignableType(type, cache)
            );


        private static bool IsFullyAssignableType(Type type, HashSet<Type> cache) =>
            type.IsValueType &&
            GetFields(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(field => field.FieldType)
            .Concat(type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(prop => prop.PropertyType))
            .All(x => !cache.Add(x) || IsAssignableType(x, cache));
    }
}
