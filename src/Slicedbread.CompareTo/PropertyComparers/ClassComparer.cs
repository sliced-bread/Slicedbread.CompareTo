namespace Slicedbread.CompareTo.PropertyComparers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Engine;
    using Models;

    public class ClassComparer : IPropertyComparer
    {
        public bool CanCompare(PropertyInfo property)
        {
            return property.PropertyType.IsClass;
        }

        public IEnumerable<Difference> Compare<T>(PropertyInfo property, T originalObject, T newObject, IList<PropertyInfo> ignoreList)
        {
            var comparisonEngine = new ComparisonEngine();

            var originalValue = property.GetValue(originalObject,new object[0]);
            var newValue = property.GetValue(newObject, new object[0]);

            // Recurse back into comparison engine for this nested class
            var results = comparisonEngine.Compare(property.PropertyType, originalValue, newValue, ignoreList);

            // Prefix all property names
            foreach (var result in results)
            {
                result.PrefixPropertyName(property.Name + ".");
            }

            return results;
        }
    }
}