# The Map class

## Creating a map
To create a new, empty map, call the [constructor of `Map`](xref:TruckLib.ScsMap.Map.%23ctor*).
The `name` parameter is what the `.mbd` file and the sector directory of the map will be named when it is saved.

```cs
using TruckLib.ScsMap;

Map map = new Map("example");
```

## Opening a map
To load an existing map from disk, call the static [`Map.Open`](xref:TruckLib.ScsMap.Map.Open*) method with
the path of the `.mbd` file.

```cs
using TruckLib.ScsMap;

Map map = Map.Open(@"E:\SteamLibrary\steamapps\common\Euro Truck Simulator 2\extracted\map\europe.mbd");
```

If you would like to only load specific sectors rather than the entire map, use the optional `sectors` parameter.
It expects an array of sector coordinates as tuples in (X, Z) order.

## Saving a map
To save a map, call the [`Save`](xref:TruckLib.ScsMap.Map.Save*) method of the map object. The map will be
written to the specified directory.

```cs
string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
string userMap = Path.Combine(documents, "Euro Truck Simulator 2/mod/user_map/map/");
map.Save(userMap, true);
```

If the optional parameter `cleanSectorDirectory` is set to true, which it is by default, the sector directory
will be emptied before saving the map.

Remember to recompute the map in the editor (Map > Recompute map) following this.

## Map items
All map items are stored in the [`MapItems`](xref:TruckLib.ScsMap.Map.MapItems) dictionary, indexed by UID.

```cs
bool exists = map.MapItems.TryGetValue(0x521CD80FA4000001, out MapItem item);

var allMovers = map.MapItems.OfType<Mover>();
```

## Nodes
Likewise, the [`Nodes`](xref:TruckLib.ScsMap.Map.Nodes) property is a dictionary containing all nodes in the map.

```cs
bool exists = map.Nodes.TryGetValue(0x521CD80C53000000, out INode node);
```

### Searching for nodes
Searching for nodes within a bounding box or around a point is not yet implemented. Soon™.

## Sectors
In TruckLib, sectors don't contain any map objects internally: which sector an item or a node should be written
to is decided when [`Save`](xref:TruckLib.ScsMap.Map.Save*) is called.

Sector metadata, however, does exist, and is stored in the [`Sectors`](xref:TruckLib.ScsMap.Map.Sectors) dictionary.

## Map metadata
The scale of the map can be set with the [`NormalScale`](xref:TruckLib.ScsMap.Map.NormalScale) and
[`CityScale`](xref:TruckLib.ScsMap.Map.CityScale) properties.