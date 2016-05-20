# Slicedbread.CompareTo [![NuGet Version](https://img.shields.io/nuget/v/Slicedbread.CompareTo.svg?style=flat)](https://www.nuget.org/packages/Slicedbread.CompareTo/)

Compares the properties on two objects and returns the differences.

```csharp

var item1 = new { MyProp = "Hello" };
var item2 = new { MyProp = "World" };

var comparison = item1.CompareTo(item2);

// Prints: 'MyProp' changed from 'Hello' to 'World'
Console.WriteLine(comparison[0]);
```


- Prints changes between properties in a human readable format
- Supports nested types
- Compares collections
- Is fully configurable

## Installation

Install via nuget:

```
PM > Install-Package Slicedbread.CompareTo
```

## Basic comparisons

To compare two relatively simple objects, you can just call `CompareTo()` directly..

```csharp
var diff = oldItem.CompareTo(newItem);
```

.. which will return an `IEnumerable<IDifference>`, which looks like this:

```csharp
public interface IDifference
{
    Type PropertyType { get; }
    string PropertyName { get; set; }

    object OriginalValue { get; }
    object NewValue { get; }
}
```

Importantly, all of the differences returned override `ToString()` to return a human readable explaination of the change.

```csharp
// Prints: 'SomeProperty' changed from 'SomeOldValue' to 'SomeNewValue'
Console.WriteLine(someDiff);
```

#### Nested Classes

Nested classes are handled automatically. The string representation of any changes will include the full path to the property:

``` csharp
var originalObject = new
{
   Child = new { IntegerProperty = 999 }
};

var newObject = new
{
   Child = new { IntegerProperty = 123 }
};

var result = originalObject.CompareTo(newObject);

// Prints: 'Child.IntegerProperty' changed from '999' to '123'
Console.WriteLine(result[0]);
```

## Configuration

If you want to do anything more complicated, you can create a comparison configuration first:

```csharp
var comparisonConfig = item1.ConfigureCompareTo(item2)
   .Ignore(x => x.SomeProperty);
   
var result = comparisonConfig.Compare();
```

#### Ignoring Properties

To ignore properties, use the `.Ignore()` method when configuring the comparison, passing the property you wish to be ignored:

```csharp
var comparisonConfig = item1.ConfigureCompareTo(item2)
   .Ignore(x => x.IgnoreMe);
   
var result = comparisonConfig.Compare();
```

The comparison will ignore all changes to the `IgnoreMe` property.

You can, of course, do this on nested properties too:

```csharp
var comparisonConfig = item1.ConfigureCompareTo(item2)
   .Ignore(x => x.Parent.Child.IgnoreMe);
   
var result = comparisonConfig.Compare();
```

#### Collections

You can also compare collections. 

Collection changes also override the 'ToString()` method, but look like this:

```
'RemoveMe' was removed from 'SomeList'
'AddMe' was added to 'SomeList'
'Hello' changed to 'World' in 'SomeList'
```

By default, collections are compared by reference.

If you wish to compare the collections using some sort of ID property, you can configure this too:

```csharp
// When
var config = oldThing.ConfigureCompareTo(newThing)
    .CompareCollection<CollectionItem>()
    .UsingPropertyAsKey(c => c.Id);
   
var result = config.Compare();
```

In this case, whenever a collection of `CollectionItems` is compared, they will be considered to represent the same item if both of their `Id` properties match.

Items that have the same 'Key' property will have their properties compared. If _any_ are different, the entire item is considered to have changed.

If you wish the result of these comparisons to be human readable, you should override the `ToString()` method on the type contained in the list. 

```csharp
// Some class held in a collection to be compared
public class AddressItem
{
    public override string ToString()
    {
        return AddressLine1 + ", " + Country;
    }
}
```
A comparison of a collection of any `IEnumerable<AddressItems>` would then like like this:

```
'1 Main Street, USA' changed to '2 Main Street, USA' in 'AddressList'
'10 Park Lane, UK' was added to 'AddressList'
```

#### Ignoring Exceptions

If the comparison is not crucial to your application (you may be using it for logging, for example), you can configure CompareTo to swallow any exceptions.

```csharp
var config = thing1.ConfigureCompareTo(thing2)
    .SuppressExceptions();

var comparison = config.Compare();
```

Exceptions should not occur during normal comparisons, but it is always possible that something could go wrong in a propery accessor or in our library itself (hey, nobody's perfect!). 
If the comparison is not essential to the running of the application, consider supressing exceptions.
