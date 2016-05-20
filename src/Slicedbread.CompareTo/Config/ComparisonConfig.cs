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

        public List<PropertyInfo> GetIgnoreList()
        {
            return IgnoreList;
        }

        public dynamic GetKeyPropertyAccessorForCollection(Type type)
        {
            if (CollectionIdentifiers.ContainsKey(type))
                return CollectionIdentifiers[type];

            return null;
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
        public CollectionComparisonConfig<T, TCollectionType> CompareCollection<TCollectionType>()
        {
            return new CollectionComparisonConfig<T, TCollectionType>(this);
        }

        public Comparison Compare()
        {
            return new ComparisonEngine().Compare(_originalObject, _newObject, this);
        }
        
        public void AddCollectionComparison<TCollectionType, TProp>(Type type, Func<TCollectionType, TProp> keyAccessorExpression)
        {
            CollectionIdentifiers.Add(type, keyAccessorExpression);
        }
    }
}