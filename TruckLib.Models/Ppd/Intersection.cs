using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd
{
    public class Intersection : IBinarySerializable
    {
        // TODO: Figure out what this obj does

        public uint CurveId { get; set; }

        public float Position { get; set; }

        public float Radius { get; set; }

        public FlagField Flags { get; set; }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            CurveId = r.ReadUInt32();
            Position = r.ReadSingle();
            Radius = r.ReadSingle();
            Flags = new FlagField(r.ReadUInt32());
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(CurveId);
            w.Write(Position);
            w.Write(Radius);
            w.Write(Flags.Bits);
        }
    }
}
