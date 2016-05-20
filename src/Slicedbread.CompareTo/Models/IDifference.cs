namespace Slicedbread.CompareTo.Models
{
    using System;

    public interface IDifference
    {
        Type PropertyType { get; }
        string PropertyName { get; set; }

        object OriginalValue { get; }
        object NewValue { get; }
    }
}