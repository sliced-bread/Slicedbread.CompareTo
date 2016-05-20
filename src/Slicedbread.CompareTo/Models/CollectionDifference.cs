namespace Slicedbread.CompareTo.Models
{
    using System;

    public class CollectionDifference : IDifference
    {
        public Type PropertyType { get; }
        public string PropertyName { get; set; }
        public object OriginalValue { get; }
        public object NewValue { get; }

        public CollectionDifference(Type propertyType, string propertyName, object originalValue, object newValue)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            OriginalValue = originalValue;
            NewValue = newValue;
        }

        public override string ToString()
        {
            if (OriginalValue == null && NewValue != null)
                return string.Format("'{0}' was added to '{1}'", NewValue, PropertyName);

            if (OriginalValue != null && NewValue == null)
                return string.Format("'{0}' was removed from '{1}'", OriginalValue, PropertyName);

            return string.Empty;
        }
    }
}