using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Represents the terrain configuration of one side of a road or prefab.
    /// </summary>
    public class TerrainQuadData : IBinarySerializable
    {
        public static readonly Token QuadErase = new Token(0xB823);

        /// <summary>
        /// The main terrain material.
        /// </summary>
        public Token Material
        {
            get => BrushMaterials[0];
            set => BrushMaterials[0] = value;
        }

        /// <summary>
        /// Brush materials used on terrain quads.
        /// </summary>
        public List<Token> BrushMaterials { get; set; } 
            = new List<Token> { "0" };

        /// <summary>
        /// Brush colors used on terrain quads.
        /// </summary>
        public List<Color> BrushColors { get; set; } 
            = new List<Color>() { Color.FromArgb(0, 255, 255, 255) };

        /// <summary>
        /// Amount of terrain quad rows.
        /// </summary>
        public ushort Rows { get; set; }

        /// <summary>
        /// Amount of terrain quad columns.
        /// </summary>
        public ushort Cols { get; set; }

        /// <summary>
        /// Terrain quads in this terrain. Indexing begins in the bottom left corner
        /// in driving direction.
        /// </summary>
        public List<TerrainQuad> Quads { get; set; } = new List<TerrainQuad>();

        // TODO: Change implementation of this data to be easier to work with
        /// <summary>
        /// Offsets in terrain vertices created with the vertex tool.
        /// </summary>
        public List<VertexData> Offsets { get; set; } = new List<VertexData>();

        /// <summary>
        /// TODO: What is this?
        /// </summary>
        public List<VertexData> Normals { get; set; } = new List<VertexData>();

        public TerrainQuadData Clone()
        {
            var q = new TerrainQuadData();
            q.Rows = Rows;
            q.Cols = Cols;
            q.BrushMaterials = new List<Token>(BrushMaterials);
            q.BrushColors = new List<Color>(BrushColors);
            q.Quads = new List<TerrainQuad>(Quads.Select(x => x.Clone()));
            q.Offsets = new List<VertexData>(Offsets);
            q.Normals = new List<VertexData>(Normals);
            return q;
        }

        public void ReadFromStream(BinaryReader r)
        {
            // the uids of the material brushes used on this terrain.
            // first one is the main mat.
            // 0xB823 is Quad Erase.
            var brushMatCount = r.ReadUInt16();
            BrushMaterials.Clear();
            for (int i = 0; i < brushMatCount; i++)
            {
                var brush = r.ReadToken();
                BrushMaterials.Add(brush);
            }

            // colors used on this terrain
            var colorsCount = r.ReadUInt16();
            BrushColors.Clear();
            for (int i = 0; i < colorsCount; i++)
            {
                var red = r.ReadByte();
                var green = r.ReadByte();
                var blue = r.ReadByte();
                var alpha = r.ReadByte();
                BrushColors.Add(Color.FromArgb(alpha, red, green, blue));
            }

            // amt of rows & cols of the terrain quad grid
            Rows = r.ReadUInt16();
            Cols = r.ReadUInt16();

            // terrain quad data; 4 bytes per quad.
            // indexing begins at the bottom left quad in forward direction.
            var terrainQuadCount = r.ReadUInt32();
            for (int i = 0; i < terrainQuadCount; i++)
            {
                var quad = new TerrainQuad();
                quad.ReadFromStream(r);           
                Quads.Add(quad);
            }

            // offset from vertex tool
            var offsetCount = r.ReadUInt32();
            for (int i = 0; i < offsetCount; i++)
            {
                var offset = new VertexData()
                {
                    X = r.ReadUInt16(),
                    Y = r.ReadUInt16(),
                    Data = r.ReadVector3(),
                };
                Offsets.Add(offset);
            }

            // normals
            // what is this??
            var normalCount = r.ReadUInt32();
            for (int i = 0; i < normalCount; i++)
            {
                var normal = new VertexData
                {
                    X = r.ReadUInt16(),
                    Y = r.ReadUInt16(),
                    Data = r.ReadVector3(),
                };
                Normals.Add(normal);
            }
        }

        public void WriteToStream(BinaryWriter w)
        {           
            // amount of materials used on this terrain
            w.Write((ushort)BrushMaterials.Count);

            // the uids of the material brushes used on this terrain.
            // first one is the main mat.
            // Quad Erase is 0xB823.
            // also, erased quads are always No Veg. as well.
            foreach(var brush in BrushMaterials)
            {
                w.Write(brush);
            }

            // colors used on this terrain
            w.Write((ushort)BrushColors.Count);
            foreach (var color in BrushColors)
            {
                w.Write(color.R);
                w.Write(color.G);
                w.Write(color.B);
                w.Write(color.A);
            }

            // amt of rows & cols of the terrain quad grid
            w.Write(Rows);
            w.Write(Cols);

            // terrain quad data; 4 bytes per quad.
            w.Write(Rows * Cols);
            foreach (var quad in Quads)
            {
                quad.WriteToStream(w);
            }

            // offset
            w.Write(Offsets.Count);
            foreach(var offset in Offsets)
            {
                w.Write(offset.X);
                w.Write(offset.Y);
                w.Write(offset.Data);
            }

            // normals
            w.Write(Normals.Count);
            foreach (var normal in Normals)
            {
                w.Write(normal.X);
                w.Write(normal.Y);
                w.Write(normal.Data);
            }
        }
    }

    public struct VertexData
    {
        public ushort X;
        public ushort Y;
        public Vector3 Data;
    }

}
