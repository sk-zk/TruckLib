using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.Models
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

        /// <summary>
        /// Inverts the order of vertices from CW to CCW or vice versa.
        /// </summary>
        public void InvertOrder()
        {
            (C, A) = (A, C);
        }

        public override string ToString()
        {
            return $"({A} {B} {C})";
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            A = r.ReadUInt16();
            B = r.ReadUInt16();
            C = r.ReadUInt16();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(A);
            w.Write(B);
            w.Write(C);
        }

    }
}
