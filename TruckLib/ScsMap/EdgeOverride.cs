using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap
{
    public struct EdgeOverride
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
    }
}
