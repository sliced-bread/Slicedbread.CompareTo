namespace Slicedbread.CompareTo.PropertyComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Config;
    using Models;

    public class ValueTypeComparer : IPropertyComparer
    {
        public bool CanCompare(PropertyInfo property)
        {
            return property.PropertyType.IsValueType;
        }

        public IEnumerable<IDifference> Compare<T>(PropertyInfo property, T originalObject, T newObject, ComparisonConfig config)
        {
            var originalValue = property.GetValue(originalObject, new object[0]);
            var newValue = property.GetValue(newObject, new object[0]);

            // If original value is null, there is nothing to call 'Equals' on..
            if (originalValue == null)
            {
                if (newValue == null)
                    return Enumerable.Empty<Difference>();

                return Difference.SingleDifference(property.PropertyType, property.Name, null, newValue);
            }

            // Value types (should) all override .Equals()
            // https://msdn.microsoft.com/en-us/library/2dts52z7(v=vs.110).aspx

            return originalValue.Equals(newValue) 
                ? Enumerable.Empty<Difference>() 
                : Difference.SingleDifference(property.PropertyType, property.Name, originalValue, newValue);
        }
    }
}