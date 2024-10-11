using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd
{
    /// <summary>
    /// Represents a control node.
    /// </summary>
    public class ControlNode : IBinarySerializable
    { 
        // is this a ref. we can resolve instead of
        // having an index?
        public uint TerrainPointIndex { get; set; }

        // and can we replace this with a property then?
        public uint TerrainPointCount { get; set; }

        public uint VariantIdx { get; set; }

        public uint VariantCount { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Direction { get; set; }

        public int[] InputLines { get; set; } = new int[8];

        public int[] OutputLines { get; set; } = new int[8];

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            TerrainPointIndex = r.ReadUInt32();
            TerrainPointCount = r.ReadUInt32();

            VariantIdx = r.ReadUInt32();
            VariantCount = r.ReadUInt32();

            Position = r.ReadVector3();
            Direction = r.ReadVector3();

            for (int i = 0; i < InputLines.Length; i++)
            {
                InputLines[i] = r.ReadInt32();
            }

            for (int i = 0; i < OutputLines.Length; i++)
            {
                OutputLines[i] = r.ReadInt32();
            }
        }

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}