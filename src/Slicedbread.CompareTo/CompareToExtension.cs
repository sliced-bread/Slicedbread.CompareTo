namespace Slicedbread.CompareTo
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class CompareToExtensions
    {
        public static Comparison CompareTo<T>(this T originalObject, T newObject)
        {
            var result = new Comparison();

            result.AddRange(Compare(typeof(T), originalObject, newObject, string.Empty));

            return result;
        }

        private static IEnumerable<PropertyComparison> Compare(Type type, object originalObject, object newObject, string classPrefix)
        {
            var results = new List<PropertyComparison>();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanRead)
                    continue;

                var originalValue = property.GetValue(originalObject);
                var newValue = property.GetValue(newObject);

                if (property.PropertyType.IsClass)
                    results.AddRange(Compare(property.PropertyType, originalValue, newValue, property.Name + "."));
                else
                {
                    var hasChanged = !originalValue.Equals(newValue);

                    results.Add(new PropertyComparison
                    {
                        PropertyName = classPrefix + property.Name,
                        OriginalValue = originalValue,
                        NewValue = newValue,
                        PropertyType = property.PropertyType,
                        HasChanged = hasChanged
                    });
                }
            }

            return results;
        }
    }
}
