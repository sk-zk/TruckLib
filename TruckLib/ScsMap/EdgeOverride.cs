using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    public struct EdgeOverride : IBinarySerializable
    {
        public Token Edge { get; set; }
        public ushort Offset { get; set; }
        public ushort Length { get; set; }

        public EdgeOverride(Token edge, ushort offset, ushort length)
        {
            Edge = edge;
            Offset = offset;
            Length = length;
        }

        public void Deserialize(BinaryReader r)
        {
            Offset = r.ReadUInt16();
            Length = r.ReadUInt16();
            Edge = r.ReadToken();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Offset);
            w.Write(Length);
            w.Write(Edge);
        }
    }
}
