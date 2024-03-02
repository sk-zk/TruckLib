namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents a map sector coordinate.
    /// </summary>
    /// <param name="X">The X coordinate.</param>
    /// <param name="Z">The Z coordinate.</param>
    public record struct SectorCoordinate(int X, int Z);
}
