namespace Slicedbread.CompareTo.Tests.Models
{
    using System;

    public class ExceptionThrowingItem
    {
        public string Thing
        {
            get
            {
                throw new Exception("Bang");
            }
            set
            {
                
            }
        }
    }
}