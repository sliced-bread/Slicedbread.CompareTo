namespace Slicedbread.CompareTo
{
    using Models;

    public static class CompareToExtensions
    {
        private static readonly ComparisonEngine ComparisonEngine;

        static CompareToExtensions()
        {
            ComparisonEngine = new ComparisonEngine();
        }
        
        public static Comparison CompareTo<T>(this T originalObject, T newObject)
        {
            return ComparisonEngine.CompareTo(originalObject, newObject);
        }
    }
}
