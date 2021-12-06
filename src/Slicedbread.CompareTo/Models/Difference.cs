namespace Slicedbread.CompareTo.Models
{
    using System;
    using System.Collections.Generic;

    public class Difference : IDifference
    {
        public Type PropertyType { get; private set; }
        public string PropertyName { get; set; }

        public object OriginalValue { get; private set; }
        public object NewValue { get; private set; }

        public Difference(Type propertyType, string propertyName, object originalValue, object newValue)
        {
            PropertyType = propertyType;
            PropertyName = propertyName;
            OriginalValue = originalValue;
            NewValue = newValue;
        }

        public static IEnumerable<Difference> SingleDifference(Type propertyType, string propertyName, object originalValue, object newValue)
        {
            return new[]
            {
                new Difference(propertyType, propertyName, originalValue, newValue)
            };
        }
        public override string ToString()
        {
            if (OriginalValue is null)
                return $"Set '{PropertyName}' to '{NewValue}'";

            if (NewValue is null)
                return $"Removed '{PropertyName}' (value was previously '{OriginalValue}')";

            return $"'{PropertyName}' changed from '{OriginalValue}' to '{NewValue}'";
        }
    }
}