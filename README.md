# TruckLib

**TruckLib** is an attempt at writing a C# library for working with the map format of Euro Truck Simulator 2
and American Truck Simulator. Currently, the library supports map version 876 (game version 1.36.1).

(The code is 💩💩💩 and not "production ready" by any means, so expect plenty of bugs, missing features,
sudden API changes, and inconsistent naming - you know, all that fun stuff that is exactly what you want from a library.)

You can find some sample code in the Samples folder. (Proper documentation will follow eventually™.)
You can also take a look at my project [DlcCheck](https://github.com/sk-zk/DlcCheck). 

TruckLib is not affiliated with SCS Software.

## What's in the box?
**TruckLib.ScsMap**:  
What you're here for. Create or modify maps for ETS2 and ATS.

**TruckLib.Sii**:  
Mostly complete parser for .sii and .mat files.

**TruckLib.Model**:  
For working with binary model files (.pm\*). Functions well enough to create or modify simple static models,
but that's about it at the moment.

**TruckLib.HashFs**:
Simple reader for HashFS (.scs) files.

## Known issues and limitations
* The library does not calculate the bounding box of items, so you'll need to recalculate the map (F8) on load.
* All nodes of Road, Terrain, Building and Curve items created or modified with this library have slightly to _very_ wrong
angles until you modify them in the editor. This is because I just can't figure out exactly how the game calculates the 
Rotation value. If you have any ideas, please let me know.

## License
TruckLib is licensed under GPL v2 except for `CityHash.cs`.

## Credits
Parts of TruckLib are based on [ConverterPIX](https://github.com/mwl4/ConverterPIX)
and [SCS Blender Tools](https://github.com/SCSSoftware/BlenderTools/).
