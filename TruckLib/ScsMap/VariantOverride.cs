using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents a variant override of a <see cref="Road"/>.
    /// </summary>
    public struct VariantOverride : IBinarySerializable
    {
        /// <summary>
        /// Unit name of the road variant.
        /// </summary>
        public Token Variant { get; set; }

        /// <summary>
        /// Offset of the section at which the override begins, counted in forward direction.
        /// <remarks>One section spans three road quads.</remarks>
        /// </summary>
        public ushort Offset { get; set; }

        /// <summary>
        /// Number of sections for which the override will repeat.
        /// <remarks>One section spans three road quads.</remarks>
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// Instantiates a VariantOverride.
        /// </summary>
        /// <param name="variant">Unit name of the edge model.</param>
        /// <param name="offset">Offset of the section at which the override begins,
        /// counted in forward direction.</param>
        /// <param name="length">Number of road sections for which the override will repeat.</param>
        public VariantOverride(Token variant, ushort offset, ushort length)
        {
            Variant = variant;
            Offset = offset;
            Length = length;
        }

        /// <inheritdoc/>
        public void Deserialize(BinaryReader r)
        {
            Offset = r.ReadUInt16();
            Length = r.ReadUInt16();
            Variant = r.ReadToken();
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write(Offset);
            w.Write(Length);
            w.Write(Variant);
        }
    }
}
