using System;

namespace DeepCopy
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class CopyMemberAttribute : Attribute
    {
        public CopyMemberAttribute(CopyPolicy copyPolicy = CopyPolicy.Default)
        {
            CopyPolicy = copyPolicy;
        }

        public CopyPolicy CopyPolicy { get; set; }

        public static CopyMemberAttribute Default { get; } = new();
    }
}
