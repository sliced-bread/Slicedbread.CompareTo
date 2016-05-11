namespace Slicedbread.CompareTo.Models
{
    using System;

    public class Difference
    {
        public Type PropertyType { get; set; }
        public string PropertyName { get; set; }

        public object OriginalValue { get; set; }
        public object NewValue { get; set; }

        public override string ToString()
        {
            return string.Format("'{0}' changed from '{1}' to '{2}'", PropertyName, OriginalValue, NewValue);
        }
    }
}