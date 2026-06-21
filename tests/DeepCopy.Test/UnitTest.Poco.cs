using System.Collections.Generic;
using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void PocoTest()
        {
            var source = new Poco(10, 20)
            {
                PublicField = 1,
#if NET8_0_OR_GREATER
                InitOnly = 3,
#endif
            };
            source.WriteOnly = 5;

            var cloned = ObjectCloner.Clone(source);

            cloned.IsNotSameReferenceAs(source);
            cloned.IsStructuralEqual(source);
        }

        [Fact]
        public void SharedTest()
        {
            var shared = new Node { Value = 30 };

            var root = new
            {
                A = shared,
                B = shared
            };

            var cloned1 = ObjectCloner.Clone(root);
            cloned1.IsNotSameReferenceAs(root);
            cloned1.IsStructuralEqual(root);

            cloned1.A.IsNotSameReferenceAs(root.A);
            cloned1.B.IsNotSameReferenceAs(root.B);
            cloned1.A.IsNotSameReferenceAs(cloned1.B);
        }

        [Fact]
        public void NestedTest()
        {
            var shared = new Node { Value = 30 };
            var root = new Graph { Name = "A" };
            root.Left = new Graph { Name = "B", Left = new Graph { Name = "C", Payload = shared } };
            root.Right = new Graph { Name = "D", Right = new Graph { Name = "E", Payload = shared } };

            var cloned1 = ObjectCloner.Clone(root);
            cloned1.IsNotSameReferenceAs(root);
            cloned1.IsStructuralEqual(root);
#if NET8_0_OR_GREATER
            cloned1.Left!.IsNotSameReferenceAs(root.Left);
            cloned1.Right!.IsNotSameReferenceAs(root.Right);
            cloned1.Left!.Left!.IsNotSameReferenceAs(root.Left.Left);
            cloned1.Right!.Right!.IsNotSameReferenceAs(root.Right.Right);
            cloned1.Left!.Left!.Payload!.IsNotSameReferenceAs(cloned1.Right!.Right!.Payload);
#else
            cloned1.Left.IsNotSameReferenceAs(root.Left);
            cloned1.Right.IsNotSameReferenceAs(root.Right);
            cloned1.Left.Left.IsNotSameReferenceAs(root.Left.Left);
            cloned1.Right.Right.IsNotSameReferenceAs(root.Right.Right);
            cloned1.Left.Left.Payload.IsNotSameReferenceAs(cloned1.Right.Right.Payload);
#endif

            // NOTE: Restores shared references when the second argument is set to true.
            var cloned2 = ObjectCloner.Clone(root, true);
#if NET8_0_OR_GREATER
            cloned2.Left!.IsNotSameReferenceAs(root.Left);
            cloned2.Right!.IsNotSameReferenceAs(root.Right);
            cloned2.Left!.Left!.IsNotSameReferenceAs(root.Left.Left);
            cloned2.Right!.Right!.IsNotSameReferenceAs(root.Right.Right);
            cloned2.Left!.Left!.Payload!.IsSameReferenceAs(cloned2.Right!.Right!.Payload);
#else
            cloned2.Left.IsNotSameReferenceAs(root.Left);
            cloned2.Right.IsNotSameReferenceAs(root.Right);
            cloned2.Left.Left.IsNotSameReferenceAs(root.Left.Left);
            cloned2.Right.Right.IsNotSameReferenceAs(root.Right.Right);
            cloned2.Left.Left.Payload.IsSameReferenceAs(cloned2.Right.Right.Payload);
#endif
        }

        [Fact]
        public void ChainTest()
        {
            var source = BuildChain(5000);
            var cloned = ObjectCloner.Clone(source);

            cloned.IsNotSameReferenceAs(source);
            cloned.IsStructuralEqual(source);

            var nextSource = source;
            var nextCloned = cloned;
            while (nextSource != null)
            {
                nextCloned.IsNotSameReferenceAs(nextSource);
                nextCloned?.Name.Is(nextSource.Name);

                nextSource = nextSource.Next;
                nextCloned = nextCloned?.Next;
            }
            nextCloned.IsNull();

            var copied = new Graph();
            ObjectCloner.CopyTo(source, copied);

            copied.IsNotSameReferenceAs(source);
            copied.IsStructuralEqual(source);

            nextSource = source;
            var nextCopied = copied;
            while (nextSource != null)
            {
                nextCopied.IsNotSameReferenceAs(nextSource);
                nextCopied?.Name.Is(nextSource.Name);

                nextSource = nextSource.Next;
                nextCopied = nextCopied?.Next;
            }
            nextCopied.IsNull();

        }

        private static Graph BuildChain(int depth)
        {
            var root = new Graph() {  Name = "root" };

            var current = root;
            for (var i = 0; i < depth; i++)
            {
                current.Next = new Graph() { Name = $"{i}" };
                current = current.Next;
            }

            return root;
        }

        private sealed class Poco
        {
            private readonly int _private;

            public Poco(int privateValue, int privateSetter)
            {
                _private = privateValue;
                PrivateSetterValue = privateSetter;
            }

            public int PublicField { get; set; }

#if NET8_0_OR_GREATER
            public int InitOnly { get; init; }
#endif

#if NET8_0_OR_GREATER
            public int PrivateSetterValue { get; init; }
#else
            public int PrivateSetterValue { get; private set; }
#endif

            public int WriteOnly { private get; set; }

            public int Computed => PublicField + _private;
        }


    }
}
