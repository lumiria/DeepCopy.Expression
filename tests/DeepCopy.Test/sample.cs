using System;
using System.Collections.Generic;
using System.Text;

namespace DeepCopy.Test
{
    internal class Sample
    {
        private static Random _random =
            new Random(Environment.TickCount);

        private readonly int _id;
        public Child[] _children;

        public Sample()
        {
            _id = _random.Next(int.MaxValue);
            _children = new Child[3];
            for (int i=0; i<_children.Length; ++i)
            {
                _children[i] = new Child();
            }
        }
    }

    public interface I
    {
        int I_Value { get; }
    }

    public class A : I
    {
        int _i_value;
        int _a_value;

        public A(int value)
        {
            _i_value = value;
            _a_value = value * 10;
        }

        public int I_Value => _i_value;

        public int A_Value => _a_value;
    }

    public class B : A
    {
        int _b_value;

        public B(int value)
            : base(value) { }
    }
}
