using System;

namespace DeepCopy.Test
{
    [Cloneable]
    internal sealed class AttributeTestObject
    {
        private static Random _random =
            new Random(Environment.TickCount);

        [CopyMember]
        private readonly int _readonlyIntValue;
        [CopyMember]
        private readonly SubClass _readonlyInstance;
        [CopyMember]
        private readonly SubClass[] _readonlyArray;

        private int _notCopyIntValue;
        private SubClass _notCopyInstance;
        private SubClass[] _notCopyArray;

        [CopyMember]
        private int _defaultIntValue;
        [CopyMember]
        private SubClass _defaultInstane;
        [CopyMember]
        private SubClass[] _defaultArray;

        [CopyMember(CopyPolicy.DeepCopy)]
        private int _deepCopyIntValue;
        [CopyMember(CopyPolicy.DeepCopy)]
        private SubClass _deepCopyInstane;
        [CopyMember(CopyPolicy.DeepCopy)]
        private SubClass[] _deepCopyArray;

        [CopyMember(CopyPolicy.ShallowCopy)]
        private int _shallowCopyIntValue;
        [CopyMember(CopyPolicy.ShallowCopy)]
        private SubClass _shallowCopyInstane;
        [CopyMember(CopyPolicy.ShallowCopy)]
        private SubClass[] _shallowCopyArray;

        [CopyMember(CopyPolicy.Assign)]
        private int _simpleCopyIntValue;
        [CopyMember(CopyPolicy.Assign)]
        private SubClass _simpleCopyInstane;
        [CopyMember(CopyPolicy.Assign)]
        private SubClass[] _simpleCopyArray;

        public AttributeTestObject()
        {
            _readonlyIntValue = _random.Next(100);
            _readonlyInstance = new SubClass();
            _readonlyArray = new[] { new SubClass() };

            _notCopyIntValue = _random.Next(100);
            _notCopyInstance = new SubClass();
            _notCopyArray = new[] { new SubClass() };

            _defaultIntValue = _random.Next(100);
            _defaultInstane = new SubClass();
            _defaultArray = new[] { new SubClass() };

            _deepCopyIntValue = _random.Next(100);
            _deepCopyInstane = new SubClass();
            _deepCopyArray = new[] { new SubClass() };

            _shallowCopyIntValue = _random.Next(100);
            _shallowCopyInstane = new SubClass();
            _shallowCopyArray = new[] { new SubClass() };

            _simpleCopyIntValue = _random.Next(100);
            _simpleCopyInstane = new SubClass();
            _simpleCopyArray = new[] { new SubClass() };
        }

        public int NotCopyIntValue
        {
            get => _notCopyIntValue;
            set => _notCopyIntValue = value;
        }
        public SubClass NotCopyInstance
        {
            get => _notCopyInstance;
            set => _notCopyInstance = value;
        }
        public SubClass[] NotCopyArray
        {
            get => _notCopyArray;
            set => _notCopyArray = value;
        }

        public SubClass DefaultInstance => _defaultInstane;
        public SubClass[] DefaultArray => _defaultArray;

        public SubClass DeepCopyInstance => _deepCopyInstane;
        public SubClass[] DeepCopyArray => _deepCopyArray;

        public SubClass ShallowCopyInstance => _shallowCopyInstane;
        public SubClass[] ShallowCopyArray => _shallowCopyArray;

        public SubClass SimpleCopyInstance => _simpleCopyInstane;
        public SubClass[] SimpleCopyArray => _simpleCopyArray;

        public sealed class SubClass
        {
            public string Id { get; } = Guid.NewGuid().ToString();
        }
    }
}
