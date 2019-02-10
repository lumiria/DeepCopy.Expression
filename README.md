# DeepCopy.Expression
The library for deep copying objects made with expression trees.

## Install
~~~
PM > Install-Package DeepCopy.Expression
~~~

## Quick start
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

Supports anonymous types
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

Classes can be marked using the [Cloneable] to customize copy behavior. In this case, target fields or properties mark [CopyMember].
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
Row is the specified copy policy.
Column is the type of target object.

|                |  ValueType |           Class | Array(ValueType) | Array(Class) |
|----------------|:----------:|:---------------:|:----------------:|:------------:|
|     **Default**|     Assign |        DeepCopy |            Clone |     DeepCopy |
|    **DeepCopy**|     Assign |        DeepCopy |         DeepCopy |     DeepCopy |
| **ShallowCopy**|     Assgin | MemberwiseClone |            Clone |        Clone |
|      **Assign**|     Assgin |          Assgin |           Assgin |       Assgin |

## Performance
This is a benchmark of [TestObject](https://github.com/lumiria/DeepCopy.Expression/blob/master/tests/DeepCopy.Test/TestObject.cs)'s deep clone.
Except for the first time, it is almost the same speed as the code specially impremented.

|                           Method |        Mean |      Error |     StdDev | Ratio |    Gen 0 |
|--------------------------------- |------------:|-----------:|-----------:|------:|---------:|
|          CloneWithImprementation |    41.77 us |  1.4089 us |  4.0425 us |  1.00 |  30.0293 |
|           CloneWithSerialization |   698.42 us |  5.5453 us |  4.9158 us | 15.57 | 179.6875 |
| CloneWithExpressionFirstTimeOnly | 4,242.48 us | 84.4623 us | 97.2669 us | 97.07 | 179.6875 |
|              CloneWithExpression |    42.37 us |  0.5604 us |  0.5242 us |  0.95 |  27.8931 |


## Limitations
* Does not copy events and delegate.
* Not supported direct array specification.

## License
This library is under the MIT License.