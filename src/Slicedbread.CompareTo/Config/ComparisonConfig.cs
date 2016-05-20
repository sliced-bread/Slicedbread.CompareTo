namespace Slicedbread.CompareTo.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Engine;
    using Models;

    public class ComparisonConfig
    {
        protected readonly List<PropertyInfo> IgnoreList;
        protected readonly Dictionary<Type, dynamic> CollectionIdentifiers;

        public ComparisonConfig()
        { 
            CollectionIdentifiers = new Dictionary<Type, dynamic>();
            IgnoreList = new List<PropertyInfo>();
        }

        internal List<PropertyInfo> GetIgnoreList()
        {
            return IgnoreList;
        }

        internal dynamic GetKeyPropertyAccessorForCollection(Type type)
        {
            return CollectionIdentifiers.ContainsKey(type) 
                ? CollectionIdentifiers[type] 
                : null;
        }
    }

    public class ComparisonConfig<T> : ComparisonConfig
    {
        private readonly T _originalObject;
        private readonly T _newObject;
        
        public ComparisonConfig(T originalObject, T newObject) : base()
        {
            _originalObject = originalObject;
            _newObject = newObject;
        }

        public ComparisonConfig<T> Ignore<TPropType>(Expression<Func<T, TPropType>> func)
        {
            var property = func.Body as MemberExpression;

            if (property == null)
                throw new ArgumentException("Cannot ignore '" + func + "'. Ignore expressions must point to a property.");

            var info = property.Member as PropertyInfo;

            IgnoreList.Add(info);

            return this;
        }

        /// <summary>
        /// Sets up the configuration for comparing a collection of a given Type
        /// </summary>
        public CollectionComparisonConfig<T, TCollectionType> CompareCollection<TCollectionType>()
        {
            return new CollectionComparisonConfig<T, TCollectionType>(this);
        }

        /// <summary>
        /// Executes the comparison
        /// </summary>
        public Comparison Compare()
        {
            return new ComparisonEngine().Compare(_originalObject, _newObject, this);
        }
        
        internal void AddCollectionComparison<TCollectionType, TProp>(Type type, Func<TCollectionType, TProp> keyAccessorExpression)
        {
            CollectionIdentifiers.Add(type, keyAccessorExpression);
        }
    }
}