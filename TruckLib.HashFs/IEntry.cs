namespace TruckLib.HashFs
{
    public interface IEntry
    {
        /// <summary>
        /// Hash of the full path of the file.
        /// </summary>
        ulong Hash { get; }

        /// <summary>
        /// Start of the file contents in the archive.
        /// </summary>
        ulong Offset { get; }

        /// <summary>
        /// Size of the file when uncompressed.
        /// </summary>
        uint Size { get; }

        /// <summary>
        /// Size of the file in the archive.
        /// </summary>
        uint CompressedSize { get; }

        /// <summary>
        /// If true, the entry is a directory listing.
        /// </summary>
        bool IsDirectory { get; }

        /// <summary>
        /// Whether the file is compressed.
        /// </summary>
        bool IsCompressed { get; }
    }
}