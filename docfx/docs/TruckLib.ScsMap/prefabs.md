# Working with prefabs

## Adding prefabs
Prefabs are the most complex map item in the game, so adding one requires an extra step.

### Loading the descriptor
To add a prefab to the map, the positions of its control nodes and spawn points must be known to the library
so that the map nodes can be placed correctly and the required slave items can be created. This means that
the prefab descriptor (`.ppd`) file of the prefab must be opened first, which you can do with the
[`PrefabDescriptor`](xref:TruckLib.Models.Ppd.PrefabDescriptor) class:

```cs
using TruckLib.Models.Ppd;

PrefabDescriptor ppd = PrefabDescriptor.Open("<location of extracted files>/prefab2/car_dealer/car_dealer_01_fr.ppd");
```

Alternatively, you can use a [`HashFsReader`](xref:TruckLib.HashFs.HashFsReader) to load the desciptor directly
from its `.scs` file without having to extract it first: 

```cs
using TruckLib.Models.Ppd;
using TruckLib.HashFs;

HashFsReader baseScs = HashFsReader.Open("<game root>/base.scs");
byte[] ppdBytes = baseScs.Extract("/prefab2/car_dealer/car_dealer_01_fr.ppd");
PrefabDescriptor ppd = PrefabDescriptor.Load(ppdBytes);
```

Note that this will not work while the game is running because the game locks the file.

### Creating the prefab
Now that we have the prefab descriptor, we can call [`Prefab.Add`](xref:TruckLib.ScsMap.Prefab.Add*)
to add a prefab to the map:

```cs
Prefab prefab = Prefab.Add(map, new Vector3(12.3f, 0, 23.4f), "dlc_fr_14", ppd, Quaternion.Identity);
```

The position passed to this method will be the position of the 0th control node. 

Rotation is handled the same way as it is ingame: no matter how the model's geometry is actually oriented, it will
always be rotated such that the 0th control node has a direction of (0, 0, -1). This means that if you pass e.g.
a yaw of 90°, the model will be rotated the same way as it would be if you specified 90° in the new item dialog
of the editor.

## Changing the origin
On occasion, the origin node of a prefab must be changed to allow for connections which would otherwise not be possible.
You can do this with the [`ChangeOrigin`](xref:TruckLib.ScsMap.Prefab.ChangeOrigin*) method:

```cs
prefab.ChangeOrigin(2);
```

Unlike other prefab methods, the index expected here is the index of the node in the prefab descriptor,
not the [`Nodes`](xref:TruckLib.ScsMap.Prefab.Nodes) list of the object, because `Nodes` is always
ordered such that the origin node is the 0th entry.

If one or both nodes affected by the operation already have an item attached to them, `InvalidOperationException` is thrown.

## Attaching polyline items
To attach a road or another polyline item to a node of a prefab, call the
[`Attach`](xref:TruckLib.ScsMap.Prefab.Attach(TruckLib.ScsMap.INode,System.UInt16)) method:

```cs
prefab.Attach(road.ForwardNode, 1);
```

The first parameter is the node you want to attach to the prefab, and the second parameter is the index of the prefab node
to attach it to.

Attempting to attach the backward node of a polyline item to the origin node of a prefab will throw `InvalidOperationException`.
All other configurations will work.

There is also a "I'm feeling lucky" option which will attach the closest nodes of the item and the prefab without having to
specifiy what they are:

```cs
prefab.Attach(road);
```

In both cases, the leftover node of the polyline item will be deleted.

## Appending a road
In addition to attaching an existing road, you can append a new road to a prefab node with the
[`AppendRoad`](xref:TruckLib.ScsMap.Prefab.AppendRoad*) method:

```cs
Road road = prefab.AppendRoad(1, new Vector3(42, 0, 21), "ger1");
```

The given position will be the position of the forward node of the road, unless you are appending to the origin node,
in which case it will be the backward node (meaning the road is actually prepended).
