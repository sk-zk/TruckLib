# Finding map items and nodes

## Finding map items

### By UID
To test whether an item with a specific UID exists in the map, call the [`HasItem`](xref:TruckLib.ScsMap.Map.HasItem*) method:

```cs
bool exists = map.HasItem(0x521CD80FA4000001);
```

To retrieve the item, call [`GetItem`](xref:TruckLib.ScsMap.Map.GetItem*):

```cs
MapItem item = map.GetItem(0x521CD80FA4000001);
```

You can do both at the same time with [`TryGetItem`](xref:TruckLib.ScsMap.Map.TryGetItem*):

```cs
bool exists = map.TryGetItem(0x521CD80FA4000001, out MapItem item);
```

### By type
To retrieve a dictionary of all map items of a specific type, call [`GetAllItems<T>`](xref:TruckLib.ScsMap.Map.GetAllItems``1):

```cs
Dictionary<ulong, Mover> movers = map.GetAllItems<Mover>();
```

### By sector
A dictionary of all items in a specific sector can be retrieved as follows:

```cs
(int, int) sectorCoord = (4, -2);
Dictionary<ulong, MapItem> sectorItems = map.Sectors[sectorCoord].MapItems;
```

### All of them
You can fetch all items in the map with [`GetAllItems`](xref:TruckLib.ScsMap.Map.GetAllItems):

```cs
Dictionary<ulong, MapItem> everything = map.GetAllItems();
```

## Finding nodes

### By UID
The [`Nodes`](xref:TruckLib.ScsMap.Map.Nodes) property is a dictionary containing all nodes in the map indexed by UID. Thus, you can retrieve a node like this:

```cs
map.Nodes.TryGetValue(0x521CD80C53000000, out INode node);
```

### Within a bounding box

Not yet implemented. Soon™.

### Around a point

Not yet implemented. Soon™.

### All of them
You can fetch all nodes in the map with [`GetAllNodes`](xref:TruckLib.ScsMap.Map.GetAllNodes):

```cs
Dictionary<ulong, INode> everything = map.GetAllNodes();
```