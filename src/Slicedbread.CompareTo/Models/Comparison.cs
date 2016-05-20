namespace Slicedbread.CompareTo.Models
{
    using System.Collections.Generic;

    public class Comparison : List<IDifference>
    {
        public IDifference this[string propertyName]
        {
            get { return base.Find(c => c.PropertyName == propertyName); }
        }
    }
}