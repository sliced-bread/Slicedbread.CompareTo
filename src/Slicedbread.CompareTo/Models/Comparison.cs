namespace Slicedbread.CompareTo.Models
{
    using System.Collections.Generic;

    public class Comparison : List<Difference>
    {
        public Difference this[string propertyName]
        {
            get { return base.Find(c => c.PropertyName == propertyName); }
        }
    }
}