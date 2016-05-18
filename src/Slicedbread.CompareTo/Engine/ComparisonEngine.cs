namespace Slicedbread.CompareTo.Engine
{
    using System;
    using System.Collections.Generic;
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

        public Comparison Compare<T>(T originalObject, T newObject, IList<PropertyInfo> ignoreList = null)
        {
            return Compare(typeof(T), originalObject, newObject, ignoreList);
        }

        public Comparison Compare(Type type, object originalObject, object newObject, IList<PropertyInfo> ignoreList = null)
        {
            var result = new Comparison();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (ignoreList != null && ignoreList.Contains(property))
                    continue;

                if (!property.CanRead)
                    continue;

                var comparer = PropertyComparers.FirstOrDefault(c => c.CanCompare(property));
                if (comparer != null)
                    result.AddRange(comparer.Compare(property, originalObject, newObject, ignoreList));
            }

            return result;
        }
    }
}