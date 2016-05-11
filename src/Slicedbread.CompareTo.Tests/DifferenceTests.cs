namespace Slicedbread.CompareTo.Tests
{
    using CompareTo.Models;
    using Shouldly;
    using Xunit;

    public class DifferenceTests
    {
        [Fact]
        public void Returns_Difference_As_String()
        {
            // Given
            var diff = new Difference
            {
                PropertyName = "MyProperty",
                OriginalValue = 789,
                NewValue = 999
            };

            // When
            var diffString = diff.ToString();

            // Then
            diffString.ShouldBe("'MyProperty' changed from '789' to '999'");
        }
    }
}
