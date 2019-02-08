namespace DeepCopy.Test
{
    public sealed class ObjectTestObject
    {
        private object _obj;
        private object _child;

        public ObjectTestObject()
        {
            _obj = new object();
            _child = new Child();
        }

        public object Obj => _obj;

        public object Child => _child;
    }
}
