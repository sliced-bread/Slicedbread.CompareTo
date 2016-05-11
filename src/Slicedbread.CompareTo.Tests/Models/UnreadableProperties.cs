namespace Slicedbread.CompareTo.Tests.Models
{
    internal class UnreadablePropertyContainer
    {
        private string _cannotRead;
        public string CannotRead { set { _cannotRead = value; } }
    }
}