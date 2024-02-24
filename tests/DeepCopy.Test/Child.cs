using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DeepCopy.Test.Inners;

namespace DeepCopy.Test
{
    [DataContract]
    public sealed class Child : IEquatable<Child>
    {
        private static Random _random =
            new Random(Environment.TickCount);

        [DataMember]
        private readonly Guid _id;
        [DataMember]
        private int _intValue;
        [DataMember]
        private int[] _intArray;

        public Child()
        {
            _intValue = _random.Next(100);
            _id = Guid.NewGuid();
            _intArray = Enumerable.Range(0, 100)
                .Select(x => x * _random.Next(100))
                .ToArray();
        }

        public int[] IntArray => _intArray;

        public Child DeepCopy()
        {
            var instance = (Child)RuntimeHelpers.GetUninitializedObject(typeof(Child));
            ReflectionUtils.SetReadonlyField(instance, nameof(_id), _id);
            instance._intValue = _intValue;
            instance._intArray = new int[_intArray.Length];
            Buffer.BlockCopy(_intArray, 0, instance._intArray, 0, sizeof(int) * _intArray.Length);

            return instance;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode()
                ^_intValue.GetHashCode()
                ^ _intArray.GetHashCode()
                ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Child)) return false;
            return ((IEquatable<Child>)this).Equals((Child)obj);
        }

        bool IEquatable<Child>.Equals(Child other)
        {
            if (this == other) return false;

            return _id == other._id
                && _intValue == other._intValue
                && _intArray.SequenceEqual(other._intArray)
                ;
        }
    }
}
