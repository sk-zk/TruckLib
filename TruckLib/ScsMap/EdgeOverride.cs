using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents an edge model override of a <see cref="Road"/>.
    /// </summary>
    public struct EdgeOverride : IBinarySerializable
    {
        /// <summary>
        /// Unit name of the edge model, as defined in <c>/def/world/road_edge.sii</c>.
        /// </summary>
        public Token Edge { get; set; }

        /// <summary>
        /// Look of the edge model.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// Offset of the section at which the override begins, counted in forward direction.
        /// </summary>
        /// <remarks>One section spans three road quads.</remarks>
        public ushort Offset { get; set; }

        /// <summary>
        /// Number of sections for which the override will repeat.
        /// </summary>
        /// <remarks>One section spans three road quads.</remarks>
        public ushort Length { get; set; }

        /// <summary>
        /// Instantiates an EdgeOverride.
        /// </summary>
        /// <param name="edge">Unit name of the edge model.</param>
        /// <param name="look">Look of the edge model.</param>
        /// <param name="offset">Offset of the section at which the override begins,
        /// counted in forward direction.</param>
        /// <param name="length">Number of road sections for which the override will repeat.</param>
        public EdgeOverride(Token edge, Token look, ushort offset, ushort length)
        {
            Edge = edge;
            Look = look;
            Offset = offset;
            Length = length;
        }

        public EdgeOverride(Token edge, ushort offset, ushort length) 
            : this(edge, "", offset, length)
        {         
        }

        /// <inheritdoc/>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Offset = r.ReadUInt16();
            Length = r.ReadUInt16();
            Edge = r.ReadToken();
            Look = r.ReadToken();
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write(Offset);
            w.Write(Length);
            w.Write(Edge);
            w.Write(Look);
        }
    }
}
