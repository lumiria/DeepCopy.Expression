using System;
using System.Reflection;

namespace DeepCopy.Internal.Utilities
{
    internal static class ReflectionUtils
    {
        public static MethodInfo MemberwizeClone { get; } =
            typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        public static MethodInfo SetValue { get; } =
            typeof(FieldInfo).GetMethod("SetValue", [typeof(object), typeof(object)]);

        public static MethodInfo CloneArray { get; } =
            typeof(Array).GetMethod("Clone");

        public static MethodInfo ArrayCopy { get; } =
            typeof(Array).GetMethod("Copy", [typeof(Array), typeof(Array), typeof(int)]);

        public static MethodInfo GetArrayLength { get; } =
            typeof(Array).GetMethod("GetLength");

        public static MethodInfo GetObjectType { get; } =
            typeof(object).GetMethod("GetType");

        public static MethodInfo IsObjectOrValueType { get; } =
            typeof(TypeUtils).GetMethod("IsObjectOrValueType", BindingFlags.Static | BindingFlags.Public);

        public static MethodInfo IsValueType { get; } =
            typeof(TypeUtils).GetMethod("IsValueType", BindingFlags.Static | BindingFlags.Public);

        public static PropertyInfo IsArray { get; } =
            typeof(Type).GetProperty(nameof(Type.IsArray));

        public static MethodInfo ObjectClone { get; } =
            typeof(ObjectCloner).GetMethod("_Clone", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo CloneAs { get; } =
            typeof(ObjectCloner).GetMethod("_CloneAs", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo CloneLeaf { get; } =
            typeof(ObjectCloner).GetMethod("_CloneLeaf", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo ValueClone { get;  } =
            typeof(ObjectCloner).GetMethod("_CloneValue", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo NullableValueClone { get;  } =
            typeof(ObjectCloner).GetMethod("_CloneNullableValue", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo InterfaceClone { get;  } =
            typeof(ObjectCloner).GetMethod("_CloneInterface", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo ObjectTypeClone { get; } =
            typeof(ObjectCloner).GetMethod("_CloneObject", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo ArrayClone { get; } =
            typeof(ObjectCloner).GetMethod("_CloneArray", BindingFlags.Static | BindingFlags.NonPublic);
    }
}
