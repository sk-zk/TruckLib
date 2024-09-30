using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.HashFs
{
    internal abstract class HashFsReaderBase : IHashFsReader
    {
        /// <inheritdoc/>
        public string Path { get; internal set; }

        /// <inheritdoc/>
        public Dictionary<ulong, IEntry> Entries { get; } = [];

        /// <inheritdoc/>
        public ushort Salt { get; set; }

        /// <inheritdoc/>
        public abstract ushort Version { get; }

        internal BinaryReader Reader { get; set; }

        protected const string RootPath = "/";
        protected const string SupportedHashMethod = "CITY";

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public byte[][] Extract(string path)
        {
            if (EntryExists(path) == EntryType.NotFound)
                throw new FileNotFoundException();

            var entry = GetEntry(path);
            return Extract(entry, path);
        }

        /// <inheritdoc/>
        public virtual byte[][] Extract(IEntry entry, string path)
        {
            if (!Entries.ContainsValue(entry))
                throw new FileNotFoundException();

            return [GetEntryContent(entry)];
        }

        /// <inheritdoc/>
        public void ExtractToFile(string entryPath, string outputPath)
        {
            if (EntryExists(entryPath) == EntryType.NotFound)
                throw new FileNotFoundException();

            var entry = GetEntry(entryPath);
            ExtractToFile(entry, entryPath, outputPath);
        }

        /// <inheritdoc/>
        public virtual void ExtractToFile(IEntry entry, string entryPath, string outputPath)
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

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outputPath));
            Reader.BaseStream.Position = (long)entry.Offset;
            using var fileStream = new FileStream(outputPath, FileMode.Create);
            if (entry.IsCompressed)
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

        /// <inheritdoc/>
        public DirectoryListing GetDirectoryListing(
            string path, bool filesOnly = false, bool returnAbsolute = true)
        {
            path = RemoveTrailingSlash(path);

            var entryType = EntryExists(path);
            if (entryType == EntryType.NotFound)
                throw new FileNotFoundException();
            else if (entryType != EntryType.Directory)
                throw new ArgumentException($"\"{path}\" is not a directory.");

            var entry = GetEntry(path);

            var dir = GetDirectoryListing(entry, filesOnly);

            if (returnAbsolute)
            {
                MakePathsAbsolute(path, dir.Subdirectories);
                MakePathsAbsolute(path, dir.Files);
            }

            return new DirectoryListing(dir.Subdirectories, dir.Files);
        }

        /// <inheritdoc/>
        public abstract DirectoryListing GetDirectoryListing(
            IEntry entry, bool filesOnly = false);

        /// <inheritdoc/>
        public IEntry GetEntry(string path)
        {
            ulong hash = HashPath(path);
            var entry = Entries[hash];
            return entry;
        }

        /// <inheritdoc/>
        public ulong HashPath(string path, uint? salt = null)
        {
            if (path != "" && path.StartsWith('/'))
                path = path[1..];

            // TODO do salts work the same way in v2?
            salt ??= Salt;
            if (salt != 0)
                path = salt + path;

            var bytes = Encoding.UTF8.GetBytes(path);
            var hash = CityHash.CityHash64(bytes, (ulong)bytes.Length);
            return hash;
        }

        /// <summary>
        /// Closes the file stream.
        /// </summary>
        public void Dispose()
        {
            Reader.BaseStream.Dispose();
            Reader.Dispose();
        }

        protected void MakePathsAbsolute(string parent, List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                if (parent == RootPath)
                    paths[i] = RootPath + paths[i];
                else
                    paths[i] = $"{parent}/{paths[i]}";
            }
        }

        protected string RemoveTrailingSlash(string path)
        {
            if (path.EndsWith('/') && path != RootPath)
                path = path[0..^1];
            return path;
        }

        protected virtual byte[] GetEntryContent(IEntry entry)
        {
            Reader.BaseStream.Position = (long)entry.Offset;
            byte[] file;
            if (entry.IsCompressed)
            {
                if (entry is EntryV2 v2 && v2.TobjMetadata != null)
                {
                    file = Reader.ReadBytes((int)entry.CompressedSize);
                }
                else
                {
                    file = Reader.ReadBytes((int)entry.CompressedSize);
                    file = ZlibStream.UncompressBuffer(file);
                }
            }
            else
            {
                file = Reader.ReadBytes((int)entry.Size);
            }
            return file;
        }
    }
}
