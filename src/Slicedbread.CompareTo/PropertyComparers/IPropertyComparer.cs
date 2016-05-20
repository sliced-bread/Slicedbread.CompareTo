namespace Slicedbread.CompareTo.PropertyComparers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Config;
    using Models;

    public interface IPropertyComparer
    {
        bool CanCompare(PropertyInfo property);
        IEnumerable<IDifference> Compare<T>(PropertyInfo property, T originalObject, T newObject, ComparisonConfig config);
    }
}
