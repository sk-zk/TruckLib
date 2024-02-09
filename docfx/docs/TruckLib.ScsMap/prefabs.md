# Working with prefabs

## Adding prefabs
Prefabs are the most complex map item in the game, so adding one requires an extra step.

### Loading the descriptor
To add a prefab to the map, the positions of its control nodes and spawn points must be known to the library
so that the map nodes can be placed correctly and slave items, if required, can be created. This means that
the prefab descriptor (`.ppd`) file of the prefab must be opened first, which you can do with the
[`PrefabDescriptor`](xref:TruckLib.Models.Ppd.PrefabDescriptor) class:

```cs
using TruckLib.Models.Ppd;

PrefabDescriptor ppd = PrefabDescriptor.Open("<location of extracted files>/prefab2/car_dealer/car_dealer_01_fr.ppd");
```

Alternatively, you can use a [`HashFsReader`](xref:TruckLib.HashFs.HashFsReader) to load the descriptor directly
from its `.scs` file without having to extract it: 

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
to add the prefab to the map:

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

## Attaching prefabs
You can attach two prefabs to each other with [`Attach`](xref:TruckLib.ScsMap.Prefab.Attach(System.UInt16,TruckLib.ScsMap.Prefab,System.UInt16)):

```cs
prefab1.Attach(3, prefab2, 0);
```

The first parameter is the index of the node of `prefab1` the other prefab will be connected to, the second parameter
is the prefab, and the third parameter is the index of the node of `prefab2` which you want to attach. Note that
`prefab2` will be moved such that the two specified nodes have the same position &ndash; the prefabs can't
be connected otherwise. 

If both specified nodes are the current origin node (which is the case when both index parameters are 0) or
one or both of the nodes already have something attached to them, `InvalidOperationException` is thrown.

There is also a "I'm feeling lucky" option which will attach the closest nodes of the two prefabs without having to
specify what they are:

```cs
prefab1.Attach(prefab2);
```

In both cases, one of the prefab nodes will become unnecessary and will be deleted.

## Attaching polyline items
There is another overload of [`Attach`](xref:TruckLib.ScsMap.Prefab.Attach(System.UInt16,TruckLib.ScsMap.INode))
for attaching a road or another polyline item to a node of a prefab:

```cs
prefab.Attach(1, road.ForwardNode);
```

The first parameter is the index of the prefab node to attach the item to, and the second parameter is the item node
you want to attach to the prefab.

Attempting to attach the backward node of a polyline item to the origin node of a prefab will throw `InvalidOperationException`.
All other configurations will work.

As with attaching a prefab, there is another method which attaches the closest nodes of the item and the prefab:

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
