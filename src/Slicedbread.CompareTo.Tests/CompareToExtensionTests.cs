﻿namespace Slicedbread.CompareTo.Tests
{
    using System;
    using System.Collections.Generic;
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
        public void Returns_Correct_Text_When_Moving_From_From_Null_To_Value()
        {
            // Given
            var item1 = new { Prop = default(int?) };
            var item2 = new { Prop = new int?(123) };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("Set 'Prop' to '123'");
        }

        [Fact]
        public void Returns_Correct_Text_When_Moving_From_From_Value_To_Null()
        {
            // Given
            var item1 = new { Prop = new int?(123) };
            var item2 = new { Prop = default(int?) };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("Removed 'Prop' (value was previously '123')");
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
        public void Does_Not_Throw_When_New_Value_Type_Property_Is_Null()
        {
            // Given
            var item1 = new { Prop = new int?(123) };
            var item2 = new { Prop = default(int?) };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("Removed 'Prop' (value was previously '123')");
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
            comparison.First().ToString().ShouldBe("Set 'ComparableThing' to '999'");
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
            comparison.First().ToString().ShouldBe("Removed 'ComparableThing' (value was previously '999')");
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
            comparison.Count.ShouldBe(2);
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

        [Fact]
        public void Matches_Collections_By_Reference_If_No_Key_Configured()
        {
            // Given
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var item1 = new CollectionItem(id1, "Hello");
            var item2 = new CollectionItem(id2, "Foo");
            var item3 = new CollectionItem(Guid.NewGuid(), "RemoveMe");
            var item4 = new CollectionItem(Guid.NewGuid(), "AddMe");

            var collection1 = new
            {
                SomeList = new[] { item1, item2, item3 }
            };

            var collection2 = new
            {
                SomeList = new[] { item1, item2, item4 }
            };

            // When
            var config = collection1.ConfigureCompareTo(collection2);

            var comparison = config.Compare();

            // Then
            comparison.Count.ShouldBe(2);
            comparison.ShouldContain(i => i.ToString() == "'RemoveMe' was removed from 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'AddMe' was added to 'SomeList'");
        }

        [Fact]
        public void Matches_Array_By_Key_If_Configured()
        {
            // Given
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var collection1 = new
            {
                SomeList = new[]
                {
                    new CollectionItem(id1, "Hello"),
                    new CollectionItem(id2, "Foo"),
                    new CollectionItem(Guid.NewGuid(), "RemoveMe")
                }
            };

            var collection2 = new
            {
                SomeList = new[]
                {
                    new CollectionItem(id1, "World"),
                    new CollectionItem(id2, "Foo"),
                    new CollectionItem(Guid.NewGuid(), "AddMe")
                }
            };

            // When
            var config = collection1.ConfigureCompareTo(collection2)
                .CompareCollection<CollectionItem>()
                .UsingPropertyAsKey(c => c.Id);

            var comparison = config.Compare();

            // Then
            comparison.Count.ShouldBe(3);
            comparison.ShouldContain(i => i.ToString() == "'Hello' changed to 'World' in 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'RemoveMe' was removed from 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'AddMe' was added to 'SomeList'");
        }

        [Fact]
        public void Matches_IEnumerable_By_Key_If_Configured()
        {
            // Given
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var collection1 = new
            {
                SomeList = new List<CollectionItem>
                {
                    new CollectionItem(id1, "Hello"),
                    new CollectionItem(id2, "Foo"),
                    new CollectionItem(Guid.NewGuid(), "RemoveMe")
                }
            };

            var collection2 = new
            {
                SomeList = new List<CollectionItem>
                {
                    new CollectionItem(id1, "World"),
                    new CollectionItem(id2, "Foo"),
                    new CollectionItem(Guid.NewGuid(), "AddMe")
                }
            };

            // When
            var config = collection1.ConfigureCompareTo(collection2)
                .CompareCollection<CollectionItem>()
                .UsingPropertyAsKey(c => c.Id);

            var comparison = config.Compare();

            // Then
            comparison.Count.ShouldBe(3);
            comparison.ShouldContain(i => i.ToString() == "'Hello' changed to 'World' in 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'RemoveMe' was removed from 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'AddMe' was added to 'SomeList'");
        }

        [Fact]
        public void Matches_IEnumerable_By_Method_Used_As_Key()
        {
            // Given
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var collection1 = new
            {
                SomeList = new List<CollectionItem>
                {
                    new CollectionItem(id1, "Hello"),
                    new CollectionItem(id2, "Foo"),
                    new CollectionItem(Guid.NewGuid(), "RemoveMe")
                }
            };

            var collection2 = new
            {
                SomeList = new List<CollectionItem>
                {
                    new CollectionItem(id1, "World"),
                    new CollectionItem(id2, "Foo"),
                    new CollectionItem(Guid.NewGuid(), "AddMe")
                }
            };

            // When
            var config = collection1.ConfigureCompareTo(collection2)
                .CompareCollection<CollectionItem>()
                .UsingPropertyAsKey(c => c.GetId());

            var comparison = config.Compare();

            // Then
            comparison.Count.ShouldBe(3);
            comparison.ShouldContain(i => i.ToString() == "'Hello' changed to 'World' in 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'RemoveMe' was removed from 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'AddMe' was added to 'SomeList'");
        }

        [Fact]
        public void Treats_Null_Original_Collection_As_Empty()
        {
            // Given
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var collection1 = new
            {
                SomeList = default(List<CollectionItem>)
            };

            var collection2 = new
            {
                SomeList = new List<CollectionItem>
                {
                    new CollectionItem(id1, "Hello"),
                    new CollectionItem(id2, "World")
                }
            };

            // When
            var config = collection1.ConfigureCompareTo(collection2)
                .CompareCollection<CollectionItem>()
                .UsingPropertyAsKey(c => c.GetId());

            var comparison = config.Compare();

            // Then
            comparison.Count.ShouldBe(2);
            comparison.ShouldContain(i => i.ToString() == "'Hello' was added to 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'World' was added to 'SomeList'");
        }

        [Fact]
        public void Treats_Null_New_Collection_As_Empty()
        {
            // Given
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            var collection1 = new
            {
                SomeList = new List<CollectionItem>
                {
                    new CollectionItem(id1, "Hello"),
                    new CollectionItem(id2, "World")
                }
            };

            var collection2 = new
            {
                SomeList = default(List<CollectionItem>)
            };

            // When
            var config = collection1.ConfigureCompareTo(collection2)
                .CompareCollection<CollectionItem>()
                .UsingPropertyAsKey(c => c.GetId());

            var comparison = config.Compare();

            // Then
            comparison.Count.ShouldBe(2);
            comparison.ShouldContain(i => i.ToString() == "'Hello' was removed from 'SomeList'");
            comparison.ShouldContain(i => i.ToString() == "'World' was removed from 'SomeList'");
        }

        [Fact]
        public void Swallows_Exceptions_If_Configured()
        {
            // Given
            var thing1 = new
            {
                Prop1 = "Foo",
                ExplodingThing = new ExceptionThrowingItem(),
                Prop2 = "Hello",
            };

            var thing2 = new
            {
                Prop1 = "Bar",
                ExplodingThing = new ExceptionThrowingItem(),
                Prop2 = "World",
            };

            // When
            var config = thing1.ConfigureCompareTo(thing2)
                .SuppressExceptions();

            var comparison = config.Compare();

            // Then
            comparison.Count.ShouldBe(2);
            comparison.ShouldContain(i => i.ToString() == "'Prop1' changed from 'Foo' to 'Bar'");
            comparison.ShouldContain(i => i.ToString() == "'Prop2' changed from 'Hello' to 'World'");
        }

        [Fact]
        public void Does_Not_Swallow_Exceptions_Unless_Configured()
        {
            // Given
            var thing1 = new
            {
                Prop1 = "Foo",
                ExplodingThing = new ExceptionThrowingItem(),
                Prop2 = "Bar",
            };

            var thing2 = new
            {
                Prop1 = "Hello",
                ExplodingThing = new ExceptionThrowingItem(),
                Prop2 = "World",
            };

            // When
            var config = thing1.ConfigureCompareTo(thing2);

            var ex = Record.Exception(() => config.Compare());

            // Then
            ex.ShouldNotBeNull();
        }

        [Fact]
        public void Treats_Null_Original_Class_As_New_Item()
        {
            // Given
            var item1 = new
            {
                Prop = default(ClassHolder)
            };

            var item2 = new
            {
                Prop = new ClassHolder { Child = new ClassExample("Hello") }
            };

            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("Set 'Prop.Child.Prop' to 'Hello'");
        }

        [Fact]
        public void Does_Not_Throw_When_New_Class_Is_Null()
        {
            // Given
            var item1 = new
            {
                Prop = new ClassHolder { Child = new ClassExample("Hello") }
            };

            var item2 = new
            {
                Prop = default(ClassHolder)
            };


            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.First().ToString().ShouldBe("Removed 'Prop.Child.Prop' (value was previously 'Hello')");
        }

        [Fact]
        public void Returns_Summary_Of_Differences_When_All_Can_Be_Included()
        {
            // Given
            var item1 = new
            {
                Prop1 = "Value 1a",
                Prop2 = "Value 2a",
                Prop3 = "Value 3a",
            };

            var item2 = new
            {
                Prop1 = "Value 1b",
                Prop2 = "Value 2b",
                Prop3 = "Value 3b",
            };


            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.GetShortDescription(3).ShouldBe(
                "'Prop1' changed from 'Value 1a' to 'Value 1b', " +
                "'Prop2' changed from 'Value 2a' to 'Value 2b', " +
                "'Prop3' changed from 'Value 3a' to 'Value 3b'"
            );
        }

        [Fact]
        public void Returns_Summary_Of_Differences_When_All_But_One_Can_Be_Included()
        {
            // Given
            var item1 = new
            {
                Prop1 = "Value 1a",
                Prop2 = "Value 2a",
                Prop3 = "Value 3a",
                Prop4 = "Value 4a",
            };

            var item2 = new
            {
                Prop1 = "Value 1b",
                Prop2 = "Value 2b",
                Prop3 = "Value 3b",
                Prop4 = "Value 4b",
            };


            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.GetShortDescription(3).ShouldBe(
                "'Prop1' changed from 'Value 1a' to 'Value 1b', " +
                "'Prop2' changed from 'Value 2a' to 'Value 2b', " +
                "'Prop3' changed from 'Value 3a' to 'Value 3b' " +
                "and 1 other change"
            );
        }

        [Fact]
        public void Returns_Summary_Of_Differences_When_More_Than_One_Are_Not_Included()
        {
            // Given
            var item1 = new
            {
                Prop1 = "Value 1a",
                Prop2 = "Value 2a",
                Prop3 = "Value 3a",
                Prop4 = "Value 4a",
            };

            var item2 = new
            {
                Prop1 = "Value 1b",
                Prop2 = "Value 2b",
                Prop3 = "Value 3b",
                Prop4 = "Value 4b",
            };


            // When
            var comparison = item1.CompareTo(item2);

            // Then
            comparison.GetShortDescription(2).ShouldBe(
                "'Prop1' changed from 'Value 1a' to 'Value 1b', " +
                "'Prop2' changed from 'Value 2a' to 'Value 2b' " +
                "and 2 other changes"
            );
        }
    }
}
