# DeepCopy.Expression

[![Nuget](https://img.shields.io/nuget/v/DeepCopy.Expression.svg?logo-nuget)](https://www.nuget.org/packages/DeepCopy.Expression/)

[![Nuget Downloads](https://img.shields.io/nuget/dt/DeepCopy.Expression.svg)](https://www.nuget.org/packages/DeepCopy.Expression/)

[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

`DeepCopy.Expression` is a high-performance deep copy library for .NET.
It generates clone logic with expression trees and cashes compiled delegates for repreated use.

A deep copy duplicates not only the object itself, but also the objects it references.
An Expression Tree is a data structure that represents code as a tree of expressions.

## Features
- Deep copy for classes, structs, nullable structs, and anonymous types
- Array clone APIs for `T[]`, `T[,]`, `T[,,]`, `T[,,,]`, `T[,,,,]` and non-generic `Array`
- Supports deep cloning of polymorphic object hierarchies, including abstract classes and interfaces
- Handles circular references and self-referencing object graphs
- Supports cloning deeply nested object graphs such as `LinkedList<T>`.
- Optional reference-preserving mode (`preserveObjectReferences` parameter) for shared object graphs
- Member-level copy control with `[Cloneable]`, [`CopyMember]` attributes and `CopyPolicy` parameter.
- Custom clone logic registration for each type
- Warm-up and cache lifecycle APIs: `Compile<T>` and `Cleanup<T>` methods

## Installation
To install the library, you can use the following command in the Package Manager Console

~~~
PM > Install-Package DeepCopy.Expression
~~~

## Supported Target Frameworks
- `net10.0`
- `netstandard2.0`

## Quick Start
To create a deep copy of an object, call the `ObjectCloner.Clone` method:

```csharp
var source = new User
{
    Id = 1,
    Name = "John Doe",
    Tags = new List<string> { "admin", "reviewer" }
};

var cloned = ObjectCloner.Clone(source);
```

Anonymous types are also supported:

```csharp
var source = new
{
    Id = 1,
    Name = "John Doe",
    Tags = ["admin", "reviewer"]
};

var cloned = ObjectCloner.Clone(source);
```

## Preserving Shared References

By default, objects are deep-cloned without preserving shared references to maximize performance.
If the object graph contains shared nodes and reference identity must be preserved, pass `true`:

```csharp
var cloned = ObjectCloner.Clone(source, preserveObjectReferences: true);
```

## Attribute-Based Member Control

Default behavior (without `[Cloneable]`:
- All instance fields are copied, including private fields.
- Event backing fields are excluded

Opt-in behavior (with `[Cloneable]`):
- Only members marked with `[CopyMember]` are copied
- A `CopyPolicy` can be specified for each member

```csharp
[Cloneable]
public sealed class Settings
{
    [CopyMember]
    private readonly string _id = Guid.NewGuid().ToString();

    [CopyMember(CopyPolicy.DeepCopy)]
    public List<string> Values { get; set; } = new ();

    [CopyMember(CopyPolicy.Assign)]
    public IServiceProvider ServiceProvider { get; set; } = default!;
}
```

## Copy Policy
The following copy policies are available:

- `Default`: Uses the default behavior for the member type. Value types are assigned, reference types are deep-copied, arrays are cloned, and delegates are assigned.
- `DeepCopy`: Performs a deep copy of the member regardless of its type.
- `ShallowCopy`: Performs a shallow copy of the member regardless of its type. A shallow copy duplicates only the object itself and not the objects it references.
- `Assign`: Copies the member as-is regardless of its type. For reference types, no new instance is created and the original reference is shared.

|                |  ValueType | Class /<br>Struct (contains references) | Array (Value Type) | Array (Class) | Delegate |
|----------------|:----------:|:---------------:|:----------------:|:------------:|:--------:|
|     **Default**|     Assign |        DeepCopy |            Clone |     DeepCopy |   Assign |
|    **DeepCopy**|     Assign |        DeepCopy |         DeepCopy |     DeepCopy |   Assign |
| **ShallowCopy**|     Assign | MemberwiseClone |            Clone |        Clone |   Assign |
|      **Assign**|     Assign |          Assign |           Assign |       Assign |   Assign |

## Custom Clone Registration
Starting with version 1.5.0, custom clone logic can be registered for specific types using the `ObjectCloner.RegisterCustomClone` method.

The following example ensures that cloned object recieves a new unique ID:

```csharp
ObjectCloner.RegisterCustomClone(
    typeof(MyCustomizableObject),
    (source, destination, context) =>
    {
        var fields = CustomCloneHelper.BuildCloneFieldsExpression(
            source.Type, source, destination, context, "_id");

        return Expression.Block(
            fields,
            CustomCloneHelper.BuildFieldAssignment(
                destination, "_id", Guid.NewGuid())
        );
    });
```

## Performance
This is a benchmark of [TestObject](https://github.com/lumiria/DeepCopy.Expression/blob/master/tests/DeepCopy.Test/TestObject.cs)'s deep clone.
The performance of the library is comparable to the code that is specially implemented for deep copying. The library uses caching to avoid generating expression trees every time. The first time you clone an object, it may take longer than subsequent times.

|                           Method |        Mean |      Error |     StdDev | Ratio |    Gen 0 |
|--------------------------------- |------------:|-----------:|-----------:|------:|---------:|
|          CloneWithImprementation |    41.77 us |  1.4089 us |  4.0425 us |  1.00 |  30.0293 |
|           CloneWithSerialization |   698.42 us |  5.5453 us |  4.9158 us | 15.57 | 179.6875 |
| CloneWithExpressionFirstTimeOnly | 4,242.48 us | 84.4623 us | 97.2669 us | 97.07 | 179.6875 |
|              CloneWithExpression |    42.37 us |  0.5604 us |  0.5242 us |  0.95 |  27.8931 |


## Limitations
The library has some limitations:

* Delegates are not copied and are shared between the source and cloned objects.
* `CopyTo` is disabled for immutable collections such as `ImmutableList<T>`, `ImmutableStack<T>`, `ImmutableQueue<T>`, and `ImmutableHashSet<T>`.
* `ImmutableHashSet<T>` and `ImmutableSortedSet<T>` cannot be cloned correctly when they contain self-references.
* To prevent stack overflows, members deeper than `DeepCopyOptions.MaxRecursionDepth` are processed interatively, except for types specified in `DeepCopyOptions.NonCopyableGenericTypes`, which are always processed recursively.

## License
This library is under the MIT License.
