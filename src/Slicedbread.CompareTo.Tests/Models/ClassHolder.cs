namespace Slicedbread.CompareTo.Tests.Models
{
    internal class ClassHolder
    {
        public ClassExample Child { get; set; }
    }

    internal class ClassExample
    {
        public string Prop { get; private set; }

        public ClassExample()
        {
            
        }

        public ClassExample(string prop)
        {
            Prop = prop;
        }
    }
}