namespace Slicedbread.CompareTo.PropertyComparers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Models;

    public class ValueTypeComparer : IPropertyComparer
    {
        public bool CanCompare(PropertyInfo property)
        {
            return property.PropertyType.IsValueType;
        }

        public IEnumerable<Difference> Compare<T>(PropertyInfo property, T originalObject, T newObject)
        {
            // Value types all (should) override .Equals()
            // https://msdn.microsoft.com/en-us/library/2dts52z7(v=vs.110).aspx

            var originalValue = property.GetValue(originalObject);
            var newValue = property.GetValue(newObject);

            if (originalValue.Equals(newValue))
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