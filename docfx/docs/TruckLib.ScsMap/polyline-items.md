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

In both cases, if there is already an item attached in the direction you wish to extend, a `InvalidOperationException` is thrown.

By default, the properties of item the method is called on are copied to the new item. If you would like it to have its default
properties instead, set the optional parameter `cloneSettings` to `false`.

## First and last item

To find the first or last item of a polyline chain given one of its members, call [`FindFirstItem`](xref:TruckLib.ScsMap.PolylineItem.FindFirstItem*)
or [`FindLastItem`](xref:TruckLib.ScsMap.PolylineItem.FindLastItem*) respectively:

```cs
PolylineItem start = road.FindFirstItem();
PolylineItem end = road.FindLastItem();
```

`start` and `end` are now the first and last polyline item of the chain `road` is a part of. Keep in mind that all polyline item types
can attach to each other, so `start` end `end` are not guaranteed to be of type [`Road`](xref:TruckLib.ScsMap.Road).