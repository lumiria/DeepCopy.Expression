using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DeepCopy.Internal;
using DeepCopy.Internal.BuiltIns;

namespace DeepCopy
{
    /// <summary>
    /// Deep copying object.
    /// </summary>
    public static partial class ObjectCloner
    {
        /// <summary>
        /// Precompiles copy processing dynamic code.
        /// </summary>
        /// <typeparam name="T">The type of target.</typeparam>
        public static void Compile<T>()
        {
            var type = typeof(T);
            if (Internal.Utilities.TypeUtils.IsUnmanagedType(type))
            {
                _ = ValueTypeCloneExpressionGenerator<T>.Delegate;
                return;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                FixedDictionaryCloner.Compile(type);
                return;
            }

            _ = ReferenceTypeCloneDelegateGenerator<T>.Delegate;

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
            if (Internal.Utilities.TypeUtils.IsUnmanagedType(type))
            {
                _ = ValueTypeCloneDelegateGenerator.CreateDelegate(type);
                return;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                FixedDictionaryCloner.Compile(type);
                return;
            }

            _ = ReferenceTypeCloneDelegateGenerator.CreateDelegate(type);

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
            Console.WriteLine($"[{typeof(T).Name}] | Source = [{source.GetType()}]");
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
                DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

                if (type == typeof(T))
                {
                    _CopyValueType(source, ref instance, context);
                }
                else
                {
                    _CopyValueType(type, source, ref instance, context);
                }

                context.Flush();
            }
            else
            {
                DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences, source, instance));

                _CopyTo(type, source, ref instance, context);

                context.Flush();
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

            DeepCopyContext context = new();

            _CopyNullableValueType(source, ref instance, context);

            context.Flush();

            return instance;
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
            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            return instance;
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
            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            return instance;
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
            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            return instance;
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
            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,,,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            return instance;
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
            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,,,,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

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
            if (source == null) return;

            ValidateCopyableType(source.GetType());

            var context = new DeepCopyContext(ObjectReferencesCache.Create(preserveObjectReferences, source, destination));

            _CopyTo(source.GetType(), source, ref destination, context);

            context.Flush();
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
            DeepCopyContext context = new();

            _CopyValueType(source, ref destination, context);

            context.Flush();
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
            DeepCopyContext context = new();

            _CopyNullableValueType(source, ref destination, context);

            context.Flush();
        }

        public static void CopyTo<T>(T[] source, T[] destination, bool preserveObjectReferences = false)
        {
            ValidateCopyableArray(source, destination);

            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences, source, destination));

            _CopyTo(source, destination, context);

            context.Flush();
        }

        public static void CopyTo<T>(T[,] source, T[,] destination, bool preserveObjectReferences = false)
        {
            ValidateCopyableArray(source, destination);

            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            Array.Copy(instance, destination, source.Length);
        }

        public static void CopyTo<T>(T[,,] source, T[,,] destination, bool preserveObjectReferences = false)
        {
            ValidateCopyableArray(source, destination);

            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            Array.Copy(instance, destination, source.Length);
        }

        public static void CopyTo<T>(T[,,,] source, T[,,,] destination, bool preserveObjectReferences = false)
        {
            ValidateCopyableArray(source, destination);

            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,,,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            Array.Copy(instance, destination, source.Length);
        }

        public static void CopyTo<T>(T[,,,,] source, T[,,,,] destination, bool preserveObjectReferences = false)
        {
            ValidateCopyableArray(source, destination);

            DeepCopyContext context = new(ObjectReferencesCache.Create(preserveObjectReferences));

            var cloner = CloneArrayExpressionGenerator<T, T[,,,,]>.Delegate;
            var instance = cloner(source, context);

            context.Flush();

            Array.Copy(instance, destination, source.Length);
        }

        public static void Cleanup<T>()
        {
            var type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                DictionaryCloneDelegateGenerator.Cleanup(type);
            }
            else if (Internal.Utilities.TypeUtils.IsUnmanagedType(type))
            {
                ValueTypeCloneExpressionGenerator<T>.Cleanup();
                ValueTypeCloneDelegateGenerator.Cleanup(type);
            }
            else
            {
                ReferenceTypeCloneDelegateGenerator<T>.Cleanup();
                ReferenceTypeCloneDelegateGenerator.Cleanup(type);
            }

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

        #region Obsolete

        [Obsolete("This method is obsolete. Use Clone instead.", false)]
        public static T[] CloneArray<T>(T[] source, bool preserveObjectReferences = false) =>
            Clone(source, preserveObjectReferences);

        #endregion Obsolete

        private static void ValidateCopyableType(Type type)
        {
            if (type.IsGenericType && DeepCopyOptions.NonCopyableGenericTypes.Contains(type.GetGenericTypeDefinition()))
            {
                throw new NotSupportedException("The type is not supported for copying.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateCopyableArray(Array source, Array destination)
        {
            if (source.Rank != destination.Rank)
            {
                throw new RankException("source and destination have different ranks.");
            }

            if (source.Length != destination.Length)
            {
                throw new ArgumentException("The lengths of source and description do not match.");
            }
        }

        internal static T _Clone<T>(T source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            var type = source.GetType();
            //if (context.Cache.TryGetOrCache(type, source, out T instance)) return instance;
            if (context.EnterScope(context, type, source, out T instance)) return instance;

            int queueCount = context.QueueCount;
            _CopyTo(type, source, ref instance, context);

            context.ExitScope(queueCount);

            return instance;
        }

        internal static T _CloneAs<T>(T source, DeepCopyContext context)

        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            //if (context.Cache.TryGetOrCache(typeof(T), source, out T instance)) return instance;
            if (context.EnterScope(context, typeof(T), source, out T instance)) return instance;

            int queueCount = context.QueueCount;
            _CopyToAs(source, ref instance, context);

            context.ExitScope(queueCount);

            return instance;
        }

        internal static T _CloneLeaf<T>(T source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            if (context.Cache.TryGetOrCache(typeof(T), source, out T instance)) return instance;

            _CopyToAs(source, ref instance, context);

            context.Cache.RemoveLatest();
            return instance;
        }

        private static T _CloneValue<T>(in T source, DeepCopyContext context)
            where T : struct
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif

#if NETSTANDARD2_0
            var instance = (T)FormatterServices.GetUninitializedObject(typeof(T));
#else
            var instance = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
#endif

            _CopyValueType(source, ref instance, context);

            return instance;
        }

        private static T? _CloneNullableValue<T>(in T? source, DeepCopyContext context)
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

            _CopyNullableValueType(source, ref instance, context);

            return instance;
        }

        private static T _CloneInterface<T>(T source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(T).Name}]");
