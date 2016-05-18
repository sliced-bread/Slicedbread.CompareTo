namespace Slicedbread.CompareTo.Tests.Models
{
    using System;

    internal class ComparableHolder
    {
        public ComparableThing ComparableThing { get; set; }
        public ComparableThing AnotherComparableThing { get; set; }
    }

    internal class ComparableThing : IComparable
    {
        public int Value { get; }

        public ComparableThing(int value)
        {
            Value = value;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return -1;

            return Value == ((ComparableThing) obj).Value
                ? 0
                : -1;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
