namespace Slicedbread.CompareTo
{
    using Config;
    using Engine;
    using Models;

    public static class CompareToExtensions
    {
        public static Comparison CompareTo<T>(this T originalObject, T newObject)
        {
            return new ComparisonEngine().Compare(originalObject, newObject, new ComparisonConfig());
        }

        public static ComparisonConfig<T> ConfigureCompareTo<T>(this T originalObject, T newObject)
        {
            return new ComparisonConfig<T>(originalObject, newObject);
        }
    }
}
