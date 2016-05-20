namespace Slicedbread.CompareTo.Tests.Models
{
    using System;

    public class CollectionItem
    {
        public Guid Id { get; private set; }
        public string Value { get; private set; }

        public CollectionItem(Guid id, string value)
        {
            Id = id;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public Guid GetId()
        {
            return Id;
        }
    }
}