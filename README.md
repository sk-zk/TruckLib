# TruckLib

**TruckLib** is a C# library for working with the map format of Euro Truck Simulator 2 / American Truck Simulator.
Programmatic map creation is the primary focus of this library, but it also handles various other tasks needed for this purpuse, such as reading ppd files.

The library currently supports map version 889 (game version 1.41 & 1.42).

(This project is pretty much a perpetual alpha, so you'll probably run into a few issues sooner or later.)

Proper documentation will follow ~~eventually™~~ probably never. You know how it is.  
You can find some sample code in the Samples folder, or you can check out my tool [DlcCheck](https://github.com/sk-zk/DlcCheck) and my weird experimental thing [OsmProto](https://github.com/sk-zk/OsmProto) for some more existing code.

TruckLib is not affiliated with SCS Software.

## What's in the box?
**TruckLib.ScsMap**:  
Create or modify maps for ETS2 and ATS.

**TruckLib.Sii**:  
Mostly complete de/serializer for .sii and .mat files.

**TruckLib.Model**:  
Code for working with binary model files (.pm\*). Works well enough to create or modify simple static models,
but that's about it at the moment.

Also contains a (binary) .tobj de/serializer.

**TruckLib.HashFs**:  
A reader for HashFS (.scs) files, the asset archive format of the game.

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
