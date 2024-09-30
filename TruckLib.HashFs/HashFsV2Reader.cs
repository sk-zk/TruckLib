using Ionic.Zlib;
using GisDeflate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TruckLib.HashFs.Dds;

namespace TruckLib.HashFs
{
    internal class HashFsV2Reader : HashFsReaderBase
    {
        public override ushort Version => 2;

        public Platform Platform { get; private set; }

        private ulong entryTableStart;
        private uint entryTableLength;
        private ulong metadataTableStart;
        private uint metadataTableLength;

        /// <inheritdoc/>
        public override DirectoryListing GetDirectoryListing(
            IEntry entry, bool filesOnly = false)
        {
            var bytes = GetEntryContent(entry);
            using var ms = new MemoryStream(bytes);
            using var dirReader = new BinaryReader(ms);

            var count = dirReader.ReadUInt32();
            var stringLengths = dirReader.ReadBytes((int)count);

            var subdirs = new List<string>();
            var files = new List<string>();

            for (int i = 0; i < count; i++)
            {
                var str = Encoding.UTF8.GetString(dirReader.ReadBytes(stringLengths[i]));
                // is directory
                if (str.StartsWith('/'))
                {
                    if (filesOnly) continue;
                    var subPath = str[1..] + "/";
                    subdirs.Add(subPath);
                }
                // is file
                else
                {
                    files.Add(str);
                }
            }

            return new DirectoryListing(subdirs, files);
        }

        /// <inheritdoc/>
        public override byte[][] Extract(IEntry entry, string path)
        {
            if (!Entries.ContainsValue(entry))
                throw new FileNotFoundException();

            if (entry is EntryV2 v2 && v2.TobjMetadata != null)
            {
                using var tobjMs = new MemoryStream();
                RecreateTobj(v2, path, tobjMs);

                using var ddsMs = new MemoryStream();
                RecreateDds(v2, ddsMs);

                return [tobjMs.ToArray(), ddsMs.ToArray()];
            }
            else
            {
                return [GetEntryContent(entry)];
            }
        }

        /// <inheritdoc/>
        public override void ExtractToFile(IEntry entry, string entryPath, string outputPath)
        {
            if (entry.IsDirectory)
            {
                throw new ArgumentException("This is a directory.", nameof(entry));
            }

            if (entry.Size == 0)
            {
                // create an empty file
                File.Create(outputPath).Dispose();
                return;
            }

            Reader.BaseStream.Position = (long)entry.Offset;
            using var fileStream = new FileStream(outputPath, FileMode.Create);
            if (entry is EntryV2 v2 && v2.TobjMetadata != null)
            {
                RecreateTobj(v2, entryPath, fileStream);
                using var ddsFileStream = new FileStream(
                    System.IO.Path.ChangeExtension(outputPath, "dds"),
                    FileMode.Create);
                RecreateDds(v2, ddsFileStream);
            }
            else if (entry.IsCompressed)
            {
                var zlibStream = new ZlibStream(Reader.BaseStream, CompressionMode.Decompress);
                try
                {
                    zlibStream.CopyTo(fileStream, (int)entry.CompressedSize);
                }
                catch (ZlibException)
                {
                    throw;
                }
            }
            else
            {
                var buffer = new byte[(int)entry.Size];
                Reader.BaseStream.Read(buffer, 0, (int)entry.Size);
                fileStream.Write(buffer, 0, (int)entry.Size);
            }
        }

        private void RecreateTobj(EntryV2 entry, string tobjPath, Stream stream)
        {
            using var w = new BinaryWriter(stream);
            var tobj = entry.TobjMetadata.Value.AsTobj(tobjPath);
            tobj.Serialize(w);
        }

