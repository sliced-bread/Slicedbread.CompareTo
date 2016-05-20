namespace Slicedbread.CompareTo
{
    using Config;
    using Engine;
    using Models;

    public static class CompareToExtensions
    {
        /// <summary>
        /// Compares two objects, returning the differences
        /// </summary>
        public static Comparison CompareTo<T>(this T originalObject, T newObject)
        {
            return new ComparisonEngine().Compare(originalObject, newObject, new ComparisonConfig());
        }

        /// <summary>
        /// Sets up the configuration for a future comparison
        /// </summary>
        /// <returns>A ComparisonConfig object, on which CompareTo can be called 
        /// to instigate the configured comparison</returns>
        public static ComparisonConfig<T> ConfigureCompareTo<T>(this T originalObject, T newObject)
        {
            return new ComparisonConfig<T>(originalObject, newObject);
        }
    }
}
