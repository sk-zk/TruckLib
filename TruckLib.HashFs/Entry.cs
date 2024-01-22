using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.HashFs
{
    /// <summary>
    /// Represents an entry header which contains metadata about a file in a HashFS archive.
    /// </summary>
    public struct Entry
    {
        /// <summary>
        /// Hash of the full path of the file.
        /// </summary>
        public ulong Hash { get; internal set; }

        /// <summary>
        /// Start of the file contents in the archive.
        /// </summary>
        public ulong Offset { get; internal set; }

        internal FlagField Flags { get; set; }

        /// <summary>
        /// CRC32 checksum of the file.
        /// </summary>
        public uint Crc { get; internal set; }

        /// <summary>
        /// Size of the file when uncompressed.
        /// </summary>
        public uint Size { get; internal set; }

        /// <summary>
        /// Size of the file in the archive.
        /// </summary>
        public uint CompressedSize { get; internal set; }

        /// <summary>
        /// If true, the entry is a directory listing.
        /// </summary>
        public bool IsDirectory => Flags[0];

        /// <summary>
        /// Whether the file is compressed.
        /// </summary>
        public bool IsCompressed => Flags[1];

        public bool Verify => Flags[2]; // TODO: What is this?

        public bool IsEncrypted => Flags[3];
    }
}
