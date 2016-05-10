namespace Slicedbread.CompareTo
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Comparison : IEnumerable
    {
        private readonly List<PropertyComparison> _comparisons;

        public Comparison()
        {
            _comparisons = new List<PropertyComparison>();
        }

        public PropertyComparison this[string propertyName]
        {
            get { return _comparisons.FirstOrDefault(c => c.PropertyName == propertyName); }
        }

        public void Add(PropertyComparison comparison)
        {
            _comparisons.Add(comparison);
        }

        public void AddRange(IEnumerable<PropertyComparison> comparison)
        {
            _comparisons.AddRange(comparison);
        }

        public IEnumerator GetEnumerator()
        {
            return _comparisons.GetEnumerator();
        }
    }
}