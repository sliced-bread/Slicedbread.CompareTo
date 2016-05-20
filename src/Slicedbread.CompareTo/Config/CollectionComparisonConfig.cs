namespace Slicedbread.CompareTo.Config
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class CollectionComparisonConfig<TComparisonType, TCollectionType>
    {
        private readonly ComparisonConfig<TComparisonType> _comparisonConfig;

        public CollectionComparisonConfig(ComparisonConfig<TComparisonType> comparisonConfig)
        {
            _comparisonConfig = comparisonConfig;
        }
        
        public ComparisonConfig<TComparisonType> UsingPropertyAsKey<TProp>(Expression<Func<TCollectionType, TProp>> func)
        {
            var property = func.Body as MemberExpression;

            //if (property == null)
            //    throw new ArgumentException("Cannot ignore '" + func + "'. Ignore expressions must point to a property.");

            var info = property.Member as PropertyInfo;

            _comparisonConfig.AddCollectionComparison(typeof(TCollectionType), info);

            return _comparisonConfig;
        }
            
    }
}