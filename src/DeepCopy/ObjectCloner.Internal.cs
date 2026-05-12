using System;
using DeepCopy.Internal;

namespace DeepCopy
{
    public static partial class ObjectCloner
    {
        internal static void CopyTo<T>(Type type, T source, ref T destination, DeepCopyContext context) =>
            _CopyTo(type, source, ref destination, context);

        internal static void CopyToAs<T>(T source, ref T destination, DeepCopyContext context)
           => _CopyToAs(source, ref destination, context);
    }
}
