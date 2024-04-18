using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TruckLib.HashFs
{
    internal class HashFsV1Reader : HashFsReaderBase
    {
        private uint entriesCount;
        private uint startOffset;

        /// <inheritdoc/>
        public override (List<string> Subdirs, List<string> Files) GetDirectoryListing(
            IEntry entry, bool filesOnly = false)
        {
            var dirEntries = Encoding.ASCII.GetString(GetEntryContent(entry)).Split(new[] { '\r', '\n' });

            const string dirMarker = "*";
            var subdirs = new List<string>();
            var files = new List<string>();
            for (int i = 0; i < dirEntries.Length; i++)
            {
                // is directory
                if (dirEntries[i].StartsWith(dirMarker))
                {
                    if (filesOnly) continue;
                    var subPath = dirEntries[i][1..] + "/";
                    subdirs.Add(subPath);
                }
                // is file
                else
                {
                    files.Add(dirEntries[i]);
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

            entriesCount = Reader.ReadUInt32();
            startOffset = Reader.ReadUInt32();
        }

        internal void ParseEntryTable(bool forceEntryTableAtEnd)
        {
            Reader.BaseStream.Position = forceEntryTableAtEnd 
                ? Reader.BaseStream.Length - (entriesCount * 32) 
                : startOffset;

            for (int i = 0; i < entriesCount; i++)
            {
                var entry = new EntryV1
                {
                    Hash = Reader.ReadUInt64(),
                    Offset = Reader.ReadUInt64(),
                    Flags = new FlagField(Reader.ReadUInt32()),
                    Crc = Reader.ReadUInt32(),
                    Size = Reader.ReadUInt32(),
                    CompressedSize = Reader.ReadUInt32()
                };
                Entries.Add(entry.Hash, entry);
            }
        }
    }
}