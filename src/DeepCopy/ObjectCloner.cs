using System;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static T Clone<T>(T source, bool preserveObjectReferences = false)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            var type = source.GetType();
#if NETSTANDARD2_0
            var instance = (T)FormatterServices.GetUninitializedObject(type);
#else
            var instance = (T)RuntimeHelpers.GetUninitializedObject(type);
#endif

            if (type.IsValueType)
            {
                _CopyValueType(type, source, ref instance,
                    CreateObjectReferenceCache(preserveObjectReferences));
            }
            else
            {
                _CopyTo(type, source, instance,
                    CreateObjectReferenceCache(preserveObjectReferences, source, instance));
            }

            return instance;
        }



        /// <summary>
        /// Returns a deep copy of the nullable specified value types object.
        /// </summary>
        /// <typeparam name="T">The type of the object to be copied.</typeparam>
        /// <param name="source">The object to be copied.</param>
        /// <param name="preserveObjectReferences">A flag indicating whether to preserve object references.</param>
        /// <returns>A deep copy of the specified object.</returns>
        public static T? Clone<T>(T? source, bool preserveObjectReferences = false)
            where T : struct
        {
            if (source == null) return default;

#if NETSTANDARD2_0
            var instance = (T?)FormatterServices.GetUninitializedObject(typeof(T?));
#else
            var instance = (T?)RuntimeHelpers.GetUninitializedObject(typeof(T?));
#endif

            _CopyNullableValueType(source, ref instance,
                CreateObjectReferenceCache(preserveObjectReferences));

            return instance;
        }

        /// <summary>
        /// Copies the object to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">A source object.</param>
        /// <param name="destination">The object that is destination of the copy.</param>
        /// <param name="preserveObjectReferences">A value that specifies whether to preserve object reference data.</param>
        public static void CopyTo<T>(T source, T destination, bool preserveObjectReferences = false)
        {
            _CopyTo(source.GetType(), source, destination,
                CreateObjectReferenceCache(preserveObjectReferences, source, destination));
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
            _CopyValueType(typeof(T), source, ref destination,
                    CreateObjectReferenceCache(preserveObjectReferences));
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
                CreateObjectReferenceCache(preserveObjectReferences));
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
                CreateObjectReferenceCache(preserveObjectReferences));
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
                CreateObjectReferenceCache(preserveObjectReferences));
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
                CreateObjectReferenceCache(preserveObjectReferences));
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
                CreateObjectReferenceCache(preserveObjectReferences));
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
                CreateObjectReferenceCache(preserveObjectReferences));
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

        /// <summary>
        /// Registers the builder for custom clone.
        /// </summary>
        /// <param name="targetType">The <see cref="Type"/> of the object to be custom cloned.</param>
        /// <param name="builder">The <see cref="CustomCloneBuilder"/> that builds a custom clone expression.</param>
        /// <remarks>
        /// If the builder is invalid, an <see cref="InvalidCloneBuilderException"/> will be thrown during clone execution.
        /// </remarks>
        public static void RegisterCustomClone(Type targetType, CustomCloneBuilder builder)
        {
            FixedCloner.Add(targetType, builder);
        }

        private static ObjectReferencesCache CreateObjectReferenceCache(bool preserveObjectReferences, object self = null, object cloned = null) =>
            ObjectReferencesCache.Create(self, cloned, preserveObjectReferences);

        #region Obsolete

        [Obsolete("This method is obsolete. Use Clone instead.", false)]
        public static T[] CloneArray<T>(T[] source, bool preserveObjectReferences = false) =>
            Clone(source, preserveObjectReferences);

        #endregion Obsolete


        private static T _Clone<T>(T source, ObjectReferencesCache cache)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            if (cache.Get(source, out var obj)) return obj;

            var type = source.GetType();
#if NETSTANDARD2_0
            var instance = (T)FormatterServices.GetUninitializedObject(type);
#else
            var instance = (T)RuntimeHelpers.GetUninitializedObject(type);
#endif

            cache.Add(source, instance);

            _CopyTo(type, source, instance, cache);

            cache.RemoveLatest();

            return instance;
        }

        private static T _CloneValue<T>(in T source, ObjectReferencesCache cache)
            where T : struct
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif

            var type = source.GetType();
