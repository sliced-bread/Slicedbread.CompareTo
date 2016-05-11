namespace Slicedbread.CompareTo.PropertyComparers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Models;

    public class ClassComparer : IPropertyComparer
    {
        public bool CanCompare(PropertyInfo property)
        {
            return property.PropertyType.IsClass;
        }

        public IEnumerable<Difference> Compare<T>(PropertyInfo property, T originalObject, T newObject)
        {
            var comparisonEngine = new ComparisonEngine();

            var originalValue = property.GetValue(originalObject);
            var newValue = property.GetValue(newObject);

            // Recurse back into comparison engine for this nested class
            var results = comparisonEngine.CompareTo(property.PropertyType, originalValue, newValue);

            // Prefix all property names
            foreach (Difference result in results)
            {
                result.PropertyName = property.Name + "." + result.PropertyName;
            }

            return results;
        }
    }
}