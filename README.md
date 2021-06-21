# JOS.FlatDictionary
Transforms a C# object to a flat ```Dictionary<string, string```.

## Example
```csharp
var sut = new Implementation3();
var myClass = new MyClass
{
    Boolean = true,
    Guid = Guid.NewGuid(),
    Integer = 100,
    String = "string",
    MyNestedClass = new 
    {
        Boolean = true,
        Guid = Guid.NewGuid(),
        Integer = 100,
        String = "string"
    }
};

var result = sut.Execute(myClass, prefix: "Data");

result.ShouldContainKeyAndValue("Data.Boolean", "true");
result.ShouldContainKeyAndValue("Data.Guid", myClass.Guid.ToString());
result.ShouldContainKeyAndValue("Data.Integer", myClass.Integer.ToString());
result.ShouldContainKeyAndValue("Data.String", myClass.String);
result.ShouldContainKeyAndValue("Data.MyNestedClass.Boolean", myClass.MyNestedClass.Boolean.ToString().ToLower());
result.ShouldContainKeyAndValue("Data.MyNestedClass.Guid", myClass.MyNestedClass.Guid.ToString());
result.ShouldContainKeyAndValue("Data.MyNestedClass.Integer", myClass.MyNestedClass.Integer.ToString());
result.ShouldContainKeyAndValue("Data.MyNestedClass.String", myClass.MyNestedClass.String);

```

## More info
https://josef.codes/transform-csharp-objects-to-a-flat-string-dictionary/
