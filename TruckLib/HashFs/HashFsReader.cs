using Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.HashFs
{
    /// <summary>
    /// Simple HashFS reader for extracting data from HashFS containers.
    /// </summary>
    public class HashFsReader : IDisposable
    {
        public string Path { get; private set; }
        public int EntryCount => entries.Count;

        private const uint Magic = 0x23534353; // as ascii: "SCS#"
        private const ushort SupportedVersion = 1;
        private const string SupportedHashMethod = "CITY";
        private const string RootPath = "/";

        private BinaryReader reader;

        private ushort Salt;
        private string HashMethod;
        private uint EntriesCount;
        private uint StartOffset;

        private Dictionary<ulong, Entry> entries = new();

        /// <summary>
        /// Opens a HashFS file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static HashFsReader Open(string path)
        {
            var hfr = new HashFsReader
            {
                Path = path,
                reader = new BinaryReader(new FileStream(path, FileMode.Open))
            };
            hfr.ParseHeader();
            hfr.CacheEntryHeaders();
            return hfr;
        }

        /// <summary>
        /// Checks if an entry exists and returns its type if it does.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public EntryType EntryExists(string path)
        {
            path = RemoveTrailingSlash(path);
            var hash = HashPath(path);
            if (entries.TryGetValue(hash, out var entry))
            {
                return entry.IsDirectory
                    ? EntryType.Directory
                    : EntryType.File;
            }
            return EntryType.NotFound;
        }

        public Dictionary<ulong, Entry> GetEntries()
        {
            return new Dictionary<ulong, Entry>(entries);
        }

        /// <summary>
        /// Extracts and decompresses an entry to memory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] Extract(string path)
        {
            if (EntryExists(path) == EntryType.NotFound)
                throw new FileNotFoundException();

            var entry = GetEntryHeader(path);
            return GetEntryContent(entry);
        }

        /// <summary>
        /// Extracts and decompresses an entry to memory.
        /// </summary>
        /// <param name="entry">The entry header of the file to extract.</param>
        /// <returns></returns>
        public byte[] Extract(Entry entry)
        {
            if (!entries.ContainsValue(entry))
                throw new FileNotFoundException();

            return GetEntryContent(entry);
        }

        /// <summary>
        /// Extracts and decompresses an entry to a file.
        /// </summary>
        /// <param name="path">The path of the file in the archive.</param>
        /// <param name="outputPath">The output path.</param>
        public void ExtractToFile(string path, string outputPath)
        {
            if (EntryExists(path) == EntryType.NotFound)
                throw new FileNotFoundException();

            var entry = GetEntryHeader(path);
            ExtractToFile(entry, outputPath);
        }

        /// <summary>
        /// Extracts and decompresses an entry to a file.
        /// </summary>
        /// <param name="entry">The entry header of the file to extract.</param>
        /// <param name="outputPath">The output path.</param>
        public void ExtractToFile(Entry entry, string outputPath)
        {
            reader.BaseStream.Position = (long)entry.Offset;
            using var fileStream = new FileStream(outputPath, FileMode.Create);
            if (entry.IsCompressed)
            {
                var zlibStream = new ZlibStream(reader.BaseStream, CompressionMode.Decompress);
                zlibStream.CopyTo(fileStream, (int)entry.CompressedSize);
            }
            else
            {
                var buffer = new byte[(int)entry.Size];
                reader.BaseStream.Read(buffer, 0, (int)entry.Size);
                fileStream.Write(buffer, 0, (int)entry.Size);
            }
        }

        /// <summary>
        /// Returns a list of subdirectories and files in the given directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="returnAbsolute"></param>
        /// <returns></returns>
        public (List<string> Subdirs, List<string> Files) GetDirectoryListing(
            string path, bool filesOnly = false, bool returnAbsolute = true)
        {
            path = RemoveTrailingSlash(path);

            var entryType = EntryExists(path);
            if (entryType == EntryType.NotFound)
                throw new FileNotFoundException();
            else if (entryType != EntryType.Directory)
                throw new ArgumentException($"\"{path}\" is not a directory.");

            var entry = GetEntryHeader(path);

            var (subdirs, files) = GetDirectoryListing(entry, filesOnly);

            if (returnAbsolute)
            {
                MakePathsAbsolute(path, subdirs);
                MakePathsAbsolute(path, files);
            }

            return (subdirs, files);

            static void MakePathsAbsolute(string parent, List<string> subdirs)
            {
                for (int i = 0; i < subdirs.Count; i++)
                {
                    if (parent == RootPath)
                        subdirs[i] = RootPath + subdirs[i];
                    else
                        subdirs[i] = $"{parent}/{subdirs[i]}";
                }
            }
        }

        /// <summary>
        /// Returns a list of subdirectories and files in the given directory.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="filesOnly"></param>
        /// <returns></returns>
        public (List<string> Subdirs, List<string> Files) GetDirectoryListing(
            Entry entry, bool filesOnly = false)
        {        
            var dirEntries = Encoding.ASCII.GetString(GetEntryContent(entry)).Split("\n");

            const string dirMarker = "*";
            var subdirs = new List<string>();
            var files = new List<string>();
            for (int i = 0; i < dirEntries.Length; i++)
            {
                // is directory
                if (dirEntries[i].StartsWith(dirMarker))
                {
                    if (filesOnly)
                        continue;
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

        private byte[] GetEntryContent(Entry entry)
        {
            reader.BaseStream.Position = (long)entry.Offset;
            byte[] file;
            if (entry.IsCompressed)
            {
                file = reader.ReadBytes((int)entry.CompressedSize);
                file = ZlibStream.UncompressBuffer(file);
            }
            else
            {
                // I hope this is correct
                file = reader.ReadBytes((int)entry.Size);
            }
            return file;
        }

        private Entry GetEntryHeader(string path)
        {
            ulong hash = HashPath(path);
            var entry = entries[hash];
            return entry;
        }

        /// <summary>
        /// Hashes a file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ulong HashPath(string path)
        {
            if(path != "")
                path = path[1..];

            if (Salt != 0)
                path = Salt + path;

            var hash = CityHash.CityHash64(Encoding.ASCII.GetBytes(path), (ulong)path.Length);
            return hash;
        }

        private void ParseHeader()
        {
            uint magic = reader.ReadUInt32();
            if (magic != Magic)
                throw new InvalidDataException("Probably not a HashFS file.");

            ushort version = reader.ReadUInt16();
            if (version != SupportedVersion)
                throw new NotSupportedException($"Version {version} is not supported.");

            Salt = reader.ReadUInt16();

            HashMethod = new string(reader.ReadChars(4));
            if (HashMethod != SupportedHashMethod)
                throw new NotSupportedException($"Hash method \"{HashMethod}\" is not supported.");

            EntriesCount = reader.ReadUInt32();
            StartOffset = reader.ReadUInt32();
        }

        private void CacheEntryHeaders()
        {
            reader.BaseStream.Position = StartOffset;

            for (int i = 0; i < EntriesCount; i++)
            {
                var entry = new Entry
                {
                    Hash = reader.ReadUInt64(),
                    Offset = reader.ReadUInt64(),
                    Flags = new FlagField(reader.ReadUInt32()),
                    Crc = reader.ReadUInt32(),
                    Size = reader.ReadUInt32(),
                    CompressedSize = reader.ReadUInt32()
                };
                entries.Add(entry.Hash, entry);
            }
        }

        private string RemoveTrailingSlash(string path)
        {
            if (path.EndsWith("/") && path != RootPath)
                path = path[0..^1];
            return path;
        }

        public void Dispose()
        {
            reader.BaseStream.Dispose();
            reader.Dispose();
        }
    }

    /// <summary>
    /// Return type for HashFsReader.EntryExists().
    /// </summary>
    public enum EntryType
    {
        NotFound = -1,
        File = 0,
        Directory = 1
    }
}
