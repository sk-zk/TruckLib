# Writing HashFS (.scs) archives

TruckLib provides the classes [`HashFsV1Writer`](xref:TruckLib.HashFs.HashFsV1Writer) and [`HashFsV2Writer`](xref:TruckLib.HashFs.HashFsV2Writer)
for writing HashFS v1 and v2 archives respectively.

## Usage
First, construct a new writer and stage files for packing with the `Add` method:

```cs
using TruckLib.HashFs;

HashFsV2Writer writer = new();
writer.Add(@"D:\foo\bar\manifest.sii", "/manifest.sii");
writer.Add(@"C:\stuff\my_house.pmd", "/model/building/my_house.pmd");
```

The first parameter of this method is either the path to a file on your file system, a stream, or a byte array.
The second parameter is the absolute path which this file will have in the HashFS archive.

`Add` will throw `ArgumentException` if the archive path is empty or invalid.

Once you've specified the files you want to pack, call `Save` with either an output path or output stream:

```cs
writer.Save("example.scs");
```

The files will now be opened, compressed, and written to a new HashFS archive.

## Textures in HashFS v2
In HashFS v2, `.tobj`/`.dds` pairs are packed into just one entry whose path will be that of the `.tobj` file.
If either of these two files cannot be parsed or the `.dds` file referenced in the `.tobj` file does not exist,
`Save` will throw a [`TexturePackingException`](xref:TruckLib.HashFs.HashFsV2.TexturePackingException).
Loose `.dds` files which are not referenced by any `.tobj` file will not be added to the archive.
