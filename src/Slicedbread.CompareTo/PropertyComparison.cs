namespace Slicedbread.CompareTo
{
    using System;

    public class PropertyComparison
    {
        public bool HasChanged { get; set; }

        public Type PropertyType { get; set; }
        public string PropertyName { get; set; }

        public object OriginalValue { get; set; }
        public object NewValue { get; set; }
    }
}