# TruckLib

**TruckLib** is an attempt at writing a C# library for working with the map format of 
Euro Truck Simulator 2 / American Truck Simulator (and various other formats used in the games).

The library currently supports map version 876 (game version 1.36.1 and up).

(The code is 💩💩💩, so expect bugs, missing features,
sudden API changes, inconsistent naming - you know, all that fun stuff that is exactly 
what you want from a library.)

You can find some sample code in the Samples folder. (Proper documentation will follow eventually™.)  

TruckLib is not affiliated with SCS Software.

## What's in the box?
**TruckLib.ScsMap**:  
Create or modify maps for ETS2 and ATS.

**TruckLib.Sii**:  
Mostly complete de/serializer for .sii and .mat files.

**TruckLib.Model**:  
For working with binary model files (.pm\*). Functions well enough to create or modify simple static models,
but that's about it at the moment.

Also contains a (binary) tobj de/serializer.

**TruckLib.HashFs**:  
Simple reader for HashFS (.scs) files.

## Known issues and limitations
* The library does not calculate the bounding box of items, so you'll need to recalculate the map (F8) on load.
* Anything to do with prefabs is probably not going to behave as expected.
* The prefab object creator doesn't handle terrain points yet. Unless you need prefab terrain, this is also
 fixed by recalculating.

## License
TruckLib is licensed under GPL v2 except for `CityHash.cs`.

## Credits
Parts of TruckLib are based on [ConverterPIX](https://github.com/mwl4/ConverterPIX)
and [SCS Blender Tools](https://github.com/SCSSoftware/BlenderTools/).
