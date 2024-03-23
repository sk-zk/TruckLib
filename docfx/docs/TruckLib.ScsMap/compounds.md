# Working with compounds

## Creating a compound
To compound items, you can either start with an empty compound item and add to it, or you can
compound existing items.

### Starting with an empty compound
To create an empty compound, call the static [`Add`](xref:TruckLib.ScsMap.Compound.Add*) method of the class, like with any other map item:

```cs
Compound compound = Compound.Add(map, new Vector3(42, 0, 84));
```

The second parameter is the center point of the compound.

Adding new items to a compound works the same way as adding them to the map directly &ndash; just use
the compound item as the first parameter:

```cs
Model model = Model.Add(compound, new Vector3(40, 0, 80), "dlc_no_654", "default", "default");
```

### Compound existing items
The [`CompoundItems`](xref:TruckLib.ScsMap.Map.CompoundItems*) method of a map object takes a list of existing map items
and compounds them. Its return value is the newly created compound item.

```cs
Compound compound = map.CompoundItems(new[] {model1, model2});
```

## Removing an item from a compound
Like adding to a compound, [removing](xref:TruckLib.ScsMap.Compound.Delete(TruckLib.ScsMap.MapItem)) from a compound also has the same interface as removing from a map:

```cs
Compound compound = Compound.Add(map, new Vector3(42, 0, 84));
Model model = Model.Add(compound, new Vector3(40, 0, 80), "dlc_no_654", "default", "default");
compound.Delete(model);
// The compound is now empty again.
```

## Uncompounding
Dissolving a compound &ndash; moving its items back into the map itself and deleting the compound item &ndash; is accomplished with
[`UncompoundItems`](xref:TruckLib.ScsMap.Map.UncompoundItems*):

```cs
map.UncompoundItems(compound);
```