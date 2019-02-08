using System;

namespace DeepCopy.Test
{
    public sealed class ArrayTestObject
    {
        private static Random _random =
            new Random(Environment.TickCount);

        private int[] _intArray;
        private int[,] _intArray2;
        private int[,,] _intArray3;
        private int[][] _intjaggedArray;

        private Child[] _objArray;
        private Child[,] _objArray2;
        private Child[][] _objJaggedArray;

        public ArrayTestObject()
        {
            _intArray = new int[_random.Next(1, 10)];
            for (int i=0; i<_intArray.Length; ++i)
            {
                _intArray[i] = _random.Next(1, 100);
            }

            _intArray2 = new int[_random.Next(1, 10), _random.Next(1, 10)];
            for (int i=0; i<_intArray2.GetLength(0); ++i)
            {
                for (int j=0; j<_intArray2.GetLength(1); ++j)
                {
                    _intArray2[i, j] = _random.Next(1, 100);
                }
            }

            _intArray3 = new int[_random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10)];
            for (int i = 0; i < _intArray3.GetLength(0); ++i)
            {
                for (int j = 0; j < _intArray3.GetLength(1); ++j)
                {
                    for (int k = 0; k < _intArray3.GetLength(2); ++k)
                    {
                        _intArray3[i, j, k] = _random.Next(1, 100);
                    }
                }
            }

            _intjaggedArray = new int[_random.Next(1, 10)][];
            for (int i=0; i<_intjaggedArray.Length; ++i)
            {
                _intjaggedArray[i] = new int[_random.Next(1, 10)];
                for (int j=0; j<_intjaggedArray[i].Length; ++j)
                {
                    _intjaggedArray[i][j] = _random.Next(1, 100);
                }
            }

            _objArray = new Child[_random.Next(1, 10)];
            for (int i = 0; i < _objArray.Length; ++i)
            {
                _objArray[i] = new Child();
            }

            _objArray2 = new Child[_random.Next(1, 10), _random.Next(1, 10)];
            for (int i = 0; i < _objArray2.GetLength(0); ++i)
            {
                for (int j = 0; j < _objArray2.GetLength(1); ++j)
                {
                    _objArray2[i, j] = new Child();
                }
            }

            _objJaggedArray = new Child[_random.Next(1, 10)][];
            for (int i = 0; i < _objJaggedArray.Length; ++i)
            {
                _objJaggedArray[i] = new Child[_random.Next(1, 10)];
                for (int j = 0; j < _objJaggedArray[i].Length; ++j)
                {
                    _objJaggedArray[i][j] = new Child();
                }
            }
        }

        public int[] IntArray => _intArray;
        public int[,] IntArray2 => _intArray2;
        public int[,] IntArray3 => _intArray2;
        public int[][] IntjaggedArray => _intjaggedArray;

        public Child[] ObjArray => _objArray;
        public Child[,] ObjArray2 => _objArray2;
        public Child[][] ObjJaggedArray => _objJaggedArray;
    }
}
