using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.ScsMap
{
    public struct VariantOverride
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
    }
}
