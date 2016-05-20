namespace Slicedbread.CompareTo.PropertyComparers
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Config;
    using Engine;
    using Models;

    public class CollectionComparer : IPropertyComparer
    {
        public bool CanCompare(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable));
        }

        public IEnumerable<IDifference> Compare<T>(PropertyInfo property, T originalObject, T newObject, ComparisonConfig config)
        {
            var originalValue = (IEnumerable)property.GetValue(originalObject, new object[0]);
            var newValue = (IEnumerable)property.GetValue(newObject, new object[0]);

            var baseType = property.PropertyType.IsArray
                ? property.PropertyType.GetElementType()
                : property.PropertyType.GetGenericArguments().Single();
            

            var func = config.GetKeyPropertyAccessorForCollection(baseType);

            var method = this.GetType().GetMethod("CompareInternal").MakeGenericMethod(baseType);

            return (IEnumerable<IDifference>)method.Invoke(this, new object[]{ property, originalValue, newValue, func});
        }


        public IEnumerable<IDifference> CompareInternal<T>(PropertyInfo property, IEnumerable<T> oldCOllection, IEnumerable<T> newCollection, dynamic func)
        {
            var oldItems = oldCOllection.ToList();
            var newItems = newCollection.ToList();

            var added = newItems.Where(c => !oldItems.Contains(c));
            var removed = oldItems.Where(c => !newItems.Contains(c));

            var diffs = new List<IDifference>();



            if (func != null)
            {

                var addedItems = newItems.Where(newItem => 
                    !oldItems.Any(oldItem => func.Invoke(oldItem).Equals(func.Invoke(newItem)))
                ).ToList();

                var removedItems = oldItems.Where(newItem =>
                    !newItems.Any(oldItem => func.Invoke(oldItem).Equals(func.Invoke(newItem)))
                ).ToList();

                var matchingItems = oldItems.Where(newItem =>
                    newItems.Any(oldItem => func.Invoke(oldItem).Equals(func.Invoke(newItem)))
                    ).ToList();

                diffs.AddRange(addedItems.Select(i => new CollectionDifference(property.PropertyType, property.Name, null, i)));
                diffs.AddRange(removedItems.Select(i => new CollectionDifference(property.PropertyType, property.Name, i, null)));

                foreach (var oldItem in matchingItems)
                {
                    var id = func.Invoke(oldItem);

                    var matchingItem = newItems.FirstOrDefault(newItem => func.Invoke(newItem).Equals(id));
                    var equal = new ComparisonEngine().Compare(oldItem, matchingItem, new ComparisonConfig()).Count == 0;

                    if (!equal)
                        diffs.Add(new CollectionDifference(property.PropertyType, property.Name, oldItem, matchingItem));
                }
            }
            else
            {
                diffs.AddRange(added.Select(i => new CollectionDifference(property.PropertyType, property.Name, null, i)));
                diffs.AddRange(removed.Select(i => new CollectionDifference(property.PropertyType, property.Name, i, null)));
            }

            return diffs;
        }
    }
}