using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap.Serialization;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents metadata of one sector of a <see cref="Map"/>.
    /// </summary>
    public class Sector
    {
        /// <summary>
        /// The coordinate of this sector. 
        /// </summary>
        public SectorCoordinate Coordinate { get; set; }

        /// <summary>
        /// The map the sector belongs to.
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// The path of the .base file.
        /// </summary>
        internal string BasePath { get; set; }

        /// <summary>
        /// Unit name of the climate profile of this sector.
        /// </summary>
        public Token Climate { get; set; } = "default";

        // Always 2 in both ETS2 and ATS.
        private uint SectorDescVersion = 2;

        public Vector2 MinBoundary { get; set; } = new Vector2(0, 0);

        public Vector2 MaxBoundary { get; set; } = new Vector2(4000, 4000);

        internal FlagField Flags = new();

        internal const string BaseExtension = "base";
        internal const string DataExtension = "data";
        internal const string SndExtension = "snd";
        internal const string AuxExtenstion = "aux";
        internal const string DescExtension = "desc";
        internal const string LayerExtension = "layer";

        public Sector() { }

        /// <summary>Instantiates a sector with default metadata.</summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <param name="map">The map this sector belongs to.</param>
        public Sector(int x, int z, Map map) 
            : this(new SectorCoordinate(x, z), map) { }

        /// <summary>Instantiates a sector with default metadata.</summary>
        /// <param name="coord">The coordinate of the sector.</param>
        /// <param name="map">The map this sector belongs to.</param>
        public Sector(SectorCoordinate coord, Map map)
        {
            Coordinate = coord;
            Map = map;
        }

        private const float boundaryFactor = 256f;

        /// <summary>
        /// Reads the .desc file of the sector.
        /// </summary>
        /// <param name="path">The .desc file of the sector.</param>
        internal void ReadDesc(string path)
        {
            // TODO: 
            // - figure out if there are any desc flags (ets2 & ats 
            //   base maps don't have any)
            // - figure out what exactly the boundaries are and how
            //   they work, because they seem to relate to items
            //   at the borders of the sector

            using var r = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));

            SectorDescVersion = r.ReadUInt32();

            MinBoundary = new Vector2(
                r.ReadInt32() / boundaryFactor,
                r.ReadInt32() / boundaryFactor
                );
            MaxBoundary = new Vector2(
                r.ReadInt32() / boundaryFactor,
                r.ReadInt32() / boundaryFactor
                );

            Flags = new FlagField(r.ReadUInt32());

            Climate = r.ReadToken();
        }

        /// <summary>
        /// Writes the .desc part of the sector.
        /// </summary>
        /// <param name="path">The path of the output file.</param>
        internal void WriteDesc(string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            using var w = new BinaryWriter(stream);

            w.Write(SectorDescVersion);
            w.Write((int)(MinBoundary.X * boundaryFactor));
            w.Write((int)(MinBoundary.Y * boundaryFactor));
            w.Write((int)(MaxBoundary.X * boundaryFactor));
            w.Write((int)(MaxBoundary.Y * boundaryFactor));
            w.Write(Flags.Bits);
            w.Write(Climate);
        }

        /// <summary>
        /// Parses sector coordinates from the path to a sector file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The coordinates of the sector.</returns>
        public static SectorCoordinate SectorCoordsFromSectorFilePath(string path)
        {
            var sectorName = Path.GetFileNameWithoutExtension(path);
            var x = int.Parse(sectorName.Substring(3, 5));
            var z = int.Parse(sectorName.Substring(8, 5));
            return new SectorCoordinate(x, z);
        }

        public static string SectorFileNameFromSectorCoords(SectorCoordinate coord) => 
            $"sec{coord.X:+0000;-0000;+0000}{coord.Z:+0000;-0000;+0000}";

        /// <summary>
        /// Returns the name of this sector as used in filenames and the editor's map overlay.
        /// </summary>
        /// <returns>The name of this sector.</returns>
        public override string ToString() =>
            SectorFileNameFromSectorCoords(Coordinate);

    }
}
