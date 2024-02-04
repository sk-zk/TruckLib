# TruckLib

## Create Route
* [/Samples/02-Route/README.md](https://github.com/gsus24/TruckLib/blob/route/Samples/02-Route/README.md)





**TruckLib** is a C# library for the map format of Euro Truck Simulator 2 / American Truck Simulator.
The primary focus is programmatic map creation, but the library also handles various other tasks needed for this purpose, such as reading prefab descriptor files.

The currently supported map format version is **900** (game version **1.48.5/1.49**).

(This project is pretty much a perpetual alpha, so you'll probably run into a few issues sooner or later, and breaking changes will happen on occasion.)

## What's all this then
**TruckLib.ScsMap**:  
The main namespace of the library: classes for working with the map format.

**TruckLib.Sii**:  
Mostly complete de/serializer for SII and .mat files.

**TruckLib.Models**:  
Rudimentary support for binary model files (.pm\*), prefab descriptors (.ppd), and binary .tobj files.

**TruckLib.HashFs**:  
A reader for HashFS (.scs) files, the asset archive format of the game.

## Minimal example
```csharp
using System.Numerics;
using TruckLib.ScsMap;

var map = new Map("example");
Model.Add(map, new Vector3(10, 0, 10), "dlc_no_471", "brick", "default");
map.Save(@"<ETS2 folder>\mod\user_map\map");
```

## Documentation
It's long overdue, but I've finally started working on cleaning this thing up a bit and writing some proper documentation
for it. You'll find a link to it here soon™.

## Known issues and limitations
* The library does not calculate the bounding boxes of items, so you'll need to recalculate on load (Map > Recompute map).
* I've yet to figure out how the game calculates the length of roads etc. What I've come up with is
  accurate enough to not be too annoying, but you'll notice while editing one of these items that they will slightly
  change their path as the game recalculates its length.
* Anything to do with prefabs may or may not break in unexpected ways.
* The prefab object creator doesn't handle terrain points yet. Unless you need prefab terrain, this is also
  fixed by recalculating.
* When adding a camera path, the positions of the control points in the Keyframe objects are not set to any default values
  and therefore left at (0, 0, 0).
* Helper locators of curve items, if used by the model, are not placed automatically by the library because their 
  coordinates are not known to it. Moving a locator node or a curve node which is tetherd to a locator node will also cause issues.
  The editor will fix both of these once a curve node is moved or the properties dialog of the curve is closed.
* External map data (which is how the Winter Wonderland map was included in the game) is not yet supported.
 
## Dependencies
* [Ionic.Zlib](https://www.nuget.org/packages/Iconic.Zlib.Netstandard/)
* [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp/)
* [System.Numerics.Vectors](https://www.nuget.org/packages/System.Numerics.Vectors/)

## License
TruckLib is licensed under GPL v2 except for `CityHash.cs`.

## Credits
Parts of TruckLib are based on [ConverterPIX](https://github.com/mwl4/ConverterPIX)
and [SCS Blender Tools](https://github.com/SCSSoftware/BlenderTools/).
