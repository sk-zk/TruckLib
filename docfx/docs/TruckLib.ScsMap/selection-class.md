# The Selection class

Selections (.sbd files) can be created with the [`Selection`](xref:TruckLib.ScsMap.Selection) class. It implements the same interface
as [`Map`](xref:TruckLib.ScsMap.Map), so it can be used the same way.

Here's a brief example:

```cs
using System.Numerics;
using TruckLib.ScsMap;

Selection selection = new();
Model model = Model.Add(selection, new Vector3(10f, 0f, 10f), 
    "dlc_no_471", "brick", "default"
);
selection.Save("example.sbd");
```

Note that, just like with maps, a recalculation (Map > Recompute map) is required when importing this selection with the official editor
because TruckLib does not calculate the bounding boxes of items.

## Origin
Selections have an origin point which the editor subtracts from all nodes before adding the items. You can modify this origin point via the
[`Origin`](xref:TruckLib.ScsMap.Selection.Origin) property. In addition, there is a [`CenterOrigin()`](xref:TruckLib.ScsMap.Selection.CenterOrigin*)
method which sets the origin to the center of all nodes, which is what the official editor does when exporting a selection.

## Importing to a [`Map`](xref:TruckLib.ScsMap.Map)
You can import a selection into a map with the [`Import()`](xref:TruckLib.ScsMap.Map.Import*) method:

```cs
map.Import(selection, new(72.7f, 0f, 27.2f));
```

This will add a copy of the items in `selection` to `map` at the specified offset.
UIDs of the nodes and items will differ from the originals in `selection`.