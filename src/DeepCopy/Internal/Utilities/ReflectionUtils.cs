using System;
using System.Reflection;

namespace DeepCopy.Internal.Utilities
{
    internal static class ReflectionUtils
    {
        public static MethodInfo MemberwizeClone { get; } =
            typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        public static MethodInfo SetValue { get; } =
            typeof(FieldInfo).GetMethod("SetValue", new[] { typeof(object), typeof(object) });

        public static MethodInfo CloneArray { get; } =
            typeof(Array).GetMethod("Clone");

        public static MethodInfo ArrayCopy { get; } =
            typeof(Array).GetMethod("Copy", new[] { typeof(Array), typeof(Array), typeof(int) });

        public static MethodInfo GetArrayLength { get; } =
            typeof(Array).GetMethod("GetLength");

        public static MethodInfo ObjectClone { get; } =
            typeof(ObjectCloner).GetMethod("_Clone", BindingFlags.Static | BindingFlags.NonPublic);

        public static MethodInfo ArrayClone { get; } =
            typeof(ObjectCloner).GetMethod("_CloneArray", BindingFlags.Static | BindingFlags.NonPublic);
    }
}
