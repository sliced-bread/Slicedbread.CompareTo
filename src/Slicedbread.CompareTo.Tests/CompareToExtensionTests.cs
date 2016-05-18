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
        public void Compares_Anonymous_Types()
        {
            // Given
            var item1 = new
            {
                MyProp = "Hello"
            };

            var item2 = new
            {
                MyProp = "World"
            };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison["MyProp"].ToString().ShouldBe("'MyProp' changed from 'Hello' to 'World'");
        }

        [Fact]
        public void Returns_Differences_Between_ValueTypes()
        {
            // Given
            var originalObject = new
            {
                IntegerProperty = 999,
                DateTimeProperty = new DateTime(2016, 01, 01)
            };

            var newObject = new
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
            var originalObject = new
            {
                Child = new { IntegerProperty = 999 }
            };

            var newObject = new
            {
                Child = new { IntegerProperty = 123 }
            };

            // When
            var result = originalObject.CompareTo(newObject);

            // Then
            result["Child.IntegerProperty"].OriginalValue.ShouldBe(999);
            result["Child.IntegerProperty"].NewValue.ShouldBe(123);
            result["Child.IntegerProperty"].PropertyType.ShouldBe(typeof(int));
        }

        [Fact]
        public void Does_Not_Throw_When_Original_Value_Type_Property_Is_Null()
        {
            // Given
            var item1 = new { Prop = default(int?) };
            var item2 = new { Prop = new int?(123) };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("'Prop' changed from '' to '123'");
        }

        [Fact]
        public void Does_Not_Throw_When_New_Value_Type_Property_Is_Null()
        {
            // Given
            var item1 = new { Prop = new int?(123) };
            var item2 = new { Prop = default(int?) };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("'Prop' changed from '123' to ''");
        }

        [Fact]
        public void Compares_Two_Null_Value_Type_Properties()
        {
            // Given
            var item1 = new { Prop = default(int?) };
            var item2 = new { Prop = default(int?) };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.ShouldBeEmpty();
        }

        [Fact]
        public void Does_Not_Throw_When_Original_IComparable_Is_Null()
        {
            // Given
            var item1 = new ComparableHolder();
            var item2 = new ComparableHolder { ComparableThing = new ComparableThing(999) };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("'ComparableThing' changed from '' to '999'");
        }

        [Fact]
        public void Does_Not_Throw_When_New_IComparable_Is_Null()
        {
            // Given
            var item1 = new ComparableHolder { ComparableThing = new ComparableThing(999) };
            var item2 = new ComparableHolder();

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("'ComparableThing' changed from '999' to ''");
        }

        [Fact]
        public void Compares_Two_Null_IComparables_And_Finds_No_Differences()
        {
            // Given
            var item1 = new ComparableHolder();
            var item2 = new ComparableHolder();

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.ShouldBeEmpty();
        }

        [Fact]
        public void Ignores_Properties_As_Configured()
        {
            // Given
            var item1 = new { Prop1 = "Hello", Prop2 = "World" };
            var item2 = new { Prop1 = "Foo", Prop2 = "Bar" };

            // When
            var comparisonConfig = item1.ConfigureCompareTo(item2)
                .Ignore(x => x.Prop1);

            var comparison = comparisonConfig.Compare();

            // Then
            comparison.Count().ShouldBe(1);
            comparison.First().ToString().ShouldContain("'Prop2'");
        }

        [Fact]
        public void Ignores_Properties_In_Nested_Types()
        {
            // Given
            var item1 = new
            {
                Prop1 = "Hello",
                Prop2 = new
                {
                    NestedOne = "NestedItem1",
                    NestedTwo = "NestedItem1",
                }
            };
            var item2 = new
            {
                Prop1 = "Foo",
                Prop2 = new
                {
                    NestedOne = "NestedItem2",
                    NestedTwo = "NestedItem2",
                }
            };

            // When
            var comparisonConfig = item1.ConfigureCompareTo(item2)
                .Ignore(x => x.Prop2.NestedOne);

            var comparison = comparisonConfig.Compare();

            // Then
            comparison.Count().ShouldBe(2);
            comparison.Any(c => c.ToString().Contains("'Prop1'")).ShouldBeTrue();
            comparison.Any(c => c.ToString().Contains("'Prop2.NestedTwo'")).ShouldBeTrue();
        }

        [Fact]
        public void Throws_When_Attempting_To_Ignore_A_Method()
        {
            // Given
            var item1 = new { Prop1 = "Hello", Prop2 = "World" };
            var item2 = new { Prop1 = "Foo", Prop2 = "Bar" };

            // When
            var ex = Record.Exception(() =>
                item1.ConfigureCompareTo(item2)
                    .Ignore(x => x.ToString())
                );

            // Then
            ex.ShouldBeOfType<ArgumentException>();
            ex.Message.ShouldBe("Cannot ignore 'x => x.ToString()'. Ignore expressions must point to a property.");
        }
    }
}
