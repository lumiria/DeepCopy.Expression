﻿using System;
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
            ReferenceTypeCloneDelegateGenerator<T>.CreateDelegate();
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
            ReferenceTypeCloneDelegateGenerator.CreateDelegate(type);
        }

        /// <summary>
        /// Returns a deep copy of the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object to be copied.</typeparam>
        /// <param name="source">The object to be copied.</param>
        /// <param name="preserveObjectReferences">A flag indicating whether to preserve object references.</param>
        /// <returns>A deep copy of the specified object.</returns>
        public static T Clone<T>(T source, bool preserveObjectReferences=false)
        {
            if (source == null) return default;

            var instance = (T)FormatterServices.GetUninitializedObject(
                source.GetType());

            if (typeof(T).IsValueType)
            {
                _CopyValueType(source, ref instance,
                    preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
            }
            else
            {
                _CopyTo(source, instance,
                    preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
            }

            return instance;
        }


        /// <summary>
        /// Returns a deep copy of the nullable specified value types object.
        /// </summary>
        /// <typeparam name="T">The type of the oject to be copied.</typeparam>
        /// <param name="source">The object to be copied.</param>
        /// <param name="preserveObjectReferences">A flag indicating whether to preserve object references.</param>
        /// <returns>A deep copy of the specified object.</returns>
        public static T? Clone<T>(T? source, bool preserveObjectReferences = false)
            where T : struct
        {
            if (source == null) return default;

            var instance = (T?)FormatterServices.GetUninitializedObject(
                typeof(T?));

            _CopyNullableValueType(source, ref instance,
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
        /// Deep copies the source object the destination object.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="source">The object to copy.</param>
        /// <param name="destination">The object to copy to.</param>
        /// <param name="preserveObjectReferences">Whether to preserve object references.</param>
        public static void CopyTo<T>(T source, ref T destination, bool preserveObjectReferences = false)
            where T : struct
        {
            _CopyValueType(source, ref destination,
                preserveObjectReferences ? ObjectReferencesCache.Create() : ObjectReferencesCache.Empty());
        }

        /// <summary>
        /// Deep copies the source object the destination object.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="source">The object to copy.</param>
        /// <param name="destination">The object to copy to.</param>
        /// <param name="preserveObjectReferences">Whether to preserve object references.</param>
        public static void CopyTo<T>(T? source, ref T? destination, bool preserveObjectReferences = false)
            where T : struct
        {
            _CopyNullableValueType(source, ref destination,
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
            ReferenceTypeCloneDelegateGenerator<T>.Clearnup();
            ValueTypeCloneExpressionGenerator<T>.Clearnup();
            CloneArrayExpressionGenerator<T, T[]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,,]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,,,]>.Cleanup();
            CloneArrayExpressionGenerator<T, T[,,,,]>.Cleanup();
        }

        #region Obsolete

        [Obsolete("This method is obsolete. Use Clone instead.", false)]
        public static T[] CloneArray<T>(T[] source, bool preserveObjectReferences = false) =>
            Clone(source, preserveObjectReferences);

        #endregion Obsolete


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
                var cloner = ReferenceTypeCloneDelegateGenerator<T>.CreateDelegate();
                cloner(source, destination, cache);
            }
            else
            {
                var cloner = ReferenceTypeCloneDelegateGenerator.CreateDelegate(source.GetType());
                cloner(source, (T)destination, cache);
            }
        }

        private static void _CopyValueType<T>(T source, ref T destination, ObjectReferencesCache cache)
        {
            if (source.GetType() == typeof(T))
            {
                var cloner = ValueTypeCloneExpressionGenerator<T>.CreateDelegate();
                cloner(source, ref destination, cache);
            }
            else
            {
                var cloner = ReferenceTypeCloneDelegateGenerator.CreateDelegate(source.GetType());
                cloner(source, destination, cache);
            }
        }

        private static void _CopyNullableValueType<T>(T? source, ref T? destination, ObjectReferencesCache cache)
            where T : struct
        {
            var cloner = ValueTypeCloneExpressionGenerator<T?>.CreateDelegate();
            cloner(source, ref destination, cache);
        }
    }
}
