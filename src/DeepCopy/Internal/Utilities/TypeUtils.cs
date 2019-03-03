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

        public static IEnumerable<FieldInfo> GetFields(Type type, BindingFlags bindingFlags)
        {
            var fields = new List<IEnumerable<FieldInfo>>();

            var baseType = type.BaseType;
            while (baseType != null && !baseType.IsInterface)
            {
                fields.Add(GetFields(baseType, bindingFlags));

                baseType = baseType.BaseType;
            }

            fields.Add(type.GetFields(bindingFlags));
            return fields.SelectMany(x => x);
        }
    }
}
