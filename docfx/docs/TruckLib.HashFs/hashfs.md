# Reading HashFS (.scs) archives

Euro Truck Simulator 2 and American Truck Simulator store their game assets in HashFS files with the extension `.scs`.
Mods may also be distributed in this format, though a ZIP file renamed to `.scs` is more common.

In a HashFS archive, the identifier of the contained files is a CityHash64 hash of its full path, encoded as UTF-8.
This means that a file can be retrieved by its path, but it is not possible to list every path in the archive, as the hashes
cannot be reversed. To deal with this, archives can optionally contain directory listings, which are special text files
with the path of a directory, enumerating the files and subdirectories it contains. All but three of the games' `.scs`
files contain full directory listings, but mods may not &ndash; the top level listing in particular can be omitted to
prevent the official extractor from extracting anything.

You can extract files from a HashFS archive with the [`HashFsReader`](xref:TruckLib.HashFs.HashFsReader) class.

## Opening an archive
To open a HashFS file, call the static [`Open`](xref:TruckLib.HashFs.HashFsReader.Open*) method of the `HashFsReader` class:

```cs
using TruckLib.HashFs;

using HashFsReader reader = HashFsReader.Open(@"E:\SteamLibrary\steamapps\common\Euro Truck Simulator 2\def.scs");
```

This will create a `HashFsReader` instance which you can use to extract files from the archive.

## Finding entries

### Known paths
The [`EntryExists`](xref:TruckLib.HashFs.HashFsReader.EntryExists*) method will tell you if the given path exists in the archive, 
and if yes, whether it is a directory or a file:

```cs
EntryType type = reader.EntryExists("/def/world/prefab.sii");
```

[`EntryType`](xref:TruckLib.HashFs.EntryType) has the values `Directory`, `File`, or `NotFound`.

### Directory listings
You can retrieve the contents of a directory with the [`GetDirectoryListing`](xref:TruckLib.HashFs.HashFsReader.GetDirectoryListing*) method:

```cs
// Get the top level of the archive
var (subdirs, files) = reader.GetDirectoryListing("/");
```

Keep in mind that, as mentioned above, directory listings are not required to exist.

## Extracting entries

Use the [`Extract`](xref:TruckLib.HashFs.HashFsReader.Extract*) method to extract a file to a byte array:

```cs
byte[] data = reader.Extract("/def/world/prefab.sii");
```

Alternatively, you can write the entry to disk with [`ExtractToFile`](xref:TruckLib.HashFs.HashFsReader.ExtractToFile*):

```cs
reader.ExtractToFile("/def/world/prefab.sii", "./prefab.sii");
```
