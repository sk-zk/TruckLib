namespace TruckLib.HashFs
{
    public interface IEntry
    {
        ulong Hash { get; }
        ulong Offset { get; }
        uint Size { get; }
        uint CompressedSize { get; }
        bool IsDirectory { get; }
        bool IsCompressed { get; }
    }
}