#if NETSTANDARD2_0
            var instance = (T)FormatterServices.GetUninitializedObject(type);
#else
            var instance = (T)RuntimeHelpers.GetUninitializedObject(type);
#endif

            _CopyValueType(type, source, ref instance, cache);

            return instance;
        }

        private static T? _CloneNullableValue<T>(in T? source, ObjectReferencesCache cache)
            where T : struct
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            var type = source.GetType();
#if NETSTANDARD2_0
            var instance = (T?)FormatterServices.GetUninitializedObject(type);
#else
            var instance = (T?)RuntimeHelpers.GetUninitializedObject(type);
#endif

            _CopyNullableValueType(source, ref instance, cache);

            return instance;
        }

        private static T _CloneInterface<T>(T source, ObjectReferencesCache cache)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            var type = source.GetType();
            if (!type.IsValueType && cache.Get(source, out var obj)) return obj;

#if NETSTANDARD2_0
            var instance = (T)FormatterServices.GetUninitializedObject(type);
#else
            var instance = (T)RuntimeHelpers.GetUninitializedObject(type);
#endif

            if (type.IsValueType)
            {
                _CopyValueType(type, source, ref instance, cache);
            }
            else
            {
                cache.Add(source, instance);
                _CopyTo(type, source, instance, cache);
                cache.RemoveLatest();
            }

            return instance;
        }

        private static object _CloneObject(object source, ObjectReferencesCache cache)
        {
            if (source == null) return default;

            var type = source.GetType();
#if DEBUGLOG
            Console.WriteLine($"[{typeof(object).Name} >> {type.Name}]");
#endif

            if (!type.IsValueType && cache.Get(source, out var obj)) return obj;

            if (!type.IsArray)
            {
#if NETSTANDARD2_0
                var instance = FormatterServices.GetUninitializedObject(type);
#else
                var instance = RuntimeHelpers.GetUninitializedObject(type);
#endif

                if (type.IsValueType)
                {
                    _CopyValueType(type, source, ref instance, cache);
                }
                else
                {
                    cache.Add(source, instance);
                    _CopyTo(type, source, instance, cache);
                    cache.RemoveLatest();
                }

                return instance;
            }
            else
            {
                var castedArray = ((Array)source).Cast<object>().ToArray();

#if NET9_0_OR_GREATER
                var instance = Array.CreateInstanceFromArrayType(type.GetElementType(), castedArray.Length);
#else
                var instance = Array.CreateInstance(type.GetElementType(), castedArray.Length);
#endif
                cache.Add(source, instance);
                _CopyTo(castedArray, instance, cache);
                cache.RemoveLatest();

                return instance;
            }
        }

        private static void _CopyTo<T>(in Type type, T source, T destination, ObjectReferencesCache cache)
        {
            if (type == typeof(T))
            {
                var cloner = ReferenceTypeCloneDelegateGenerator<T>.CreateDelegate();
                cloner(source, destination, cache);
            }
            else
            {
                var cloner = ReferenceTypeCloneDelegateGenerator.CreateDelegate(type);
                cloner(source, (T)destination, cache);
            }
        }

        private static void _CopyValueType<T>(in Type type, in T source, ref T destination, ObjectReferencesCache cache)
        {
            if (type == typeof(T))
            {
                var cloner = ValueTypeCloneExpressionGenerator<T>.CreateDelegate();
                cloner(source, ref destination, cache);
            }
            else
            {
                var cloner = ValueTypeCloneDelegateGenerator.CreateDelegate(type);
                cloner(source, out var obj, cache);
                destination = (T)obj;
            }
        }

        private static void _CopyNullableValueType<T>(in T? source, ref T? destination, ObjectReferencesCache cache)
            where T : struct
        {
            var cloner = ValueTypeCloneExpressionGenerator<T?>.CreateDelegate();
            cloner(source, ref destination, cache);
        }

        private static void _CopyTo<T>(T[] source, Array destination, ObjectReferencesCache cache)
        {
            var cloner = CloneArrayExpressionGenerator<T, T[]>.CreateDelegate();
            var cloned = cloner(source, cache);
            Array.Copy(cloned, destination, destination.Length);
        }
    }
}
