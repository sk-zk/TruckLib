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
        public bool IsDirectory { get; set; }

        /// <inheritdoc/>
        public bool IsCompressed { get; internal set; }

        /// <summary>
        /// .tobj/.dds metadata if this entry is a packed .tobj/.dds file.
        /// </summary>
        public PackedTobjDdsMetadata? TobjMetadata { get; internal set; }
    }
}
