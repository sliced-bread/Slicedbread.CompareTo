namespace Slicedbread.CompareTo.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Comparison : IEnumerable<Difference>
    {
        private readonly List<Difference> _differences;

        public Comparison()
        {
            _differences = new List<Difference>();
        }

        public Difference this[string propertyName]
        {
            get { return _differences.FirstOrDefault(c => c.PropertyName == propertyName); }
        }
        
        public void AddRange(IEnumerable<Difference> comparison)
        {
            _differences.AddRange(comparison);
        }

        IEnumerator<Difference> IEnumerable<Difference>.GetEnumerator()
        {
            return _differences.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _differences.GetEnumerator();
        }
    }
}