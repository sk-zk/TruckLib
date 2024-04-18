namespace TruckLib.HashFs
{
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