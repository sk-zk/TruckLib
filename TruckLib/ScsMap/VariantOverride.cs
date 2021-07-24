using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    public struct VariantOverride : IBinarySerializable
    {
        public Token Variant { get; set; }
        public ushort Offset { get; set; }
        public ushort Length { get; set; }

        public VariantOverride(Token variant, ushort offset, ushort length)
        {
            Variant = variant;
            Offset = offset;
            Length = length;
        }

        public void Deserialize(BinaryReader r)
        {
            Offset = r.ReadUInt16();
            Length = r.ReadUInt16();
            Variant = r.ReadToken();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Offset);
            w.Write(Length);
            w.Write(Variant);
        }
    }
}
