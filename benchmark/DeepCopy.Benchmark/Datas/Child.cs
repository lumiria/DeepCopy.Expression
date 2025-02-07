using System.Runtime.CompilerServices;
using DeepCopy.Benchmark.Datas.Utils;

namespace DeepCopy.Benchmark.Datas
{
    public sealed class Child : IEquatable<Child>
    {
        private static readonly Random _random =
            new(Environment.TickCount);

        private readonly Guid _id;
        private int _intValue;
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

        public override bool Equals(object? obj)
        {
            if (obj is not Child child) return false;
            return ((IEquatable<Child>)this).Equals(child);
        }

        bool IEquatable<Child>.Equals(Child? other)
        {
            if (this == other) return false;

            return _id == other?._id
                && _intValue == other._intValue
                && _intArray.SequenceEqual(other._intArray)
                ;
        }
    }
}
