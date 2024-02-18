# Working with polyline items

[Polyline items](xref:TruckLib.ScsMap.PolylineItem), such as [roads](xref:TruckLib.ScsMap.Road), have several methods which simplify working with them.

## Appending and prepending
You can append or prepend to a polyline by calling the `Append` or `Prepend` methods:

```cs
Road road2 = road1.Append(new Vector3(30, 0, 10));
```

This will create a new Road item `road2` whose backward node is the forward node of `road1` and whose forward node is at (30, 0, 10).

```cs
Road road2 = road1.Prepend(new Vector3(30, 0, 10));
```

This will create a new Road item `road2` whose forward node is the backward node of `road1` and whose backward node is at (30, 0, 10).

In both cases, if there is already an item attached in the direction you wish to extend, an `InvalidOperationException` is thrown.

By default, the properties of item the method is called on are copied to the new item. If you would like it to have its default
properties instead, set the optional parameter `cloneSettings` to `false`.

## Connecting two polyline items
To connect two unconnected polyline items where one has a free forward node and one has a free backward node, call
[`Merge`](xref:TruckLib.ScsMap.Node.Merge*) on the node you wish to keep:

```cs
// Say you have the following two road items:
Road road1 = Road.Add(map, new Vector3(10, 0, 10), new Vector3(30, 0, 10), "ger1");
Road road2 = Road.Add(map, new Vector3(32, 0, 12), new Vector3(50, 0, 30), "ger1");

// To connect the roads and keep the node at (30, 0, 10), do the following:
road1.ForwardNode.Merge(road2.Node);

// Alternatively, if you would like to keep the node at (32, 0, 12), call it 
// the other way around:
road2.Node.Merge(road1.ForwardNode);
```

## Disconnecting two polyline items
You can disconnect two polyline items by calling [`Split`](xref:TruckLib.ScsMap.Node.Split*) on the node
which connects them. For example, the following line of code will disconnect `road` from its forward item:

```cs
INode newNode = road.ForwardNode.Split();
```

The method also returns the newly created node.

## Connecting/disconnecting polyline items to/from prefabs
See [Prefabs](~/docs/TruckLib.ScsMap/prefabs.md).

## First and last item
To find the first or last item of a polyline chain given one of its members, call [`FindFirstItem`](xref:TruckLib.ScsMap.PolylineItem.FindFirstItem*)
or [`FindLastItem`](xref:TruckLib.ScsMap.PolylineItem.FindLastItem*) respectively:

```cs
PolylineItem start = road.FindFirstItem();
PolylineItem end = road.FindLastItem();
```

`start` and `end` are now the first and last polyline item of the chain `road` is a part of. Keep in mind that all polyline item types
can attach to each other, so `start` end `end` are not guaranteed to be of the same type as `road`.
