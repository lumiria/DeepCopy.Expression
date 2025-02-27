using System.Collections.Generic;
using Xunit;


namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void EqualComparerMemberTest()
        {
            var instance = new TestClass("Test1");
            var cloned = ObjectCloner.Clone(instance);

            cloned.IsStructuralEqual(instance);
            cloned.Comparer.IsSameReferenceAs(EqualityComparer<TestClass>.Default);

            var comparer = new TestClassEqualComparer();
            var instance2 = new TestClass("Test2", comparer);
            var cloned2 = ObjectCloner.Clone(instance2);

            cloned2.IsStructuralEqual(instance2);
            cloned2.Comparer.IsNotSameReferenceAs(comparer);
        }

        private class TestClass
        {
            private const int _constValue = 1024;
            private readonly string _readonlyString;
            private readonly IEqualityComparer<TestClass> _comparer;

            public TestClass(string readonlyString, IEqualityComparer<TestClass> comparer = null)
            {
                _readonlyString = readonlyString;
                _comparer = comparer ?? EqualityComparer<TestClass>.Default;
            }

            public string Id => _readonlyString;

            public IEqualityComparer<TestClass> Comparer => _comparer;
        }

        private class TestClassEqualComparer : IEqualityComparer<TestClass>
        {
#if NET8_0_OR_GREATER
            public bool Equals(TestClass? x, TestClass? y)
            {
                return x?.Id == y?.Id;
            }
#else
            public bool Equals(TestClass x, TestClass y)
            {
                return x.Id == y.Id;
            }
#endif

            public int GetHashCode(TestClass obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
