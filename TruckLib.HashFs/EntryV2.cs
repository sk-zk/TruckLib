using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.HashFs
{
    /// <summary>
    /// Represents the metadata of an entry in a HashFS v2 archive.
    /// </summary>
    public struct EntryV2 : IEntry
    {
        /// <inheritdoc/>
        public ulong Hash { get; internal set; }

        /// <inheritdoc/>
        public ulong Offset { get; internal set; }

        /// <inheritdoc/>
        public uint Size { get; internal set; }

        /// <inheritdoc/>
        public uint CompressedSize { get; internal set; }

        /// <inheritdoc/>
        public bool IsDirectory { get; internal set; }

        /// <inheritdoc/>
        public bool IsCompressed { get; internal set; }

        /// <summary>
        /// If true, the file is a packed .tobj/.dds hybrid.
        /// (TruckLib is not yet able to unpack entries of this type.)
        /// </summary>
        public bool IsTobj { get; internal set; }
    }
}
