namespace DeepCopy.Test
{
    public sealed class ObjectTestObject
    {
        private object _obj;
        private object _child;
        private object _intObj;
        private object _stringObj;

        public ObjectTestObject()
        {
            _obj = new object();
            _child = new Child();
            _intObj = 10;
            _stringObj = "Test";
        }

        public object Obj => _obj;

        public object Child => _child;

        public object IntObj => _intObj;

        public object StringObj => _stringObj;
    }
}
