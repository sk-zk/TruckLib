using System;
using System.Collections.Generic;

namespace TruckLib.HashFs
{
    public interface IHashFsReader : IDisposable
    {
        /// <summary>
        /// Gets the file path of the HashFS archive which this reader is reading from.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The entries in this archive.
        /// </summary>
        Dictionary<ulong, IEntry> Entries { get; }

        /// <summary>
        /// Gets or sets the salt which is prepended to paths before hashing them.
        /// </summary>
        ushort Salt { get; set; }

        /// <summary>
        /// Checks if an entry exists and returns its type if it does.
        /// </summary>
        /// <param name="path">The path of the entry in the archive.</param>
        /// <returns>Its type.</returns>
        EntryType EntryExists(string path);

        /// <summary>
        /// Extracts and decompresses an entry to memory.
        /// </summary>
        /// <param name="path">The path of the entry in the archive.</param>
        /// <returns>The extracted entry as byte array.</returns>
        byte[] Extract(string path);

        /// <summary>
        /// Extracts and decompresses an entry to memory.
        /// </summary>
        /// <param name="entry">The entry metadata of the file to extract.</param>
        /// <returns>The extracted entry as byte array.</returns>
        byte[] Extract(IEntry entry);

        /// <summary>
        /// Extracts and decompresses an entry to a file.
        /// </summary>
        /// <param name="path">The path of the entry in the archive.</param>
        /// <param name="outputPath">The output path.</param>
        void ExtractToFile(string path, string outputPath);

        /// <summary>
        /// Extracts and decompresses an entry to a file.
        /// </summary>
        /// <param name="entry">The entry metadata of the file to extract.</param>
        /// <param name="outputPath">The output path.</param>
        void ExtractToFile(IEntry entry, string outputPath);

        /// <summary>
        /// Returns a list of subdirectories and files in the given directory.
        /// </summary>
        /// <param name="path">The path of the directory in the archive.</param>
        /// <param name="filesOnly">Whether only files should be returned.</param>
        /// <param name="returnAbsolute">Whether the returned paths should be made absolute.</param>
        /// <returns>A list of subdirectories and files in the given directory.</returns>
        (List<string> Subdirs, List<string> Files) GetDirectoryListing(string path, bool filesOnly = false, bool returnAbsolute = true);

        /// <summary>
        /// Returns a list of subdirectories and files in the given directory.
        /// </summary>
        /// <param name="entry">The entry metadata of the directory.</param>
        /// <param name="filesOnly">Whether only files should be returned.</param>
        /// <returns>A list of subdirectories and files in the given directory.</returns>
        (List<string> Subdirs, List<string> Files) GetDirectoryListing(IEntry entry, bool filesOnly = false);

        /// <summary>
        /// Retrieves the entry metadata for the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The entry metadata.</returns>
        IEntry GetEntry(string path);

        /// <summary>
        /// Hashes a file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="salt">If set, this salt will be used to hash the path rather than
        /// the one specified by the archive's header.</param>
        /// <returns>The hash of the path.</returns>
        ulong HashPath(string path, uint? salt = null);
    }
}