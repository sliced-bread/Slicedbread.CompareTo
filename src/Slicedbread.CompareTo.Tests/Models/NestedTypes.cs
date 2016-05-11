namespace Slicedbread.CompareTo.Tests.Models
{
    internal class NestedTypeParent
    {
        public NestedTypeChild Child { get; set; }
    }

    internal class NestedTypeChild
    {
        public int IntegerProperty { get; set; }
    }
}