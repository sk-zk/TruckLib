using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the terrain configuration of one side of a road or prefab.
    /// </summary>
    public class TerrainQuadData : IBinarySerializable
    {
        public static readonly Token QuadErase = 0xB823;

        /// <summary>
        /// Brush materials used on terrain quads.
        /// </summary>
        public List<Material> BrushMaterials { get; set; }

        /// <summary>
        /// Brush colors used on terrain quads.
        /// </summary>
        public List<Color> BrushColors { get; set; }

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
        /// in forward direction.
        /// </summary>
        public List<TerrainQuad> Quads { get; set; } 

        // TODO: Change implementation of this data to be easier to work with
        /// <summary>
        /// Offsets of terrain vertices. Positions are relative to the position of 
        /// the backward node.
        /// </summary>
        public List<VertexData> Offsets { get; set; }

        /// <summary>
        /// TODO: What is this?
        /// </summary>
        public List<VertexData> Normals { get; set; }

        public TerrainQuadData()
        {
            Init();
        }

        internal TerrainQuadData(bool initFields)
        {
            if (initFields) Init();
        }

        protected void Init()
        {
            BrushMaterials = [new("0")];
            BrushColors = [Color.FromArgb(0, 255, 255, 255)];
            Quads = [];
            Offsets = [];
            Normals = [];
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            // the material brushes used on this terrain.
            // first one is the main mat.
            var brushMatCount = r.ReadUInt16();
            BrushMaterials = r.ReadObjectList<Material>(brushMatCount);

            // colors used on this terrain
            var colorsCount = r.ReadUInt16();
            BrushColors = new List<Color>(colorsCount);
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
            Quads = r.ReadObjectList<TerrainQuad>(terrainQuadCount);

            // offset from vertex tool
            var offsetCount = r.ReadUInt32();
            Offsets = new List<VertexData>((int)offsetCount);
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
            Normals = new List<VertexData>((int)normalCount);
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

        public void Serialize(BinaryWriter w)
        {
            // the material brushes used on this terrain.
            w.Write((ushort)BrushMaterials.Count);
            w.WriteObjectList(BrushMaterials);

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
            w.Write((uint)(Rows * Cols));
            foreach (var quad in Quads)
            {
                quad.Serialize(w);
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
