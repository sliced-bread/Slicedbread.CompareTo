namespace Slicedbread.CompareTo.Engine
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Config;
    using Models;
    using PropertyComparers;

    internal class ComparisonEngine
    {
        private static readonly IPropertyComparer[] PropertyComparers = {
            new ValueTypeComparer(),
            new IComparableComparer(),
            new CollectionComparer(),
            new ClassComparer(),
        };

        public Comparison Compare<T>(T originalObject, T newObject, ComparisonConfig config)
        {
            return Compare(typeof(T), originalObject, newObject, config);
        }

        public Comparison Compare(Type type, object originalObject, object newObject, ComparisonConfig config)
        {
            var result = new Comparison();

            var ignoreList = config.GetIgnoreList();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (ignoreList != null && ignoreList.Contains(property))
                    continue;

                if (!property.CanRead)
                    continue;

                var comparer = PropertyComparers.FirstOrDefault(c => c.CanCompare(property));
                if (comparer != null)
                    result.AddRange(comparer.Compare(property, originalObject, newObject, config));
            }

            return result;
        }
    }
}