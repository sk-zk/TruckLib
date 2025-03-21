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
[`Merge`](xref:TruckLib.ScsMap.Node.Merge*) on the node you wish to keep.

Say you have the following two road items:
```cs
Road road1 = Road.Add(map, new Vector3(10, 0, 10), new Vector3(30, 0, 10), "ger1");
Road road2 = Road.Add(map, new Vector3(32, 0, 12), new Vector3(50, 0, 30), "ger1");
```

To connect the roads and keep the node at (30, 0, 10), write the following:

```cs
road1.ForwardNode.Merge(road2.Node);
```

Alternatively, if you would like to keep the node at (32, 0, 12), call it  the other way around:
```cs
road2.Node.Merge(road1.ForwardNode);
```

## Disconnecting two polyline items
You can disconnect two polyline items by calling [`Split`](xref:TruckLib.ScsMap.Node.Split*) on the node
which connects them. For example, the following line of code will disconnect `road` from its forward item:

```cs
INode newNode = road.ForwardNode.Split();
```

This method also returns the newly created node.

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
can attach to each other, so `start` and `end` are not guaranteed to be of the same type as `road`.

## Interpolating the curve
A point on the curve drawn through the nodes of the item can be calculated with the [`InterpolateCurve`](xref:TruckLib.ScsMap.PolylineItem.InterpolateCurve*)
and [`InterpolateCurveDist`](xref:TruckLib.ScsMap.PolylineItem.InterpolateCurveDist*) methods. The former takes an interpolation parameter
between 0 and 1; the latter expects a distance in meters and returns the point which is _n_ meters away from the backward node.

## Placing items along the path
The method [`CreateItemsAlongPath`](xref:TruckLib.ScsMap.PolylineItem.CreateItemsAlongPath*) can be used to place objects at equidistant
intervals along the path of a chain of polyline items. It takes as parameters the starting and ending item, the interval in meters,
and a callback function in which you create the map item.

In its simplest form, calling the method can look like this:

```cs
PolylineItem.CreateItemsAlongPath(start, end, 10f, (container, point) =>
{
    Model model = Model.Add(container, point.Position, "greece_29000", "var1", "default");
    model.Node.Rotation = point.Rotation;
    return [model];
});
```

This will create a Model item every 10 meters from the backward node of `start` to the backward node of `end`.

Here is a more complicated example, which places German reflector posts along both sides of a road with
the correct orientation:

```cs
PolylineItem.CreateItemsAlongPath(start, end, 50f, (container, point) =>
{
    Vector3 normal = Vector3.Normalize(Vector3.Transform(Vector3.UnitX, point.Rotation));

    float distFromRoad = 6;
    Vector3 leftPos = point.Position + normal * -distFromRoad;
    Vector3 rightPos = point.Position + normal * distFromRoad;

    Sign leftPost = Sign.Add(container, leftPos, "ch_2y07d");
    leftPost.Node.Rotation = point.Rotation * Quaternion.CreateFromYawPitchRoll((float)Math.PI, 0, 0);
    Sign rightPost = Sign.Add(container, rightPos, "ch_2y07d");
    rightPost.Node.Rotation = point.Rotation;

    return [leftPost, rightPost];
});
```
