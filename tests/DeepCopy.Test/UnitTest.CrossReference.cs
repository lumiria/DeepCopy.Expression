using Xunit;

namespace DeepCopy.Test
{
    public partial class UnitTest
    {
        [Fact]
        public void SelfReferenceTest()
        {
            var self = new Graph { Name = "self" };
            self.Next = self;

            var cloned1 = ObjectCloner.Clone(self);
            cloned1.IsNotSameReferenceAs(self);

            cloned1.Next.IsNotSameReferenceAs(self);
            cloned1.Next.IsNotSameReferenceAs(self.Next);
            cloned1.Next.IsSameReferenceAs(cloned1);

            var cloned2 = ObjectCloner.Clone(self, true);
            cloned2.IsNotSameReferenceAs(self);

            cloned2.Next.IsNotSameReferenceAs(self);
            cloned2.Next.IsNotSameReferenceAs(self.Next);
            cloned2.Next.IsSameReferenceAs(cloned2);

            Graph cloned3 = new Graph();
            ObjectCloner.CopyTo(self, cloned3);
            cloned3.IsNotSameReferenceAs(self);

            cloned3.Next.IsNotSameReferenceAs(self);
            cloned3.Next.IsNotSameReferenceAs(self.Next);
            cloned3.Next.IsSameReferenceAs(cloned3);
        }

        [Fact]
        public void TwoNodeReferenceTest()
        {
            var a = new Graph { Name = "a" };
            var b = new Graph { Name = "b" };
            a.Next = b;
            b.Next = a;

            var cloned1 = ObjectCloner.Clone(a);
            cloned1.IsNotSameReferenceAs(a);

            cloned1.Next.IsNotSameReferenceAs(b);
            cloned1.Next.IsNotSameReferenceAs(a.Next);
#if NET8_0_OR_GREATER
            cloned1.IsSameReferenceAs(cloned1.Next!.Next);
            cloned1.Next.IsSameReferenceAs(cloned1.Next!.Next!.Next);
#else
            cloned1.IsSameReferenceAs(cloned1.Next.Next);
            cloned1.Next.IsSameReferenceAs(cloned1.Next.Next.Next);
#endif

            var cloned2 = ObjectCloner.Clone(a, true);
            cloned2.IsNotSameReferenceAs(a);

            cloned2.Next.IsNotSameReferenceAs(b);
            cloned2.Next.IsNotSameReferenceAs(a.Next);
#if NET8_0_OR_GREATER
            cloned2.IsSameReferenceAs(cloned2.Next!.Next);
            cloned2.Next.IsSameReferenceAs(cloned2.Next!.Next!.Next);
#else
            cloned2.IsSameReferenceAs(cloned2.Next.Next);
            cloned2.Next.IsSameReferenceAs(cloned2.Next.Next.Next);
#endif

            Graph cloned3 = new Graph();
            ObjectCloner.CopyTo(a, cloned3);
            cloned3.IsNotSameReferenceAs(a);

            cloned3.Next.IsNotSameReferenceAs(b);
            cloned3.Next.IsNotSameReferenceAs(a.Next);
#if NET8_0_OR_GREATER
            cloned3.IsSameReferenceAs(cloned3.Next!.Next);
            cloned3.Next.IsSameReferenceAs(cloned3.Next!.Next!.Next);
#else
            cloned3.IsSameReferenceAs(cloned3.Next.Next);
            cloned3.Next.IsSameReferenceAs(cloned3.Next.Next.Next);
#endif
        }

        [Fact]
        public void TreeCrossReferenceTest()
        {
            var root = new Graph { Name = "root" };
            root.Next = new Graph { Name = "leaf1" };
            var leaf = new Graph { Name = "leaf2", Parent = root };
            root.Children.Add(leaf);

            var cloned1 = ObjectCloner.Clone(root);
            cloned1.IsNotSameReferenceAs(root);

            cloned1.Children[0].IsNotSameReferenceAs(leaf);
#if NET8_0_OR_GREATER
            cloned1.IsSameReferenceAs(cloned1.Children[0]!.Parent);
#else
            cloned1.IsSameReferenceAs(cloned1.Children[0].Parent);
#endif

            var cloned2 = ObjectCloner.Clone(root, true);
            cloned2.IsNotSameReferenceAs(root);

            cloned2.Children[0].IsNotSameReferenceAs(leaf);
#if NET8_0_OR_GREATER
            cloned2.IsSameReferenceAs(cloned2.Children[0]!.Parent);
#else
            cloned2.IsSameReferenceAs(cloned2.Children[0].Parent);
#endif

            Graph cloned3 = new Graph();
            ObjectCloner.CopyTo(root, cloned3);
            cloned3.IsNotSameReferenceAs(root);

            cloned3.Children[0].IsNotSameReferenceAs(leaf);
#if NET8_0_OR_GREATER
            cloned3.IsSameReferenceAs(cloned3.Children[0]!.Parent);
#else
            cloned3.IsSameReferenceAs(cloned3.Children[0].Parent);
#endif
        }
    }
}
