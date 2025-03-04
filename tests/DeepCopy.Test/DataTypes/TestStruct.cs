namespace DeepCopy.Test.DataTypes
{
#if NET8_0_OR_GREATER
    internal record struct TestStruct(int Id, string Value);
#else
    internal struct TestStruct
    {
        public TestStruct(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public int Id { get; set; }
        public string Value { get; set;}
    }
#endif
}
