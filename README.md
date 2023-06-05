# DeepCopy.Expression
DeepCopy.Expression is a library that allows you to create deep copies of objects using expression trees. A deep copy is a copy that duplicates not only the object itself, but also the objects it references. Expression trees are data structures that represent code as a tree expressions.

## Install
To install the library, you can use the following command in the Package Manager Console

~~~
PM > Install-Package DeepCopy.Expression
~~~

## Quick start
To use the library, you can create a class that represents your object and call the ObjectCloner.Clone method to create a deep copy of it. For example:

```csharp
class MyObject
{
    public MyObject(int id)
    {
        Id = id;
    }

    public int Id { get; }
    public string Name { get; set; }
    public List<int> List { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var obj = new MyObject(1)
        {
            Name = "Hoge",
            List = new List<int> { 10, 20, 30 }
        };
        
        var cloned = ObjectCloner.Clone(obj);
    }
}
```

The library also supports anonymous types, which are types that are inferred from the data you assign to them. For example:

```csharp
class Program
{
    static void Main(string[] args)
    {
        var obj = new
        {
            Id = 1,
            Name = "Hoge",
            List = new List<int> { 10, 20, 30 }
        };
        
        var cloned = ObjectCloner.Clone(obj);
    }
}
```

You can customize the copy behavior of your classes by using the [Cloneable] attribute and the [CopyMember] attribute. The [Cloneable] attribute marks a class as cloneable and the [CopyMember] attribute marks a field or a property as a member to be copied. You can also specify a copy policy for each member, which determines how the member is copied. For example:

```csharp
[Cloneable]
class MyObject
{
    public MyObject(int id)
    {
        Id = id;
    }

    [CopyMember]
    public int Id { get; }
    public string Name { get; set; }
    [CopyMember(CopyPolicy.ShallowCopy)]
    public List<int> List { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var obj = new MyObject(1)
        {
            Name = "Hoge",
            List = new List<int> { 10, 20, 30 }
        };
        
        var cloned = ObjectCloner.Clone(obj);
    }
}
```

## Copy policy
 The available copy policies are:

- Default: The default policy for the type of the member. For value types, it performs an assignment, For reference tytpes, it performs a deep copy. For arrays, it performs a clone. For delegates, it performs an assignment.
- DeepCopy: Performs a deep copy of the member regardless of its type.
- ShallowCopy: Performs a shallow copy of the member regardless of its type. Shallow copy is a copy that duplicates only object itself, but not the objects it references.
- Assign: Performs an assignment to the member regardless of its type.

|                |  ValueType |           Class | Array(ValueType) | Array(Class) | Delegate |
|----------------|:----------:|:---------------:|:----------------:|:------------:|:--------:|
|     **Default**|     Assign |        DeepCopy |            Clone |     DeepCopy |   Assgin |
|    **DeepCopy**|     Assign |        DeepCopy |         DeepCopy |     DeepCopy |   Assgin |
| **ShallowCopy**|     Assgin | MemberwiseClone |            Clone |        Clone |   Assgin |
|      **Assign**|     Assgin |          Assgin |           Assgin |       Assgin |   Assgin |

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

* It does not copy delegates.
* It does not support direct array specification. (Supported in ver1.3.0)

## License
This library is under the MIT License.