        private void RecreateDds(EntryV2 entry, Stream stream)
        {
            var dds = new DdsFile();
            dds.Header = new DdsHeader()
            {
                IsCapsValid = true,
                IsHeightValid = true,
                IsWidthValid = true,
                IsPixelFormatValid = true,
                CapsTexture = true,
                Width = (uint)entry.TobjMetadata.Value.TextureWidth,
                Height = (uint)entry.TobjMetadata.Value.TextureHeight,
                IsMipMapCountValid = entry.TobjMetadata.Value.MipmapCount > 0,
                MipMapCount = entry.TobjMetadata.Value.MipmapCount,
            };
            dds.Header.PixelFormat = new DdsPixelFormat()
            {
                FourCC = DdsPixelFormat.FourCC_DX10,
                HasCompressedRgbData = true,
            };
            dds.HeaderDxt10 = new DdsHeaderDxt10()
            {
                Format = (DxgiFormat)entry.TobjMetadata.Value.Format,
                ArraySize = 1,
                ResourceDimension = D3d10ResourceDimension.Texture2d,
            };

            if (entry.TobjMetadata.Value.MipmapCount > 1)
            {
                dds.Header.IsMipMapCountValid = true;
                dds.Header.CapsMipMap = true;
                dds.Header.CapsComplex = true;
            }

            if (entry.TobjMetadata.Value.IsCube)
            {
                dds.Header.CapsComplex = true;
                dds.Header.Caps2Cubemap = true;
                dds.Header.Caps2CubemapPositiveX = true;
                dds.Header.Caps2CubemapNegativeX = true;
                dds.Header.Caps2CubemapPositiveY = true;
                dds.Header.Caps2CubemapNegativeY = true;
                dds.Header.Caps2CubemapPositiveZ = true;
                dds.Header.Caps2CubemapNegativeZ = true;
                dds.HeaderDxt10.MiscFlag = D3d10ResourceMiscFlag.TextureCube;
            }

            var data = GetEntryContent(entry);
            if (entry.IsCompressed)
            {
                data = GDeflate.Decompress(data);
            }
            dds.Data = DdsUtils.ConvertDecompBytesToDdsBytes(entry, dds, data);

            using var w = new BinaryWriter(stream);
            dds.Serialize(w);
        }

        internal void ParseHeader()
        {
            Salt = Reader.ReadUInt16();

            var hashMethod = new string(Reader.ReadChars(4));
            if (hashMethod != SupportedHashMethod)
                throw new NotSupportedException($"Hash method \"{hashMethod}\" is not supported.");

            var entriesCount = Reader.ReadUInt32();
            entryTableLength = Reader.ReadUInt32();
            var metadataEntriesCount = Reader.ReadUInt32();
            metadataTableLength = Reader.ReadUInt32();
            entryTableStart = Reader.ReadUInt64();
            metadataTableStart = Reader.ReadUInt64();
            var securityDescriptorOffset = Reader.ReadUInt32();
            Platform = (Platform)Reader.ReadByte();
        }

        internal void ParseTables()
        {
            var entryTable = ReadEntryTable();
            var metaEntries = ReadMetadataTable();

            foreach (var entry in entryTable)
            {
                var meta = metaEntries[entry.MetadataIndex + entry.MetadataCount];
                if (meta is PlainEntry plain)
                {
                    Entries.Add(entry.Hash, new EntryV2()
                    {
                        Hash = entry.Hash,
                        Offset = plain.Offset,
                        CompressedSize = plain.CompressedSize,
                        Size = plain.Size,
                        IsCompressed = (plain.Flags & 0x10) != 0,
                        IsDirectory = false,
                    });
                }
                else if (meta is DirectoryEntry dir)
                {
                    Entries.Add(entry.Hash, new EntryV2()
                    {
                        Hash = entry.Hash,
                        Offset = dir.Offset,
                        CompressedSize = dir.CompressedSize,
                        Size = dir.Size,
                        IsCompressed = (dir.Flags & 0x10) != 0,
                        IsDirectory = true,
                    });
                }
                else if (meta is TobjEntry tobj)
                {
                    Entries.Add(entry.Hash, new EntryV2()
                    {
                        Hash = entry.Hash,
                        Offset = meta.Offset,
                        CompressedSize = meta.CompressedSize,
                        Size = tobj.CompressedSize,
                        IsCompressed = tobj.IsCompressed,
                        IsDirectory = false,
                        TobjMetadata = tobj.Metadata,
                    });
                }
                else
                {
                    throw new NotImplementedException($"Unhandled MetadataTableEntry type {meta.GetType().Name}");
                }             
            }
        }

