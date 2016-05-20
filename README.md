# Slicedbread.CompareTo [![NuGet Version](https://img.shields.io/nuget/v/Slicedbread.CompareTo.svg?style=flat)](https://www.nuget.org/packages/Slicedbread.CompareTo/)

Compares the properties on two objects and returns the differences.

```csharp

var item1 = new { MyProp = "Hello" };
var item2 = new { MyProp = "World" };

var comparison = item1.CompareTo(item2);
```
> 'Prop1' changed from 'Hello' to 'World'

- Prints changes between properties in a human readable format
- Supports nested types
- Compares collections
- Is fully configurable

### Basic comparisons

To compare two relatively simple objects, you can just call `CompareTo()` directly..

```csharp
var diff = oldItem.CompareTo(newItem);
```

.. which will return an `IEnumerable<IDifference>', which looks like this:

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
Console.WriteLine(someDiff);
```
> // Prints: 'SomeProperty' changed from 'SomeOldValue' to 'SomeNewValue'


### Nested Classes

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
```
> // Will print: 'Child.IntegerProperty' changed from '999' to '123'


