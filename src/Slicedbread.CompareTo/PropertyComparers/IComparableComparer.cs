namespace Slicedbread.CompareTo.PropertyComparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Models;

    public class IComparableComparer : IPropertyComparer
    {
        public bool CanCompare(PropertyInfo property)
        {
            return typeof(IComparable).IsAssignableFrom(property.PropertyType);
        }

        public IEnumerable<Difference> Compare<T>(PropertyInfo property, T originalObject, T newObject, IList<PropertyInfo> ignoreList)
        {
            var originalValue = property.GetValue(originalObject, new object[0]);
            var newValue = property.GetValue(newObject, new object[0]);

            // Handle properties being null, CompareTo would throw if we didn't handle them explicitly
            if (originalValue == null && newValue == null)
                return Enumerable.Empty<Difference>();

            if (originalValue == null || newValue == null)
                return Difference.SingleDifference(property.PropertyType, property.Name, originalValue, newValue);


            return ((IComparable)originalValue).CompareTo(newValue) == 0 ? 
                Enumerable.Empty<Difference>() : 
                Difference.SingleDifference(property.PropertyType, property.Name, originalValue, newValue);
        }
    }
}