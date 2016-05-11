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
            return typeof (IComparable).IsAssignableFrom(property.PropertyType);
        }

        public IEnumerable<Difference> Compare<T>(PropertyInfo property, T originalObject, T newObject)
        {
            var originalValue = property.GetValue(originalObject);
            var newValue = property.GetValue(newObject);

            if (((IComparable)originalValue).CompareTo(newValue) == 0)
                return Enumerable.Empty<Difference>();

            return new[]
            {
                new Difference
                {
                    OriginalValue = originalValue,
                    NewValue = newValue,
                    PropertyName = property.Name,
                    PropertyType = property.PropertyType
                }
            };
        }
    }
}