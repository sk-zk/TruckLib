using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TruckLib.HashFs
{
    internal class HashFsV2Reader : HashFsReaderBase
    {    
        private ulong entryTableStart;
        private uint entryTableLength;
        private ulong metadataTableStart;
        private uint metadataTableLength;

        /// <inheritdoc/>
        public override (List<string> Subdirs, List<string> Files) GetDirectoryListing(
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
                if (str.StartsWith("/"))
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

            return (subdirs, files);
        }

        internal void ParseHeader()
        {
            Salt = Reader.ReadUInt16();

            var hashMethod = new string(Reader.ReadChars(4));
            if (hashMethod != SupportedHashMethod)
                throw new NotSupportedException($"Hash method \"{hashMethod}\" is not supported.");

            var entriesCount = Reader.ReadUInt32();
            entryTableLength = Reader.ReadUInt32();
            var unknown2 = Reader.ReadUInt32();
            metadataTableLength = Reader.ReadUInt32();
            entryTableStart = Reader.ReadUInt64();
            metadataTableStart = Reader.ReadUInt64();
            var unknown8 = Reader.ReadUInt32();
        }

        internal void ParseTables()
        {
            var entryTable = ReadEntryTable();
            var metaEntries = ReadMetadataTable();

            foreach (var entry in entryTable)
            {
                var offsetToMetadataTableIndex = entry.FlagsAndStuffIGuess & 0xFF;
                var meta = metaEntries[entry.Index + offsetToMetadataTableIndex];
                Entries.Add(entry.Hash, new EntryV2()
                {
                    Hash = entry.Hash,
                    Offset = meta.Offset,
                    CompressedSize = meta.CompressedSize,
                    Size = meta.Size ?? meta.CompressedSize,
                    IsCompressed = !meta.IsTobj && (meta.Flags2 & 0x10) != 0,
                    IsDirectory = !meta.IsTobj && (meta.Flags1 & 0x1) != 0,
                    IsTobj = meta.IsTobj,
                });
            }
        }

        private Dictionary<uint, MetadataTableEntry> ReadMetadataTable()
        {
            Reader.BaseStream.Position = (long)metadataTableStart;

            var metadataTableBuffer = 
                ZlibStream.UncompressBuffer(Reader.ReadBytes((int)metadataTableLength));
            using var metadataTableStream = new MemoryStream(metadataTableBuffer);
            using var r = new BinaryReader(metadataTableStream);
            var metaEntries = new Dictionary<uint, MetadataTableEntry>();

            const ulong blockSize = 16UL;
            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var indexBytes = r.ReadBytes(3);
                var index = indexBytes[0]
                    + (indexBytes[1] << 8)
                    + (indexBytes[2] << 16);
                var flags1 = r.ReadByte();

                if ((flags1 & 0x80) != 0)
                {
                    // a regular file or directory listing.
                    var compressedSizeBytes = r.ReadBytes(3);
                    var compressedSize = compressedSizeBytes[0]
                        + (compressedSizeBytes[1] << 8)
                        + (compressedSizeBytes[2] << 16);
                    var flags2 = r.ReadByte();
                    var size = r.ReadUInt32();
                    var unknown2 = r.ReadUInt32();
                    var offsetBlock = r.ReadUInt32();

                    var metaEntry = new MetadataTableEntry
                    {
                        Index = (uint)index,
                        Offset = offsetBlock * blockSize,
                        CompressedSize = (uint)compressedSize,
                        Size = size,
                        Flags1 = flags1,
                        Flags2 = flags2
                    };
                    metaEntries.Add((uint)index, metaEntry);
                }
                else
                {
                    // a packed tobj/dds thing.
                    // don't know how to unpack these.
                    var unknown1 = r.ReadUInt64();
                    var textureWidth = r.ReadUInt16() + 1;
                    var textureHeight = r.ReadUInt16() + 1;
                    var unknown2 = r.ReadUInt64();
                    var compressedSizeBytes = r.ReadBytes(3);
                    var compressedSize = compressedSizeBytes[0]
                        + (compressedSizeBytes[1] << 8)
                        + (compressedSizeBytes[2] << 16);
                    var unknown3 = r.ReadBytes(9);
                    var offsetBlock = r.ReadUInt32();

                    var metaEntry = new MetadataTableEntry();
                    metaEntry.Index = (uint)index;
                    metaEntry.Offset = offsetBlock * blockSize;
                    metaEntry.CompressedSize = (uint)compressedSize;
                    metaEntry.Flags1 = flags1;
                    metaEntry.IsTobj = true;
                    metaEntries.Add((uint)index, metaEntry);
                }
            }

            return metaEntries;
        }

        private Span<EntryTableEntry> ReadEntryTable()
        {
            Reader.BaseStream.Position = (long)entryTableStart;
            var entryTableBuffer = ZlibStream.UncompressBuffer(Reader.ReadBytes((int)entryTableLength));
            var entryTable = MemoryMarshal.Cast<byte, EntryTableEntry>(entryTableBuffer.AsSpan());
            entryTable.Sort((x, y) => (int)(x.Index - y.Index));
            return entryTable;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct EntryTableEntry
    {
        [FieldOffset(0)]
        public ulong Hash;

        [FieldOffset(8)]
        public uint Index;

        [FieldOffset(12)]
        public uint FlagsAndStuffIGuess;
    }

    internal struct MetadataTableEntry
    {
        public uint Index;
        public ulong Offset;
        public uint CompressedSize;
        public uint? Size;
        public byte Flags1;
        public byte? Flags2;
        public bool IsTobj;
    }
}