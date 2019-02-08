using DeepCopy.Internal;

namespace DeepCopy
{
    /// <summary>
    /// Deep copying object.
    /// </summary>
    public static class ObjectCloner
    {
        /// <summary>
        /// Creates a new object thas is copy of the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">A source object.</param>
        /// <returns>A new object that is copy of the specified object.</returns>
        public static T Clone<T>(T source)
        {
            var instance = source.GetType() == typeof(T)
                ? CloneExpressionGenerator<T>.NewInstance()
                : (T)CloneExpressionGenerator.NewInstance(source.GetType());
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
        
        public static void Cleanup<T>()
        {
            CloneExpressionGenerator<T>.Clearnup();
        }
    }
}
