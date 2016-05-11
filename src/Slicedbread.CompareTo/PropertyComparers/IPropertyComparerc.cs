namespace Slicedbread.CompareTo.PropertyComparers
{
    using System.Collections.Generic;
    using System.Reflection;
    using Models;

    public interface IPropertyComparer
    {
        bool CanCompare(PropertyInfo property);
        IEnumerable<Difference> Compare<T>(PropertyInfo property, T originalObject, T newObject);
    }
}
