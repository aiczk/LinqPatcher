# LinqPatcher

A tool that creates a method containing code equivalent to Linq and calls that method instead.

  - Simply attach the *[Optimize]* attribute to the target method.
  - It can be used simply by attaching attributes.
  - Currently only arrays are supported.
  
## Intoroduction

  1. Place an asmdef named Main in any parent folder.
  2. Add *LinqPatcherAttribute.dll* to Assembly Definitions References.
  3. Done.

Notation in the Code Editor.
```cs
[Optimize]
private void Optimize()
{
    IEnumerable<int> num = array.Where(x => x % 2 == 0).Select(x => x * 2);
}
```

Code called at runtime.
```cs
[Optimize]
private void Optimize()
{
    IEnumerable<int> enumerable = e242dad72e884682a5f8424598974110(array);
}
```


## Currently supported operators
 - Where
 - Select

License
----

MIT
