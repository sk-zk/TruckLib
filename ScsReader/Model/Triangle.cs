using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScsReader.Model
{
    public struct Triangle : IBinarySerializable
    {
        public ushort A { get; set; }
        public ushort B { get; set; }
        public ushort C { get; set; }

        public Triangle(ushort a, ushort b, ushort c)
        {
            A = a;
            B = b;
            C = c;
        }

        public override string ToString()
        {
            return $"({A} {B} {C})";
        }

        public void ReadFromStream(BinaryReader r)
        {
            A = r.ReadUInt16();
            B = r.ReadUInt16();
            C = r.ReadUInt16();
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(A);
            w.Write(B);
            w.Write(C);
        }


    }
}
