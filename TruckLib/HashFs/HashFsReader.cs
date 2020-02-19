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
        const uint Magic = 0x23534353; // as ascii: "SCS#"
        const ushort SupportedVersion = 1;
        const string SupportedHashMethod = "CITY";
        const string rootPath = "/";

        BinaryReader reader;

        ushort Salt;
        string HashMethod;
        uint EntriesCount;
        uint StartOffset;

        Dictionary<ulong, Entry> entries = new Dictionary<ulong, Entry>();

        /// <summary>
        /// Opens a HashFS file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static HashFsReader Open(string path)
        {
            var hfr = new HashFsReader();
            hfr.reader = new BinaryReader(new FileStream(path, FileMode.Open));
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
            if(entries.ContainsKey(hash))
            {
                return entries[hash].IsDirectory ? 
                    EntryType.Directory : EntryType.File;
            }
            return EntryType.NotFound;
        }

        /// <summary>
        /// Extracts and decompresses a file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] ExtractEntry(string path)
        {
            if (EntryExists(path) == EntryType.NotFound)
                throw new FileNotFoundException();

            var entry = GetEntryHeader(path);
            return GetEntryContent(entry);
        }

        /// <summary>
        /// Returns a list of subdirectories and files in the given directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="absolute"></param>
        /// <returns></returns>
        public List<string> GetDirectoryListing(string path, bool absolute = true)
        {
            path = RemoveTrailingSlash(path);

            var entryType = EntryExists(path);
            if (entryType == EntryType.NotFound)
                throw new FileNotFoundException();
            else if (entryType != EntryType.Directory)
                throw new ArgumentException($"\"{path}\" is not a directory.");

            var entry = GetEntryHeader(path);
            var files = Encoding.ASCII.GetString(GetEntryContent(entry))
                .Split("\n");

            const string dirMarker = "*";
            var paths = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                string subPath;

                // is directory
                if (files[i].StartsWith(dirMarker))
                {
                    subPath = files[i].Substring(1) + "/";
                }
                // is file
                else
                {
                    subPath = files[i];
                }

                if (absolute)
                {
                    if (path == rootPath)
                    {
                        subPath = rootPath + subPath;
                    }
                    else
                    {
                        subPath = $"{path}/{subPath}";
                    }
                }

                paths.Add(subPath);
            }

            return paths;
        }

        private byte[] GetEntryContent(Entry entry)
        {
            reader.BaseStream.Position = (int)entry.Offset;
            byte[] file;
            if (entry.IsCompressed)
            {
                file = reader.ReadBytes((int)entry.CompressedSize);
                file = Ionic.Zlib.ZlibStream.UncompressBuffer(file);
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
            path = path.Substring(1);
            if (Salt != 0)
            {
                path = Salt + path;
            }
            var hash = CityHash.CityHash64(Encoding.ASCII.GetBytes(path), (ulong)path.Length);
            return hash;
        }

        private void ParseHeader()
        {
            uint magic = reader.ReadUInt32();
            if (magic != Magic)
                throw new Exception("Probably not a HashFS file.");

            ushort version = reader.ReadUInt16();
            if (version != SupportedVersion)
                throw new Exception($"Version {version} is not supported.");

            Salt = reader.ReadUInt16();

            HashMethod = new string(reader.ReadChars(4));
            if (HashMethod != SupportedHashMethod)
                throw new Exception($"Hash method \"{HashMethod}\" is not supported.");

            EntriesCount = reader.ReadUInt32();
            StartOffset = reader.ReadUInt32();
        }

        private void CacheEntryHeaders()
        {
            reader.BaseStream.Position = StartOffset;

            for (int i = 0; i < EntriesCount + 1; i++)
            {
                var entry = new Entry();
                entry.Hash = reader.ReadUInt64();
                entry.Offset = reader.ReadUInt64();
                entry.Flags = new BitArray(reader.ReadBytes(4));
                entry.Crc = reader.ReadUInt32();
                entry.Size = reader.ReadUInt32();
                entry.CompressedSize = reader.ReadUInt32();
                entries.Add(entry.Hash, entry);
            }
        }

        private string RemoveTrailingSlash(string path)
        {
            if (path.EndsWith("/") && path != rootPath)
                path = path.Substring(0, path.Length - 1);
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
