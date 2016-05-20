namespace Slicedbread.CompareTo.Tests.Models
{
    using System;

    public class CollecionItem
    {
        public Guid Id { get; private set; }
        public string Value { get; private set; }

        public CollecionItem(Guid id, string value)
        {
            Id = id;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}