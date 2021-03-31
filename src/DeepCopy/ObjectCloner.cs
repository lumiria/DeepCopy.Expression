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
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        /// <returns>A new object that is copy of the specified object.</returns>
        public static T Clone<T>(T source, bool preserveObjectReferences=false)
        {
            if (source == null) return default;

            var instance = (T)FormatterServices.GetUninitializedObject(
                source.GetType());
            _CopyTo(source, instance,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());

            return instance;
        }

        /// <summary>
        /// Copies the object to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">A source object.</param>
        /// <param name="destination">The object that is desitination of the copy.</param>
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        public static void CopyTo<T>(T source, T destination, bool preserveObjectReferences = false)
        {
            _CopyTo(source, destination,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
        }

        /// <summary>
        /// Creates a new array thas is copy of the specified array.
        /// </summary>
        /// <typeparam name="T">The element type of array.</typeparam>
        /// <param name="source">A source array.</param>
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        /// <returns>A new array that is copy of the specified array.</returns>
        public static T[] Clone<T>(T[] source, bool preserveObjectReferences = false)
        {
            var cloner = CloneArrayExpressionGenerator<T, T[]>.CreateDelegate();
            return cloner(source,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
        }

        /// <summary>
        /// Creates a new array thas is copy of the specified array.
        /// </summary>
        /// <typeparam name="T">The element type of array.</typeparam>
        /// <param name="source">A source array.</param>
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        /// <returns>A new array that is copy of the specified array.</returns>
        public static T[,] Clone<T>(T[,] source, bool preserveObjectReferences = false)
        {
            var cloner = CloneArrayExpressionGenerator<T, T[,]>.CreateDelegate();
            return cloner(source,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
        }

        /// <summary>
        /// Creates a new array thas is copy of the specified array.
        /// </summary>
        /// <typeparam name="T">The element type of array.</typeparam>
        /// <param name="source">A source array.</param>
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        /// <returns>A new array that is copy of the specified array.</returns>
        public static T[,,] Clone<T>(T[,,] source, bool preserveObjectReferences = false)
        {
            var cloner = CloneArrayExpressionGenerator<T, T[,,]>.CreateDelegate();
            return cloner(source,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
        }

        /// <summary>
        /// Creates a new array thas is copy of the specified array.
        /// </summary>
        /// <typeparam name="T">The element type of array.</typeparam>
        /// <param name="source">A source array.</param>
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        /// <returns>A new array that is copy of the specified array.</returns>
        public static T[,,,] Clone<T>(T[,,,] source, bool preserveObjectReferences = false)
        {
            var cloner = CloneArrayExpressionGenerator<T, T[,,,]>.CreateDelegate();
            return cloner(source,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
        }

        /// <summary>
        /// Creates a new array thas is copy of the specified array.
        /// </summary>
        /// <typeparam name="T">The element type of array.</typeparam>
        /// <param name="source">A source array.</param>
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        /// <returns>A new array that is copy of the specified array.</returns>
        public static T[,,,,] Clone<T>(T[,,,,] source, bool preserveObjectReferences = false)
        {
            var cloner = CloneArrayExpressionGenerator<T, T[,,,,]>.CreateDelegate();
            return cloner(source,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
        }

        
        public static void Cleanup<T>()
        {
            CloneExpressionGenerator<T>.Clearnup();
            CloneArrayExpressionGenerator<T, T[]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,,]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,,,]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,,,,]>.Cleanup();
        }

        private static T _Clone<T>(T source, ObjectReferencesCache cache)
        {
            if (source == null) return default;

            if (cache.Get(source, out var obj)) return obj;

            var instance = (T)FormatterServices.GetUninitializedObject(
                source.GetType());
            cache.Add(source, instance);

            _CopyTo(source, instance, cache);

            return instance;
        }

        private static void _CopyTo<T>(T source, T destination, ObjectReferencesCache cache)
        {
            if (source.GetType() == typeof(T))
            {
                var cloner = CloneExpressionGenerator<T>.CreateDelegate();
                cloner(source, destination, cache);
            }
            else
            {
                var cloner = CloneExpressionGenerator.CreateDelegate(source.GetType());
                cloner(source, destination, cache);
            }
        }
    }
}
