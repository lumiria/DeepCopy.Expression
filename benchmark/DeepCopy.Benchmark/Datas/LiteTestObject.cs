namespace DeepCopy.Benchmark.Datas
{
    public sealed class LiteTestObject
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
        private Dictionary<TestKey, TestValue> _refDict;

        private LiteChild _child;
        private LiteChild[] _children;

        public LiteTestObject()
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
            _refDict = Enumerable.Range(0, 100)
                .ToDictionary(x => new TestKey() { Id = x }, x => new TestValue() { Value = random.Next(x).ToString()});

            _child = new LiteChild();
            _children = Enumerable.Range(0, 100)
                .Select(_ => new LiteChild())
                .ToArray();
        }

        public double DoublePropertyValue { get; set; }

        public IEnumerable<int> IntArray => _intArray;

        public IEnumerable<int> IntList => _intList;

        public LiteChild[] Children => _children;
    }

    public sealed class LiteChild : IEquatable<LiteChild>
    {
        private static readonly Random _random =
            new(Environment.TickCount);

        private int _intValue;
        private int[] _intArray;

        public LiteChild()
        {
            _intValue = _random.Next(100);
            _intArray = Enumerable.Range(0, 100)
                .Select(x => x * _random.Next(100))
                .ToArray();
        }

        public int[] IntArray => _intArray;

        public override int GetHashCode()
        {
            return _intValue.GetHashCode()
                ^ _intArray.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not LiteChild child) return false;
            return ((IEquatable<LiteChild>)this).Equals(child);
        }

        bool IEquatable<LiteChild>.Equals(LiteChild? other)
        {
            if (this == other) return false;

            return _intValue == other._intValue
                && _intArray.SequenceEqual(other._intArray)
                ;
        }
    }
}
