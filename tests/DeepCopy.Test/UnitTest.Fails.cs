using System;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void NullTest()
        {
            Assert.Null(ObjectCloner.Clone(null));
            Assert.Null(ObjectCloner.Clone(null, true));

#if NET8_0_OR_GREATER
            object? objValue = null;
#else
            object objValue = null;
#endif
            ObjectCloner.Clone(objValue).IsNull();

            int? intValue = null;
            ObjectCloner.Clone(intValue).IsNull();

#if NET8_0_OR_GREATER
            object? clonedObj = null;
#else
            object clonedObj = null;
#endif

            ObjectCloner.CopyTo(null, clonedObj);
            clonedObj.IsNull();

            int? clonedInt = 0;
            ObjectCloner.CopyTo(null, ref clonedInt);
            clonedInt.IsNull();
        }

        [Fact]
        public void ExceptionTest()
        {
            var source = new ThrowOnGetter();
            var cloned = ObjectCloner.Clone(source);

            Assert.ThrowsAny<Exception>(() => cloned.X);
        }

        private sealed class ThrowOnGetter
        {
            public int X => throw new InvalidOperationException(nameof(X));
        }
    }
}
