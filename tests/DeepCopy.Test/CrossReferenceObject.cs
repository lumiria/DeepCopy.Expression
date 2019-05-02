using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCopy.Test
{
    public sealed class CrossReferenceObject
    {
        private readonly ReferenceA _a;
        private readonly ReferenceB _b;

        public CrossReferenceObject()
        {
            _b = new ReferenceB();
            _a = new ReferenceA(_b);
        }

        public ReferenceA A => _a;
        public ReferenceB B => _b;
    }

    public sealed class ReferenceA
    {
        private readonly ReferenceB _b;

        public ReferenceA(ReferenceB b)
        {
            _b = b;
            b.A = this;
        }

        public ReferenceB B => _b;
    }

    public sealed class ReferenceB
    {
        public ReferenceB()
        {
        }

        public ReferenceA A { get; set; }
    }
}
