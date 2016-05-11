namespace Slicedbread.CompareTo.Tests
{
    using System;
    using System.Linq;
    using Models;
    using Shouldly;
    using Xunit;

    public class CompareToExtensionTests
    {
        [Fact]
        public void Returns_Differences_Between_ValueTypes()
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

            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Returns_Differences_Between_IComparables()
        {
            // Given
            var originalObject = new ComparableHolder
            {
                ComparableThing = new ComparableThing(123),
                AnotherComparableThing = new ComparableThing(999)
            };

            var newObject = new ComparableHolder
            {
                ComparableThing = new ComparableThing(456),
                AnotherComparableThing = new ComparableThing(999)
            };

            // When
            var result = originalObject.CompareTo(newObject);

            // Then
            result["ComparableThing"].OriginalValue.ShouldBe(originalObject.ComparableThing);
            result["ComparableThing"].NewValue.ShouldBe(newObject.ComparableThing);
            result["ComparableThing"].PropertyType.ShouldBe(typeof(ComparableThing));

            result.Count().ShouldBe(1);
        }
        
        [Fact]
        public void Does_Not_Read_Properties_Where_CanRead_Is_False()
        {
            // Given
            var originalObject = new UnreadablePropertyContainer();
            var newObject = new UnreadablePropertyContainer();

            // When
            var result = Record.Exception(() => originalObject.CompareTo(newObject));

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Compares_Nested_Classes()
        {
            // Given
            var originalObject = new NestedTypeParent
            {
                Child = new NestedTypeChild { IntegerProperty = 999 }
            };

            var newObject = new NestedTypeParent
            {
                Child = new NestedTypeChild { IntegerProperty = 123 }
            };

            // When
            var result = originalObject.CompareTo(newObject);

            // Then
            result["Child.IntegerProperty"].OriginalValue.ShouldBe(999);
            result["Child.IntegerProperty"].NewValue.ShouldBe(123);
            result["Child.IntegerProperty"].PropertyType.ShouldBe(typeof(int));
        }
    }
}
