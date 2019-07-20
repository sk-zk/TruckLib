using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.Model.Ppd
{
    /// <summary>
    /// Represents a control node.
    /// </summary>
    public class ControlNode : IBinarySerializable
    { 
        // is this a ref. we can resolve instead of
        // having an index?
        public uint TerrainPointIdx;

        // and can we replace this with a property then?
        public uint TerrainPointCount;

        public uint VariantIdx;

        public uint VariantCount;

        public Vector3 Position;

        public Vector3 Direction;

        public int[] InputLines = new int[8];

        public int[] OutputLines = new int[8];

        public void ReadFromStream(BinaryReader r)
        {
            TerrainPointIdx = r.ReadUInt32();
            TerrainPointCount = r.ReadUInt32();

            VariantIdx = r.ReadUInt32();
            VariantCount = r.ReadUInt32();

            Position = r.ReadVector3();
            Direction = r.ReadVector3();

            for(int i = 0; i < InputLines.Length; i++)
            {
                InputLines[i] = r.ReadInt32();
            }

            for (int i = 0; i < OutputLines.Length; i++)
            {
                OutputLines[i] = r.ReadInt32();
            }
        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}