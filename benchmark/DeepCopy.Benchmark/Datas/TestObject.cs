using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using DeepCopy.Benchmark.Datas.Utils;

namespace DeepCopy.Benchmark.Datas
{
    public sealed class TestObject : IEquatable<TestObject>
    {
        private bool _boolValue;
        private sbyte _sbyteValue;
        private byte _byteValue;
        private short _int16Value;
        private ushort _uint16Value;
        private int _int32Value;
        private uint _uint32Value;
        private long _int64Value;
        private ulong _uint64Value;
        private float _floatValue;
        private double _doubleValue;
        private string _stringValue;

        private int[] _intArray;
        private List<int> _intList;
        private Dictionary<int, string> _dict;

        private Child _child;
        private Child[] _children;

        private readonly int _readonlyIntValue;


        public TestObject()
        {
            var random = new Random(Environment.TickCount);

            _boolValue = random.Next(1) == 1;
            _sbyteValue = (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue);
            _byteValue = (byte)random.Next(byte.MaxValue);
            _int16Value = (short)random.Next(short.MinValue, short.MaxValue);
            _uint16Value = (ushort)random.Next(ushort.MaxValue);
            _int32Value = random.Next(int.MinValue, int.MaxValue);
            _uint32Value = (uint)random.Next(int.MaxValue);
            _int64Value = (long)random.Next(int.MinValue, int.MaxValue);
            _uint64Value = (ulong)random.Next(int.MaxValue);
            _floatValue = random.Next(int.MaxValue) / (float)random.Next(100);
            _doubleValue = random.Next(int.MaxValue) / (double)random.Next(100);
            _stringValue = Guid.NewGuid().ToString();

            _intArray = Enumerable.Range(0, 100)
                .Select(x => x * random.Next(100))
                .ToArray();
            _intList = Enumerable.Range(0, 100)
                .Select(x => x * random.Next(100))
                .ToList();
            _dict = Enumerable.Range(0, 100)
                .ToDictionary(x => x, x => random.Next(x).ToString());

            _child = new Child();
            _children = Enumerable.Range(0, 100)
                .Select(_ => new Child())
                .ToArray();

            _readonlyIntValue = random.Next(int.MaxValue);
        }

        [DataMember]
        public double DoublePropertyValue { get; set; }

        public IEnumerable<int> IntArray => _intArray;

        public IEnumerable<int> IntList => _intList;

        public Child[] Children => _children;

        public TestObject DeepCopy()
        {
            var instance = (TestObject)RuntimeHelpers.GetUninitializedObject((typeof(TestObject)));

            instance._boolValue = _boolValue;
            instance._sbyteValue = _sbyteValue;
            instance._byteValue = _byteValue;
            instance._int16Value = _int16Value;
            instance._uint16Value = _uint16Value;
            instance._int32Value = _int32Value;
            instance._uint32Value = _uint32Value;
            instance._int64Value = _int64Value;
            instance._uint64Value = _uint64Value;
            instance._floatValue = _floatValue;
            instance._doubleValue = _doubleValue;
            instance._stringValue = _stringValue;
            instance._intArray = new int[_intArray.Length];
            Buffer.BlockCopy(_intArray, 0, instance._intArray, 0, sizeof(int) * _intArray.Length);
            instance._intList = _intList.ToList();
            instance._dict = _dict.ToDictionary(x => x.Key, x => x.Value);
            instance._child = _child.DeepCopy();
            instance._children = new Child[_children.Length];
            for (int i = 0; i < Children.Length; ++i)
                instance._children[i] = _children[i].DeepCopy();
            instance.DoublePropertyValue = DoublePropertyValue;

            ReflectionUtils.SetReadonlyField(instance, nameof(_readonlyIntValue), _readonlyIntValue);

            return instance;
        }

        public override int GetHashCode() =>
            _boolValue.GetHashCode()
            ^ _sbyteValue.GetHashCode()
            ^ _byteValue.GetHashCode()
            ^ _int16Value.GetHashCode()
            ^ _uint16Value.GetHashCode()
            ^ _int32Value.GetHashCode()
            ^ _uint32Value.GetHashCode()
            ^ _int64Value.GetHashCode()
            ^ _uint64Value.GetHashCode()
            ^ _floatValue.GetHashCode()
            ^ _doubleValue.GetHashCode()
            ^ _stringValue.GetHashCode()
            ^ _intArray.GetHashCode()
            ^ _intList.GetHashCode()
            ^ _dict.GetHashCode()
            ^ _child.GetHashCode()
            ^ _children.GetHashCode()
            ^ _readonlyIntValue.GetHashCode()
            ;

        bool IEquatable<TestObject>.Equals(TestObject? other)
        {
            if (other == null)
                return false;

            return _boolValue == other._boolValue
                && _sbyteValue == other._sbyteValue
                && _byteValue == other._byteValue
                && _int16Value == other._int16Value
                && _uint16Value == other._uint16Value
                && _int32Value == other._int32Value
                && _uint32Value == other._uint32Value
                && _int64Value == other._int64Value
                && _uint64Value == other._uint64Value
                && _floatValue == other._floatValue
                && _doubleValue == other._doubleValue
                && _stringValue == other._stringValue
                && _intArray.SequenceEqual(other._intArray)
                && _intList.SequenceEqual(other._intList)
                && _child.Equals(other._child)
                && _children.SequenceEqual(other._children)
                && _dict.StructuralEquals(other._dict)
                && _readonlyIntValue == other._readonlyIntValue
                && DoublePropertyValue == other.DoublePropertyValue;
        }

        public override bool Equals(object? obj)
        {
            return ((IEquatable<TestObject>)this).Equals(obj as TestObject);
        }
    }

    internal class TestKey
    {
        public int Id { get; set; }
        public string? Value { get; set; }

        public TestKey()
        {
            Random _random =
                new(Environment.TickCount);
            Id = _random.Next(0, 999);
            Value = $"Key-{Id}";
        }
    }


    internal record class TestValue
    {
        public string? Value { get; set; }

        public static TestValue Create() =>
            new() { Value = $"Value-{new Random(Environment.TickCount).Next(0, 999)}" };
    }
}
