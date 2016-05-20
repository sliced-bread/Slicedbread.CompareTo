namespace Slicedbread.CompareTo.Config
{
    using System;
    using System.Linq.Expressions;

    public class CollectionComparisonConfig<TComparisonType, TCollectionType>
    {
        private readonly ComparisonConfig<TComparisonType> _comparisonConfig;

        public CollectionComparisonConfig(ComparisonConfig<TComparisonType> comparisonConfig)
        {
            _comparisonConfig = comparisonConfig;
        }
        
        public ComparisonConfig<TComparisonType> UsingPropertyAsKey<TProp>(Expression<Func<TCollectionType, TProp>> func)
        {
            _comparisonConfig.AddCollectionComparison(typeof(TCollectionType), func.Compile());

            return _comparisonConfig;
        }
            
    }
}