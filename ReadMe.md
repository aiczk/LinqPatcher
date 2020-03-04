# LinqPatcher

A tool that creates a method containing code equivalent to Linq and calls that method instead.

  - Simply attach the *[Optimize]* attribute to the target method.
  - It can be used simply by attaching attributes.
  - Currently arrays and arguments are supported.
  
## Intoroduction

  1. Download the latest from the [LinqPatcher/Release](https://github.com/aiczk/LinqPatcher/releases) page.
  2. Place an asmdef named Main in any parent folder.
  3. Add *LinqPatcherAttribute.dll* to Assembly Definitions References.
  4. Done.

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

private IEnumerable<int> e242dad72e884682a5f8424598974110(int[] P_0)
{
	if (linq_e242dad72e884682a5f8424598974110.Count > 0)
		linq_e242dad72e884682a5f8424598974110.Clear();

	foreach (int P_1 in P_0)
	{
		if (P_1 % 2 != 0)
		{
			P_1 *= 2;
			linq_e242dad72e884682a5f8424598974110.Add(P_1);
		}
	}

	return linq_e242dad72e884682a5f8424598974110;
}
```


## Currently supported operators
 - Where
 - Select
 - ToList
 
## Releases Note
See [here.](https://github.com/aiczk/LinqPatcher/releases)

## License
[MIT License](https://github.com/aiczk/LinqPatcher/blob/master/License.txt)
