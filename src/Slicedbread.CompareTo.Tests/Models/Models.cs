namespace Slicedbread.CompareTo.Tests.Models
{
    using System;

    internal class SimpleValueTypePoco
    {
        public int IntegerProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }

        private string _cannotRead;
        public string CannotRead { set { _cannotRead = value; } }
    }

    internal class NestedTypeParent
    {
        public NestedTypeChild Child { get; set; }
    }

    internal class NestedTypeChild
    {
        public int IntegerProperty { get; set; }
    }
}
