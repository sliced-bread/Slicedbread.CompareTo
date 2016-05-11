namespace Slicedbread.CompareTo
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Models;
    using PropertyComparers;

    internal class ComparisonEngine
    {
        private static readonly IPropertyComparer[] PropertyComparers = {
            new ValueTypeComparer(),
            new IComparableComparer(),
            new ClassComparer(),
        };

        public Comparison CompareTo<T>(T originalObject, T newObject)
        {
            return CompareTo(typeof(T), originalObject, newObject);
        }

        public Comparison CompareTo(Type type, object originalObject, object newObject)
        {
            var result = new Comparison();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanRead)
                    continue;

                var comparer = PropertyComparers.FirstOrDefault(c => c.CanCompare(property));
                if (comparer != null)
                    result.AddRange(comparer.Compare(property, originalObject, newObject));
            }

            return result;
        }
    }
}