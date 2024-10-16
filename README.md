# TruckLib

**TruckLib** is a C# library for the map format of Euro Truck Simulator 2 / American Truck Simulator.
The primary focus is programmatic map creation, but the library also handles various other tasks needed for this purpose, such as reading prefab descriptor files.

The currently supported [map format version](https://github.com/sk-zk/map-docs/wiki/Map-format-version) is **901** (game version **1.51/1.52**).

(This project is pretty much a perpetual alpha, so you'll probably run into a few issues sooner or later, and breaking changes will happen on occasion.)

## Namespaces
**TruckLib.ScsMap**:  
The main namespace of the library: classes for working with the map format.

**TruckLib.Sii**:  
Parsers for .sii and .mat files.

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
Documentation is available at https://sk-zk.github.io/trucklib/master/.

## Known issues and limitations
* The library does not calculate the bounding boxes of items, so you'll need to recalculate on load (Map&nbsp;>&nbsp;Recompute map).
* Anything to do with prefabs may or may not break in unexpected ways.
* The prefab object creator doesn't handle terrain points yet. Unless you need prefab terrain, this is also
  fixed by recalculating.
* When adding a camera path, the positions of the control points in the Keyframe objects are not set to any default values
  and therefore left at (0, 0, 0).
* Helper locators of curve items, if used by the model, are not placed automatically by the library because their 
  coordinates are not known to it. Moving a locator node or a curve node which is tethered to a locator node will also cause issues.
  The editor will fix both of these once a curve node is moved or the properties dialog of the curve is closed.
* External map data (which is how the Winter Wonderland map was included in the game) is not yet supported.

## License
TruckLib is licensed under GPL v2 except for `CityHash.cs`.

## Credits
Parts of TruckLib are based on [ConverterPIX](https://github.com/mwl4/ConverterPIX)
and [SCS Blender Tools](https://github.com/SCSSoftware/BlenderTools/).
