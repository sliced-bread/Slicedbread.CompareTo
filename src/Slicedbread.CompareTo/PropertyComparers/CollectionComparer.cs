namespace Slicedbread.CompareTo.PropertyComparers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Config;
    using Models;

    public class CollectionComparer : IPropertyComparer
    {
        public bool CanCompare(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable));
        }

        public IEnumerable<IDifference> Compare<T>(PropertyInfo property, T originalObject, T newObject, ComparisonConfig config)
        {
            var comparison =  new List<IDifference>();

            var originalValue = (IEnumerable)property.GetValue(originalObject, new object[0]);
            var newValue = (IEnumerable)property.GetValue(newObject, new object[0]);

            var baseType = property.PropertyType.IsArray
                ? property.PropertyType.GetElementType()
                : property.PropertyType.GetGenericArguments().Single();
            

            //var propertyInfo = config.GetKeyPropertyForCollection(property.PropertyType.GetGenericArguments()[0]);

            var method = this.GetType().GetMethod("CompareInternal").MakeGenericMethod(baseType);

            return (IEnumerable<IDifference>)method.Invoke(this, new object[]{ property, originalValue, newValue});
        }


        public IEnumerable<IDifference> CompareInternal<T>(PropertyInfo property, IEnumerable<T> oldCOllection, IEnumerable<T> newCollection)
        {
            var oldItems = oldCOllection.ToList();
            var newItems = newCollection.ToList();

            var added = newItems.Where(c => !oldItems.Contains(c));
            var removed = oldItems.Where(c => !newItems.Contains(c));

            var diffs = new List<IDifference>();

            diffs.AddRange(added.Select(i => new CollectionDifference(property.PropertyType, property.Name, null, i)));
            diffs.AddRange(removed.Select(i => new CollectionDifference(property.PropertyType, property.Name, i, null)));

            return diffs;
        }
    }
}