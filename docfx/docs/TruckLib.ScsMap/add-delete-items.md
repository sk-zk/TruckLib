# Adding and deleting map items

## Adding a map item
To add a map item, call the static `Add` method of the respective item's class. This will construct the object,
add map nodes for the item, add the item to the map, and then return the object.

Let's [add a Model](xref:TruckLib.ScsMap.Model.Add*) to the map. The first parameter of any `Add` method is the map,
compound or Selection the item will be added to; what follows next depends on the item. In this case, it is the 
position of the model, the unit name of the model, and its variant and look:

```cs
Model model = Model.Add(
  map,          // container
  new Vector3(10f, 0f, 10f), // position 
  "dlc_no_471", // unit name of "house_01_sc"
  "brick",      // model variant
  "default"     // model look
);
```

The map now contains a node at the specified position and a Model which references this node. You can use the returned object
to further modify the item:

```cs
model.Node.Rotation = Quaternion.CreateFromYawPitchRoll(1.23f, 0, 0);
model.WaterReflection = true;
```

## Deleting a map item
To delete an item from a map, call its [`Delete`](xref:TruckLib.ScsMap.IItemContainer.Delete(TruckLib.ScsMap.MapItem)) method with the item:

```cs
map.Delete(model);
```

Nodes which are exclusively used by this item will also be deleted.
When deleting a prefab, the prefab's slave items will be deleted as well.
