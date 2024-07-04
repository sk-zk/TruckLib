# Reading HashFS (.scs) archives

Euro Truck Simulator 2 and American Truck Simulator store their game assets in HashFS files with the extension `.scs`.
Mods may also be distributed in this format, though a ZIP file renamed to `.scs` is also allowed.

In a HashFS archive, the identifier of the contained files is a CityHash64 hash of its full path (encoded as UTF-8).
This means that a file can be retrieved by its path, but it is not possible to list every path in the archive, as the hashes
cannot be reversed. To deal with this, archives can optionally contain directory listings, which are special files
with the path of a directory, enumerating the files and subdirectories it contains. All but three of the games' `.scs`
files contain full directory listings, but mods may not &ndash; the top level listing in particular can be omitted to
prevent the official extractor from extracting anything.

HashFS v2, introduced with game version 1.50, is supported. Note that, in this version, .tobj/.dds pairs are
dissolved into a packed .tobj entry from which TruckLib will reconstruct the original .tobj and .dds files
when said .tobj entry is requested.

## Opening an archive
To open a HashFS file, call the static [`Open`](xref:TruckLib.HashFs.HashFsReader.Open*) method of the `HashFsReader` class:

```cs
using TruckLib.HashFs;

using IHashFsReader reader = HashFsReader.Open(@"E:\SteamLibrary\steamapps\common\Euro Truck Simulator 2\def.scs");
```

Depending on the HashFS version of the archive, this will create a `HashFsReaderV1` or 
`HashFsReaderV2` instance as [`IHashFsReader`](xref:TruckLib.HashFs.IHashFsReader) which you can use to extract files.

## Finding entries

### Known paths
The [`EntryExists`](xref:TruckLib.HashFs.IHashFsReader.EntryExists*) method will tell you if the given path exists in the archive, 
and if yes, whether it is a directory or a file:

```cs
EntryType type = reader.EntryExists("/def/world/prefab.sii");
```

[`EntryType`](xref:TruckLib.HashFs.EntryType) has the values `Directory`, `File`, or `NotFound`.

### Directory listings
You can retrieve the contents of a directory with the [`GetDirectoryListing`](xref:TruckLib.HashFs.IHashFsReader.GetDirectoryListing*) method:

```cs
// Get the top level of the archive
var (subdirs, files) = reader.GetDirectoryListing("/");
```

Keep in mind that, as mentioned above, directory listings are not required to exist.

## Extracting entries
Use the [`Extract`](xref:TruckLib.HashFs.IHashFsReader.Extract*) method to extract an entry:

```cs
byte[][] data = reader.Extract("/def/world/prefab.sii");
```

This returns an array containing the extracted file(s) as `byte[]`.
The method will return one file except in one special case: In HashFS v2,
extracting a packed .tobj entry will return the reconstructed .tobj
and .dds files in that order.

Alternatively, you can write the entry to disk with [`ExtractToFile`](xref:TruckLib.HashFs.IHashFsReader.ExtractToFile*):

```cs
reader.ExtractToFile("/def/world/prefab.sii", "./prefab.sii");
```

In HashFS v2, extracting a packed .tobj entry will write both the reconstructed .tobj
and .dds file.
