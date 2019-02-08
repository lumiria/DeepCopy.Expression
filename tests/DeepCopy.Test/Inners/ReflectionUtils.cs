using System.Reflection;

namespace DeepCopy.Test.Inners
{
    internal static class ReflectionUtils
    {
        public static void SetReadonlyField<T, TVaue>(T obj, string fieldName, TVaue value)
        {
            var field = typeof(T).GetField(
                fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(obj, value);
        }
    }
}
