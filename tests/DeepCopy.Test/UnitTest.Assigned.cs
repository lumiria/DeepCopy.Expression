using System;
using System.Collections.Generic;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void UriTest()
        {
            var source = new UriHolder
            {
                Uri = new Uri("https://example.com")
            };

            var cloned = ObjectCloner.Clone(source.Uri);

            //Assert.Same(source.Uri, cloned);
            Assert.Equal(source.Uri, cloned);

            var clonedHolder = ObjectCloner.Clone(source);

            Assert.NotSame(source, clonedHolder);
            //Assert.Same(source.Uri, clonedHolder.Uri);
            Assert.Equal(source.Uri, clonedHolder.Uri);

            var dict = new Dictionary<Uri, Uri>
            {
                { new Uri("https://example.com/key/01"),  new Uri("https://example.com/value/01") },
                { new Uri("https://example.com/key/02"),  new Uri("https://example.com/value/02") },
            };

            var clonedDict = ObjectCloner.Clone(dict);

            foreach (var (key, value) in dict)
            {
                //Assert.Same(clonedDict[key], value);
                Assert.Equal(clonedDict[key], value);
            }
        }

        [Fact]
        public void VersionTest()
        {
            var source = new VersionHolder
            {
                Version = new Version(1, 2, 3)
            };

            var cloned = ObjectCloner.Clone(source.Version);

            //Assert.Same(source.Version, cloned);
            Assert.Equal(source.Version, cloned);

            var clonedHolder = ObjectCloner.Clone(source);

            Assert.NotSame(source, clonedHolder);
            //Assert.Same(source.Version, clonedHolder.Version);
            Assert.Equal(source.Version, clonedHolder.Version);

            var dict = new Dictionary<Version, Version>
            {
                { new Version(1, 2, 3),  new Version(4, 5, 6) },
                { new Version(3, 2, 1),  new Version(6, 5, 4) },
            };

            var clonedDict = ObjectCloner.Clone(dict);

            foreach (var (key, value) in dict)
            {
                //Assert.Same(clonedDict[key], value);
                Assert.Equal(clonedDict[key], value);
            }
        }

        private sealed class UriHolder
        {
            public Uri Uri { get; set; } = null!;
        }

        private sealed class VersionHolder
        {
            public Version Version { get; set; } = null!;
        }
    }
}
