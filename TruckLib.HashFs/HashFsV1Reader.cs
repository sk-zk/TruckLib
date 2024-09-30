using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TruckLib.HashFs
{
    internal class HashFsV1Reader : HashFsReaderBase
    {
        public override ushort Version => 1;

        private uint entriesCount;
        private uint startOffset;

        // Fixes Extractor#6; see comment in `ParseEntryTable`.
        private List<IEntry> duplicateDirListings = [];

        private readonly char[] newlineChars = ['\r', '\n'];

        /// <inheritdoc/>
        public override DirectoryListing GetDirectoryListing(
            IEntry entry, bool filesOnly = false)
        {
            var subdirs = new List<string>();
            var files = new List<string>();

            var additional = duplicateDirListings.Where(x => x.Hash == entry.Hash);

            foreach (var e in (new[] { entry }).Concat(additional)) 
            {
                var entryContent = GetEntryContent(e);
                var dirEntries = Encoding.ASCII.GetString(entryContent)
                    .Split(newlineChars, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < dirEntries.Length; i++)
                {
                    const string dirMarker = "*";
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
            }
            return new DirectoryListing(subdirs, files);
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
                var success = Entries.TryAdd(entry.Hash, entry);
                if (!success)
                {
                    var existing = Entries[entry.Hash];
                    if (existing.IsDirectory && entry.IsDirectory)
                    {
                        // Primitive fix for Extractor#6. If a directory listing
                        // is fragmented across multiple entries (which therefore
                        // have the same hash, because the hash is the path of
                        // the directory), just toss the extra ones into
                        // `duplicateDirListings` to be parsed when `GetDirectoryListing`
                        // is called for that path.
                        duplicateDirListings.Add(entry);
                    }
                    else
                    {
                        // just keep the first one.
                    }
                }
            }
        }
    }
}