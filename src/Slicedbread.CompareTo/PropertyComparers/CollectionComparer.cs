namespace Slicedbread.CompareTo.PropertyComparers
{
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

            // Call the generic method to compare the collections.
            // This allows us to use strong typing
            var method = this
                .GetType()
                .GetMethod("CompareInternal", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(baseType);

            return (IEnumerable<IDifference>)method.Invoke(this, new object[]{ property, originalValue, newValue, func});
        }
        
        private IEnumerable<IDifference> CompareInternal<T>(PropertyInfo property, IEnumerable<T> oldCollection, IEnumerable<T> newCollection, dynamic keyAccessorFunc)
        {
            var oldItems = oldCollection?.ToList() ?? new List<T>();
            var newItems = newCollection?.ToList() ?? new List<T>();

            return keyAccessorFunc != null 
                ? CompareCollectionByKey(property, keyAccessorFunc, oldItems, newItems) 
                : CompareCollectionByReference(property, oldItems, newItems);
        }

        private static IEnumerable<IDifference> CompareCollectionByKey<T>(PropertyInfo property, dynamic keyAccessorFunc, List<T> oldItems, List<T> newItems)
        {
            var diffs = new List<IDifference>();

            var addedItems = newItems.Where(newItem =>
                !oldItems.Any(oldItem => keyAccessorFunc.Invoke(oldItem).Equals(keyAccessorFunc.Invoke(newItem)))
                ).ToList();

            var removedItems = oldItems.Where(newItem =>
                !newItems.Any(oldItem => keyAccessorFunc.Invoke(oldItem).Equals(keyAccessorFunc.Invoke(newItem)))
                ).ToList();

            var matchingItems = oldItems.Where(newItem =>
                newItems.Any(oldItem => keyAccessorFunc.Invoke(oldItem).Equals(keyAccessorFunc.Invoke(newItem)))
                ).ToList();

            diffs.AddRange(addedItems.Select(i => new CollectionDifference(property.PropertyType, property.Name, null, i)));
            diffs.AddRange(removedItems.Select(i => new CollectionDifference(property.PropertyType, property.Name, i, null)));

            foreach (var oldItem in matchingItems)
            {
                var id = keyAccessorFunc.Invoke(oldItem);

                var matchingItem = newItems.FirstOrDefault(newItem => keyAccessorFunc.Invoke(newItem).Equals(id));
                var equal = new ComparisonEngine().Compare(oldItem, matchingItem, new ComparisonConfig()).Count == 0;

                if (equal)
                    continue;

                diffs.Add(new CollectionDifference(property.PropertyType, property.Name, oldItem, matchingItem));
            }

            return diffs;
        }

        private static IEnumerable<IDifference> CompareCollectionByReference<T>(PropertyInfo property, IList<T> oldItems, IList<T> newItems)
        {
            var diffs = new List<IDifference>();
            
            var added = newItems.Where(c => !oldItems.Contains(c));
            var removed = oldItems.Where(c => !newItems.Contains(c));

            diffs.AddRange(added.Select(i => new CollectionDifference(property.PropertyType, property.Name, null, i)));
            diffs.AddRange(removed.Select(i => new CollectionDifference(property.PropertyType, property.Name, i, null)));

            return diffs;
        }
    }
}