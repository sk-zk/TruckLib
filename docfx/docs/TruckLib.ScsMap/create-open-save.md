# Creating, opening, and saving maps

## Creating a map
To create a new, empty map, call the [constructor of `Map`](xref:TruckLib.ScsMap.Map.%23ctor*).
The `name` parameter is what the .mbd file and the sector directory of the map will be named when it is saved.

```cs
using TruckLib.ScsMap;

var map = new Map("example");
```

## Opening a map
To load an existing map from disk, call the static [`Map.Open`](xref:TruckLib.ScsMap.Map.Open*) method with the path of the .mbd file.

```cs
using TruckLib.ScsMap;

var map = Map.Open(@"E:\SteamLibrary\steamapps\common\Euro Truck Simulator 2\extracted\map\europe.mbd");
```

If you would like to only load specific sectors rather than the entire map, use the optional `sectors` parameter. 
It expects an array of strings specifying the names of the sectors in the same format as a sector file (without extension),
e.g. `sec+0001+0002`.

## Saving a map
To save a map, call the [`Save`](xref:TruckLib.ScsMap.Map.Save*) method of the map object. The map will be written to the given directory.

```cs
var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
var userMap = Path.Combine(documents, "Euro Truck Simulator 2/mod/user_map/map/");
map.Save(userMap, true);
```

If the optional parameter `cleanSectorDirectory` is set to true, which it is by default, the sector directory
will be emptied before saving the map.

Remember to recompute the map in the editor (Map > Recompute map) following this.