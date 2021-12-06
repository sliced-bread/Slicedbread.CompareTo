using System.Text;

namespace Slicedbread.CompareTo.Models
{
    using System.Collections.Generic;

    public class Comparison : List<IDifference>
    {
        public IDifference this[string propertyName]
        {
            get { return base.Find(c => c.PropertyName == propertyName); }
        }

        public string GetShortDescription(int maxChangesToInclude = 3)
        {
            var descBuilder = new StringBuilder();

            var added = 0;
            foreach (var change in this)
            {
                var desc = change.ToString();

                // Separator + lowercase first character
                if (added > 0 && desc.Length > 0)
                    desc = ", " + desc.Substring(0, 1).ToLowerInvariant() + desc.Substring(1);

                descBuilder.Append(desc);

                if (++added >= maxChangesToInclude) break;
            }

            if (added < this.Count)
            {
                var more = this.Count - added;
                descBuilder.Append($" and {more} other change{(more > 1 ? "s" : "")}");
            }
            return descBuilder.ToString();
        }
    }
}