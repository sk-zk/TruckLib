using Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.HashFs
{
    /// <summary>
    /// A simple HashFS reader for extracting files from HashFS archives.
    /// </summary>
    public class HashFsReader : IDisposable
    {
        /// <summary>
        /// Gets the file path of the HashFS archive which this reader is reading from.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The entries in this archive.
        /// </summary>
        public Dictionary<ulong, Entry> Entries { get; private set; } = new();

        /// <summary>
        /// Gets or sets the salt which is prepended to paths before hashing them.
        /// </summary>
        public ushort Salt { get; set; }

        private const uint Magic = 0x23534353; // as ascii: "SCS#"
        private const ushort SupportedVersion = 1;
        private const string SupportedHashMethod = "CITY";
        private const string RootPath = "/";

        private BinaryReader reader;

        private string HashMethod;
        private uint EntriesCount;
        private uint StartOffset;


        /// <summary>
        /// Opens a HashFS archive.
        /// </summary>
        /// <param name="path">The path to the HashFS archive.</param>
        /// <param name="forceEntryHeadersAtEnd">If true, the entry headers will be read
        /// from the end of the file, regardless of where the archive header says they are located.</param>
        /// <returns>A HashFsReader object.</returns>
        public static HashFsReader Open(string path, bool forceEntryHeadersAtEnd = false)
        {
            var hfr = new HashFsReader
            {
                Path = path,
                reader = new BinaryReader(new FileStream(path, FileMode.Open))
            };
            hfr.ParseHeader();
            hfr.CacheEntryHeaders(forceEntryHeadersAtEnd);
            return hfr;
        }

        /// <summary>
        /// Checks if an entry exists and returns its type if it does.
        /// </summary>
        /// <param name="path">The path of the entry in the archive.</param>
        /// <returns>Its type.</returns>
        public EntryType EntryExists(string path)
        {
            path = RemoveTrailingSlash(path);
            var hash = HashPath(path);
            if (Entries.TryGetValue(hash, out var entry))
            {
                return entry.IsDirectory
                    ? EntryType.Directory
                    : EntryType.File;
            }
            return EntryType.NotFound;
        }

        /// <summary>
        /// Extracts and decompresses an entry to memory.
        /// </summary>
        /// <param name="path">The path of the entry in the archive.</param>
        /// <returns>The extracted entry as byte array.</returns>
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
        /// <returns>The extracted entry as byte array.</returns>
        public byte[] Extract(Entry entry)
        {
            if (!Entries.ContainsValue(entry))
                throw new FileNotFoundException();

            return GetEntryContent(entry);
        }

        /// <summary>
        /// Extracts and decompresses an entry to a file.
        /// </summary>
        /// <param name="path">The path of the entry in the archive.</param>
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

            reader.BaseStream.Position = (long)entry.Offset;
            using var fileStream = new FileStream(outputPath, FileMode.Create);
            if (entry.IsCompressed)
            {
                var zlibStream = new ZlibStream(reader.BaseStream, CompressionMode.Decompress);
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
                reader.BaseStream.Read(buffer, 0, (int)entry.Size);
                fileStream.Write(buffer, 0, (int)entry.Size);
            }
        }

        /// <summary>
        /// Returns a list of subdirectories and files in the given directory.
        /// </summary>
        /// <param name="path">The path of the directory in the archive.</param>
        /// <param name="filesOnly">Whether only files should be returned.</param>
        /// <param name="returnAbsolute">Whether the returned paths should be made absolute.</param>
        /// <returns>A list of subdirectories and files in the given directory.</returns>
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
        /// <param name="entry">The entry header of the directory.</param>
        /// <param name="filesOnly">Whether only files should be returned.</param>
        /// <returns>A list of subdirectories and files in the given directory.</returns>
        public (List<string> Subdirs, List<string> Files) GetDirectoryListing(
            Entry entry, bool filesOnly = false)
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

        /// <summary>
        /// Retrieves the entry header for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The entry header.</returns>
        public Entry GetEntryHeader(string path)
        {
            ulong hash = HashPath(path);
            var entry = Entries[hash];
            return entry;
        }

        /// <summary>
        /// Hashes a file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="salt">If set, this salt will be used to hash the path rather than
        /// the one specified by the archive's header.</param>
        /// <returns>The hash of the path.</returns>
        public ulong HashPath(string path, uint? salt = null)
        {
            if (path != "" && path.StartsWith("/"))
                path = path[1..];

            salt ??= Salt;
            if (salt != 0)
                path = salt + path;

            var bytes = Encoding.UTF8.GetBytes(path);
            var hash = CityHash.CityHash64(bytes, (ulong)bytes.Length);
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

        private void CacheEntryHeaders(bool forceEntryHeadersAtEnd)
        {
            if (forceEntryHeadersAtEnd)
            {
                reader.BaseStream.Position = reader.BaseStream.Length - (EntriesCount * 32);
            } 
            else
            {
                reader.BaseStream.Position = StartOffset;
            }

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
                Entries.Add(entry.Hash, entry);
            }
        }

        private string RemoveTrailingSlash(string path)
        {
            if (path.EndsWith("/") && path != RootPath)
                path = path[0..^1];
            return path;
        }

        /// <summary>
        /// Closes the file stream.
        /// </summary>
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
        /// <summary>
        /// The path does not exist in the archive.
        /// </summary>
        NotFound = -1,

        /// <summary>
        /// The path points to a file.
        /// </summary>
        File = 0,

        /// <summary>
        /// The path points to a directory listing.
        /// </summary>
        Directory = 1
    }
}
