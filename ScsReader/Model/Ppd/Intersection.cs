using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.Model.Ppd
{
    public class Intersection : IBinarySerializable
    {
        // TODO: Figure out what this obj does

        public uint CurveId { get; set; }

        public float Position { get; set; }

        public float Radius { get; set; }

        public BitArray Flags  = new BitArray(32);

        public void ReadFromStream(BinaryReader r)
        {
            CurveId = r.ReadUInt32();
            Position = r.ReadSingle();
            Radius = r.ReadSingle();
            Flags = new BitArray(r.ReadBytes(4));
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(CurveId);
            w.Write(Position);
            w.Write(Radius);
            w.Write(Flags.ToUInt());
        }
    }
}
