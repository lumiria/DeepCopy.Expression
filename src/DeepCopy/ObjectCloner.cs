using System;
using System.Runtime.Serialization;
using DeepCopy.Internal;

namespace DeepCopy
{
    /// <summary>
    /// Deep copying object.
    /// </summary>
    public static class ObjectCloner
    {
        /// <summary>
        /// Precompiles copy processing dynamic code.
        /// </summary>
        /// <typeparam name="T">The type of target.</typeparam>
        public static void Compile<T>()
        {
            CloneExpressionGenerator<T>.CreateDelegate();
        }

        /// <summary>
        /// Precompiles copy processing dynamic code.
        /// </summary>
        /// <param name="type">The type of target.</param>
        /// <remarks>
        /// If you want to copy using a base class or interface, use this <see cref="Compile"/> method.
        /// Specifies a derived class in <paramref name="type"/>。
        /// </remarks>
        public static void Compile(Type type)
        {
            CloneExpressionGenerator.CreateDelegate(type);
        }

        /// <summary>
        /// Creates a new object thas is copy of the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">A source object.</param>
        /// <returns>A new object that is copy of the specified object.</returns>
        public static T Clone<T>(T source)
        {
            if (source == null) return default;

            var instance = (T)FormatterServices.GetUninitializedObject(
                source.GetType());
            CopyTo(source, instance);

            return instance;
        }

        /// <summary>
        /// Copies the object to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">A source object.</param>
        /// <param name="destination">The object that is desitination of the copy.</param>
        public static void CopyTo<T>(T source, T destination)
        {
            if (source.GetType() == typeof(T))
            {
                var cloner = CloneExpressionGenerator<T>.CreateDelegate();
                cloner(source, destination);
            }
            else
            {
                var cloner = CloneExpressionGenerator.CreateDelegate(source.GetType());
                cloner(source, destination);
            }
        }

        public static T[] CloneArray<T>(T[] source)
        {
            var cloner = CloneArrayExpressionGenerator<T>.CreateDelegate();
            return cloner(source);
        }

        
        public static void Cleanup<T>()
        {
            CloneExpressionGenerator<T>.Clearnup();
            CloneArrayExpressionGenerator<T>.Clearnup();
        }
    }
}
