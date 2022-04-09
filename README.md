# TruckLib

**TruckLib** is a C# library for working with the map format of Euro Truck Simulator 2 / American Truck Simulator.
Programmatic map creation is the primary focus, but the library also handles various other tasks needed for this purpuse, such as reading ppd files.

The currently supported map version is 891 (game version 1.43).

(This project is pretty much a perpetual alpha, so you'll probably run into a few issues sooner or later.)

## What's all this then
**TruckLib.ScsMap**:  
Create or modify ETS2 and ATS maps.

**TruckLib.Sii**:  
Mostly complete de/serializer for .sii and .mat files.

**TruckLib.Model**:  
Some code for working with binary model files (.pm\*). Very basic and hasn't been updated in a while.

Also contains a (binary) .tobj de/serializer.

**TruckLib.HashFs**:  
A reader for HashFS (.scs) files, the asset archive format of the game.

## Minimal example
```
using System.Numerics;
using TruckLib.ScsMap;

var map = new Map("example");
Model.Add(map, new Vector3(10, 0, 10), 0, "dlc_no_471", "brick", "default");
map.Save(@"<ETS2 folder>\mod\user_map\map");
```

## Known issues and limitations
* The library does not calculate the bounding boxes of items, so you'll need to recalculate on load (Map > Recompute map).
* I've yet to figure out how the game calculates the length of roads etc. What I've come up with is
accurate enough to not be too annoying, but you'll notice while editing one of these items that they will slightly
change their path as the game recalculates its length.
* Anything to do with prefabs may or may not break in unexpected ways.
* The prefab object creator doesn't handle terrain points yet. Unless you need prefab terrain, this is also
 fixed by recalculating.
 
## Dependencies
* [Ionic.Zlib](https://www.nuget.org/packages/Iconic.Zlib.Netstandard/)
* [Microsoft.CSharp](https://www.nuget.org/packages/Microsoft.CSharp/)
* [System.Numerics.Vectors](https://www.nuget.org/packages/System.Numerics.Vectors/)

## License
TruckLib is licensed under GPL v2 except for `CityHash.cs`.

## Credits
Parts of TruckLib are based on [ConverterPIX](https://github.com/mwl4/ConverterPIX)
and [SCS Blender Tools](https://github.com/SCSSoftware/BlenderTools/).
