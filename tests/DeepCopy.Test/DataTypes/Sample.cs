using System;

namespace DeepCopy.Test.DataTypes
{
#if NET8_0_OR_GREATER
    internal abstract record class BaseSample(string Name);

    internal sealed record class SubSample(int Id, string Name) : BaseSample(Name);
#else
    internal abstract class BaseSample : IEquatable<BaseSample>
    {
        public BaseSample(string name)
        {
            Name = name;
        }

        string Name { get; }

        public bool Equals(BaseSample other)
        {
            return Name == other.Name;
        }
    }

    internal sealed class SubSample : BaseSample, IEquatable<SubSample>
    {
        public SubSample(int id, string name)
            : base(name)
        {
            Id = id;
        }

        public int Id { get; }

        public bool Equals(SubSample other)
        {
            return Id == other.Id && base.Equals(other);
        }
    }
#endif
}
