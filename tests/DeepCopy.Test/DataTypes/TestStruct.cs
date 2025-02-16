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

        int Id { get; set; }
        string Value { get; set;}
    }
#endif
}
