using System.Collections.Generic;
using DeepCopy.Test.Inners;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void ValueTypeDictionaryTest()
        {
            var dict = new Dictionary<int, string>()
            {
                [0] = "foo",
                [1] = "bar"
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            cloned.StructuralEquals(dict);
        }

        [Fact]
        public void ReferenceTypeDictionaryTest()
        {
            var dict = new Dictionary<object, object>()
            {
                [1] = new TestObject(),
                [2] = new TestObject()
            };
            var keys = dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(dict);

            cloned.StructuralEquals(dict);
        }

        [Fact]
        public void NestedDictionaryTest()
        {
            var dict = new Dictionary<object, Dictionary<int, string>>()
            {
                [1] = new Dictionary<int, string>
                {
                    [0] = "foo",
                    [1] = "bar",
                },
                [2] = new Dictionary<int, string>
                {
                    [0] = "foo",
                    [1] = "bar",
                    [2] = "baz"
                }
            };
            var keys = dict.Keys; // access ...
            foreach (var key in keys)
            {
                var _ = dict[key].Keys; // access ...
            }

            var cloned = ObjectCloner.Clone(dict);

            cloned.StructuralEquals(dict);
        }

        [Fact]
        public void HasDictionaryTest()
        {
            var cls = new
            {
                Id = 1,
                Dict = new Dictionary<object, Dictionary<int, string>>()
                {
                    [1] = new Dictionary<int, string>
                    {
                        [0] = "foo",
                        [1] = "bar",
                    },
                    [2] = new Dictionary<int, string>
                    {
                        [0] = "foo",
                        [1] = "bar",
                        [2] = "baz"
                    }
                }
            };
            var keys = cls.Dict.Keys; // access ...

            var cloned = ObjectCloner.Clone(cls);

            cloned.Id.Is(cls.Id);
            cloned.Dict.StructuralEquals(cls.Dict);
            cloned.Dict.IsNotSameReferenceAs(cls.Dict);
        }


    }
}
