# SII files

TruckLib provides the [`SiiFile`](xref:TruckLib.Sii.SiiFile) class for serializing and deserializing SII files.

A `SiiFile` object contains the units of the file in the [`Units`](xref:TruckLib.Sii.SiiFile.Units) list.
A [`Unit`](xref:TruckLib.Sii.Unit) has a class name, unit name, and a number of key-value attributes stored as `Dictionary<string, dynamic>`.


## Deserializing
You can deserialize SII files with the static [`SiiFile.Open`](xref:TruckLib.Sii.SiiFile.Open*) method:

```cs
using TruckLib.Sii;

SiiFile sii = SiiFile.Open("/foo/bar.sii");
```

Alternatively, you can use [`SiiFile.Load`](xref:TruckLib.Sii.SiiFile.Load*) to parse a string.
In case the string contains `@include` directives, you must also pass the directory which the paths
of the included files are relative to. (`Open` resolves this automatically.)

```cs
SiiFile sii = SiiFile.Load(siiStr, "/path/where/included/files/are/located");
```

If an included file does not exist, `FileNotFoundException` is thrown.

### Data types
* Numbers which have a decimal component are parsed to `float`; numbers without will be the smallest of
`int`, `long`, or `ulong` which can fit the value.

* Float tuples become a `VectorN`; integer tuples become value tuples (such as `(int, int)` and so on). SCS's documentation
for units distinguishes between `int2` and `fixed2`, but it is not possible to identify which type to use from the SII file alone,
so they are parsed the same way.

* If a quaternion has its W component separated with a semicolon, like `(1; 0, 0, 0)`, it becomes a `Quaternion`.
Otherwise, they cannot be distinguished from a `float4` and are parsed to `Vector4` as stated above.

* Quoted strings are parsed to `string`. Again, the library does not differentiate between regular strings
and `resource_tie`s as they are written the same way.

* Unquoted strings are parsed to [`Token`](xref:TruckLib.Token) if they are a valid token. If not,
they become an [`OwnerPointer`](xref:TruckLib.Sii.OwnerPointer) if they start with `.`, and a
[`LinkPointer`](xref:TruckLib.Sii.LinkPointer) otherwise.

* The values `true` and `false` are parsed as `bool`.

* If an array is preceded by a length parameter and/or has explicit indices, it is parsed to an array. Otherwise, it is
parsed to a list.

## Serializing
To serialize a `SiiFile` to string, call the [`Serialize`](xref:TruckLib.Sii.SiiFile.Serialize*) method:

```cs
SiiFile sii = new();
Unit unit = new("prefab_model", "prefab.dlc_blke_232");
unit.Attributes.Add("model_desc", "/prefab2/fork_temp/blke/blke_road2_no_offset_tram_to_road2_tram.pmd");
unit.Attributes.Add("prefab_desc", "/prefab2/fork_temp/blke/blke_road2_no_offset_tram_to_road2_tram.pmd");
unit.Attributes.Add("category", "dlc_blke");
unit.Attributes.Add("corner0", new List<Token>{"blke383", "blke385"});
unit.Attributes.Add("corner1", new List<Token>{"blke384", "blke386"});
sii.Units.Add(unit);
string siiStr = sii.Serialize();
```

You can also write it to disk using [`Save`](xref:TruckLib.Sii.SiiFile.Serialize*):

```cs
sii.Save("foo.sii");
```

Both methods have an optional parameter `indentation` which sets the indentation inside units. The default is `\t`.