# Moving and translating map items

Items can be moved with their respective [`Move`](xref:TruckLib.ScsMap.MapItem.Move*) and
[`Translate`](xref:TruckLib.ScsMap.MapItem.Translate*) methods:

```cs
var model = Model.Add(map, new Vector3(10, 0, 10), "dlc_no_654", "default", "default");
model.Move(new Vector3(50, 0, 50));
// The model is now at (50, 0, 50).
```

```cs
var model = Model.Add(map, new Vector3(10, 0, 10), "dlc_no_654", "default", "default");
model.Translate(new Vector3(40, 0, 40));
// The model is now at (50, 0, 50).
```

The behavior of `Move` and `Translate` differs slightly depending on the item type. If the target is a ...

* [single node item](xref:TruckLib.ScsMap.SingleNodeItem) such as [Models](xref:TruckLib.ScsMap.Model), they simply move the node.

* [polyline item](xref:TruckLib.ScsMap.PolylineItem) such as [Roads](xref:TruckLib.ScsMap.Road), both the backward node and the forward node are moved,
since you are moving the whole item. To move only one of its nodes, call the method on the [`Node`](xref:TruckLib.ScsMap.PolylineItem.Node)
or [`ForwardNode`](xref:TruckLib.ScsMap.PolylineItem.ForwardNode) itself.

* [path item](xref:TruckLib.ScsMap.PathItem) such as [Movers](xref:TruckLib.ScsMap.Mover) or [polygon item](xref:TruckLib.ScsMap.PolygonItem)
such as [Traffic Areas](xref:TruckLib.ScsMap.TrafficArea), the 0th node will be moved to the position given to `Move`, and the other nodes
of the item will be translated relative to it. If you would like a different node to be the anchor of the `Move` method, use the
overload `Move(Vector3, int)` to specify its index.

* [prefab](xref:TruckLib.ScsMap.Prefab), they will also move all prefabs connected to the target.
The slave items of this and any connected prefab will be moved as well.

* [compound](xref:TruckLib.ScsMap.Compound), both the parent node and the contained map items are moved.