#endif
            if (source == null) return default;

            var type = source.GetType();
            if (!type.IsValueType && context.Cache.Get(source, out var obj)) return obj;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return DictionaryCloneDelegateGenerator.GetOrCreateDelegate<Func<object, DeepCopyContext, T>>(type)(source, context);

            if (source is Array array)
            {
                return _CloneArray(array, context) is T cloned ? cloned : default;
            }

#if NETSTANDARD2_0
            var instance = (T)FormatterServices.GetUninitializedObject(type);
#else
            var instance = (T)RuntimeHelpers.GetUninitializedObject(type);
#endif

            if (type.IsValueType)
            {
                _CopyValueType(type, source, ref instance, context);
            }
            else
            {
                if (context.EnterScope(context, type, source, instance))
                {
                    return instance;
                }

                int queueCount = context.QueueCount;
                _CopyTo(type, source, ref instance, context);
                context.ExitScope(queueCount);
            }

            return instance;
        }

        internal static object _CloneObject(object source, DeepCopyContext context)
        {
            if (source == null) return default;

            var type = source.GetType();
#if DEBUGLOG
            Console.WriteLine($"[{typeof(object).Name} >> {type.Name}]");
#endif

#if NETSTANDARD2_0
            if (type == typeof(object)) return new object();
#endif
            if (type == typeof(string)) return source;

            if (!type.IsValueType && context.Cache.Get(source, out var obj)) return obj;

#if NETSTANDARD2_0
            var instance = FormatterServices.GetUninitializedObject(type);
#else
            var instance = RuntimeHelpers.GetUninitializedObject(type);
#endif

            if (type.IsValueType)
            {
                _CopyValueType(type, source, ref instance, context);
            }
            else
            {
                if (context.EnterScope(context, type, source, instance))
                {
                    return instance;
                }

                int queueCount = context.QueueCount;
                _CopyTo(type, source, ref instance, context);
                context.ExitScope(queueCount);
            }

            return instance;
        }

        private static Array _CloneArray(Array source, DeepCopyContext context)
        {
#if DEBUGLOG
            Console.WriteLine($"[{typeof(Array).Name}]");
#endif
            if (source == null) return default;

            if (context.Cache.Get(source, out var obj)) return obj;

            var type = source.GetType();
            var lengths = GetLengths(source);
#if NET9_0_OR_GREATER
            var instance = Array.CreateInstanceFromArrayType(type, lengths);
#else
            var instance = Array.CreateInstance(type.GetElementType(), lengths);
#endif

            context.Cache.Add(source, instance);

            var cloner = ArrayCloneDelegateGenerator.GetOrCreateWrapperDelegate(type);
            var cloned = cloner(source, context);

            context.Cache.RemoveLatest();

            Array.Copy(cloned, instance, source.Length);

            return instance;

            static int[] GetLengths(Array array)
            {
                var lengths = new int[array.Rank];
                for (var i = 0; i < array.Rank; i++)
                {
                    lengths[i] = array.GetLength(i);
                }
                return lengths;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _CopyTo<T>(in Type type, T source, ref T destination, DeepCopyContext context)
        {
            if (type == typeof(T))
            {
                var cloner = ReferenceTypeCloneDelegateGenerator<T>.Delegate;
                cloner(source, ref destination, context);
            }
            else
            {
                var cloner = ReferenceTypeCloneDelegateGenerator.CreateDelegate(type);
                cloner(source, (T)destination, context);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _CopyToAs<T>(T source, ref T destination, DeepCopyContext context)
        {
            var cloner = ReferenceTypeCloneDelegateGenerator<T>.Delegate;
            cloner(source, ref destination, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _CopyToAs<T>(Type type, T source, ref T destination, DeepCopyContext context)
        {
            var cloner = ReferenceTypeCloneDelegateGenerator.CreateDelegate(type);
            cloner(source, (T)destination, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _CopyValueType<T>(in Type type, in T source, ref T destination, DeepCopyContext context)
        {
            var cloner = ValueTypeCloneDelegateGenerator.CreateDelegate(type);
            cloner(source, out var obj, context);
            destination = (T)obj;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _CopyValueType<T>(in T source, ref T destination, DeepCopyContext context)
        {
            var cloner = ValueTypeCloneExpressionGenerator<T>.Delegate;
            cloner(source, ref destination, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _CopyNullableValueType<T>(in T? source, ref T? destination, DeepCopyContext context)
            where T : struct
        {
            var cloner = ValueTypeCloneExpressionGenerator<T?>.Delegate;
            cloner(source, ref destination, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _CopyTo<T>(T[] source, Array destination, DeepCopyContext context)
        {
            var cloner = CloneArrayExpressionGenerator<T, T[]>.Delegate;
            var cloned = cloner(source, context);
            Array.Copy(cloned, destination, destination.Length);
        }
    }
}
