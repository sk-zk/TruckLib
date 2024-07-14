using System;
using System.IO;

namespace TruckLib.HashFs
{
    /// <summary>
    /// Static factory class for creating the appropriate HashFsV*Reader depending on
    /// the HashFS version.
    /// </summary>
    public static class HashFsReader
    {
        private const uint Magic = 0x23534353; // "SCS#"

        /// <summary>
        /// Opens a HashFS archive.
        /// </summary>
        /// <param name="path">The path to the HashFS archive.</param>
        /// <param name="forceEntryTableAtEnd">If true, the entry table will be read
        /// from the end of the file, regardless of where the archive header says they are located.
        /// Only supported for v1.</param>
        /// <returns>A IHashFsReader.</returns>
        public static IHashFsReader Open(string path, bool forceEntryTableAtEnd = false)
        {
            var reader = new BinaryReader(new FileStream(path, FileMode.Open));

            uint magic = reader.ReadUInt32();
            if ((magic & 0xFFFF) == 0x4B50) // "PK"
                throw new InvalidDataException("This is a zip file.");
            if (magic != Magic)
                throw new InvalidDataException("Probably not a HashFS file.");

            ushort version = reader.ReadUInt16();
            switch (version)
            {
                case 1:
                    var h1 = new HashFsV1Reader
                    {
                        Path = path,
                        Reader = reader
                    };
                    h1.ParseHeader();
                    h1.ParseEntryTable(forceEntryTableAtEnd);
                    return h1;
                case 2:
                    var h2 = new HashFsV2Reader
                    {
                        Path = path,
                        Reader = reader
                    };
                    h2.ParseHeader();
                    h2.ParseTables();
                    return h2;
                default:
                    throw new NotSupportedException($"HashFS version {version} is not supported");
            }
        }
    }
}
