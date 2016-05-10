namespace Slicedbread.CompareTo.Tests
{
    using System;
    using Models;
    using Shouldly;
    using Xunit;

    public class CompareToExtensionTests
    {
        [Fact]
        public void Returns_Comparison_Between_ValueTypes()
        {
            // Given
            var originalObject = new SimpleValueTypePoco
            {
                IntegerProperty = 999,
                DateTimeProperty = new DateTime(2016, 01, 01)
            };

            var newObject = new SimpleValueTypePoco
            {
                IntegerProperty = 666,
                DateTimeProperty = new DateTime(2016, 01, 01)
            };

            // When
            var result = originalObject.CompareTo(newObject);

            // Then
            result["IntegerProperty"].OriginalValue.ShouldBe(999);
            result["IntegerProperty"].NewValue.ShouldBe(666);
            result["IntegerProperty"].PropertyType.ShouldBe(typeof(int));
            result["IntegerProperty"].HasChanged.ShouldBeTrue();

            result["DateTimeProperty"].OriginalValue.ShouldBe(new DateTime(2016, 01, 01));
            result["DateTimeProperty"].NewValue.ShouldBe(new DateTime(2016, 01, 01));
            result["DateTimeProperty"].PropertyType.ShouldBe(typeof(DateTime));
            result["DateTimeProperty"].HasChanged.ShouldBeFalse();
        }

        [Fact]
        public void Does_Not_Read_Properties_Where_CanRead_Is_False()
        {
            // Given
            var originalObject = new SimpleValueTypePoco();
            var newObject = new SimpleValueTypePoco();

            // When
            var result = Record.Exception(() => originalObject.CompareTo(newObject));

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void MethodName()
        {
            // Given
            var originalObject = new NestedTypeParent { Child = new NestedTypeChild { IntegerProperty = 999 }};
            var newObject = new NestedTypeParent { Child = new NestedTypeChild { IntegerProperty = 123 }};

            // When
            var result = originalObject.CompareTo(newObject);

            // Then
            result["Child.IntegerProperty"].OriginalValue.ShouldBe(999);
            result["Child.IntegerProperty"].NewValue.ShouldBe(123);
            result["Child.IntegerProperty"].PropertyType.ShouldBe(typeof(int));
            result["Child.IntegerProperty"].HasChanged.ShouldBeTrue();
        }
    }
}
