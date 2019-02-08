using System;

namespace DeepCopy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CloneableAttribute : Attribute
    {
    }
}
