using System;

namespace DeepCopy.Test
{
    public class PolymoirphicTestObject
    {
        public interface I
        {
            int I_Value { get; }
        }

        public class A : I, IEquatable<A>
        {
            int _i_value;
            int _a_value;
            readonly int _a_readonly_value;

            public A(int value)
            {
                _i_value = value;
                _a_value = value * 10;
                _a_readonly_value = value * 20;
            }

            public int I_Value => _i_value;

            public int A_Value => _a_value;

            public override int GetHashCode()
            {
                return _i_value.GetHashCode()
                    ^ _a_value.GetHashCode()
                    ^ _a_readonly_value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is A)) return false;
                return ((IEquatable<A>)this).Equals((A)obj);
            }

            bool IEquatable<A>.Equals(A other)
            {
                if (this == other) return false;

                return _i_value == other._i_value
                    && _a_value == other._a_value
                    && _a_readonly_value == other._a_readonly_value;
            }
        }

        public class B : A, IEquatable<B>
        {
            int _b_value;

            readonly int _b_readonly_value;

            public B(int value)
                : base(value)
            {
                _b_value = 10;
                _b_readonly_value = 20;
            }

            public int B_Value => _b_value;

            public override int GetHashCode()
            {
                return _b_value.GetHashCode()
                    ^ _b_readonly_value.GetHashCode()
                    ^ base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is B)) return false;
                return ((IEquatable<B>)this).Equals((B)obj);
            }

            bool IEquatable<B>.Equals(B other)
            {
                if (this == other) return false;

                return _b_value == other._b_value
                    && _b_readonly_value == other._b_readonly_value
                    && base.Equals((A)other);
            }
        }
    }
}
