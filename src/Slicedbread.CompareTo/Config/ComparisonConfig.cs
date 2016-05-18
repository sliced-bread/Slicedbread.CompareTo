namespace Slicedbread.CompareTo.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Engine;
    using Models;

    public class ComparisonConfig<T>
    {
        private readonly T _originalObject;
        private readonly T _newObject;
        private readonly ComparisonEngine _engine;
        private readonly List<PropertyInfo> _ignoreList;

        public ComparisonConfig(T originalObject, T newObject)
        {
            _originalObject = originalObject;
            _newObject = newObject;
            _ignoreList = new List<PropertyInfo>();
        }

        public ComparisonConfig<T> Ignore<TPropType>(Expression<Func<T, TPropType>> func)
        {
            var property = func.Body as MemberExpression;

            if (property == null)
                throw new ArgumentException("Cannot ignore '" + func + "'. Ignore expressions must point to a property.");

            var info = property.Member as PropertyInfo;

            _ignoreList.Add(info);

            return this;

        }

        public Comparison Compare()
        {
            return new ComparisonEngine().Compare(_originalObject, _newObject, _ignoreList);
        }
    }
}