        private Dictionary<uint, IMetadataTableEntry> ReadMetadataTable()
        {
            Reader.BaseStream.Position = (long)metadataTableStart;

            var metadataTableBuffer = 
                ZlibStream.UncompressBuffer(Reader.ReadBytes((int)metadataTableLength));
            using var metadataTableStream = new MemoryStream(metadataTableBuffer);
            using var r = new BinaryReader(metadataTableStream);
            var metaEntries = new Dictionary<uint, IMetadataTableEntry>();

            const ulong blockSize = 16UL;
            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var indexBytes = r.ReadBytes(3);
                var index = indexBytes[0]
                    + (indexBytes[1] << 8)
                    + (indexBytes[2] << 16);
                var type = (MetadataTableEntryType)r.ReadByte();

                if (type == MetadataTableEntryType.Plain)
                {
                    // a regular file.
                    var compressedSizeBytes = r.ReadBytes(3);
                    var compressedSize = compressedSizeBytes[0]
                        + (compressedSizeBytes[1] << 8)
                        + (compressedSizeBytes[2] << 16);
                    var flags = r.ReadByte();
                    var size = r.ReadUInt32();
                    var unknown2 = r.ReadUInt32();
                    var offsetBlock = r.ReadUInt32();

                    var metaEntry = new PlainEntry
                    {
                        Index = (uint)index,
                        Offset = offsetBlock * blockSize,
                        CompressedSize = (uint)compressedSize,
                        Size = size,
                        Flags = flags
                    };
                    metaEntries.Add((uint)index, metaEntry);
                }
                else if (type == MetadataTableEntryType.Directory)
                {
                    // a directory listing.
                    var compressedSizeBytes = r.ReadBytes(3);
                    var compressedSize = compressedSizeBytes[0]
                        + (compressedSizeBytes[1] << 8)
                        + (compressedSizeBytes[2] << 16);
                    var flags = r.ReadByte();
                    var size = r.ReadUInt32();
                    var unknown2 = r.ReadUInt32();
                    var offsetBlock = r.ReadUInt32();

                    var metaEntry = new DirectoryEntry
                    {
                        Index = (uint)index,
                        Offset = offsetBlock * blockSize,
                        CompressedSize = (uint)compressedSize,
                        Size = size,
                        Flags = flags
                    };
                    metaEntries.Add((uint)index, metaEntry);
                }
                else if (type == MetadataTableEntryType.Image)
                {   
                    // a packed .tobj/.dds file.
                    // this is probably not the correct way to read that metadata,
                    // but it works, so whatever.
                    var meta = new PackedTobjDdsMetadata();
                    var unknown1 = r.ReadUInt64();
                    meta.TextureWidth = r.ReadUInt16() + 1;
                    meta.TextureHeight = r.ReadUInt16() + 1;
                    meta.ImgFlags = new FlagField(r.ReadUInt32());
                    meta.SampleFlags = new FlagField(r.ReadUInt32());
                    var compressedSizeBytes = r.ReadBytes(3);
                    var compressedSize = compressedSizeBytes[0]
                        + (compressedSizeBytes[1] << 8)
                        + (compressedSizeBytes[2] << 16);
                    var flags = r.ReadByte();
                    var unknown3 = r.ReadBytes(8);
                    var offsetBlock = r.ReadUInt32();

                    var metaEntry = new TobjEntry
                    {
                        Index = (uint)index,
                        Offset = offsetBlock * blockSize,
                        CompressedSize = (uint)compressedSize,
                        IsCompressed = (flags & 0xF0) != 0,
                        Metadata = meta
                    };
                    metaEntries.Add((uint)index, metaEntry);
                }
                else
                {
                    throw new NotImplementedException($"Unhandled entry type {type}");
                }
            }

            return metaEntries;
        }

        private Span<EntryTableEntry> ReadEntryTable()
        {
            Reader.BaseStream.Position = (long)entryTableStart;
            var entryTableBuffer = ZlibStream.UncompressBuffer(Reader.ReadBytes((int)entryTableLength));
            var entryTable = MemoryMarshal.Cast<byte, EntryTableEntry>(entryTableBuffer.AsSpan());
            entryTable.Sort((x, y) => (int)(x.MetadataIndex - y.MetadataIndex));
            return entryTable;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct EntryTableEntry
    {
        [FieldOffset(0)]
        public ulong Hash;

        [FieldOffset(8)]
        public uint MetadataIndex;

        [FieldOffset(12)]
        public ushort MetadataCount;

        [FieldOffset(14)]
        public ushort Flags;
    }

    internal interface IMetadataTableEntry
    {
        public uint Index { get; set; }
        public ulong Offset { get; set; }
        public uint CompressedSize { get; set; }
    }

    internal struct PlainEntry : IMetadataTableEntry
    {
        public uint Index { get; set; }
        public ulong Offset { get; set; }
        public uint CompressedSize { get; set; }
        public uint Size { get; set; }
        public byte Flags { get; set; }
    }

    internal struct DirectoryEntry : IMetadataTableEntry
    {
        public uint Index { get; set; }
        public ulong Offset { get; set; }
        public uint CompressedSize { get; set; }
        public uint Size { get; set; }
        public byte Flags { get; set; }
    }

    internal struct TobjEntry : IMetadataTableEntry
    {
        public uint Index { get; set; }
        public ulong Offset { get; set; }
        public uint CompressedSize { get; set; }
        public bool IsCompressed { get; set; }
        public PackedTobjDdsMetadata Metadata { get; set; }
    }

    internal enum MetadataTableEntryType
    {
        Image = 1,
        Sample = 2,
        MipProxy = 3,
        InlineDirectory = 4,
        Plain = 128,
        Directory = 129,
        Mip0 = 130,
        Mip1 = 131,
        MipTail = 132,
    }
}