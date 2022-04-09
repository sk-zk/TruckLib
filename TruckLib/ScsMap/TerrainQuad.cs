using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A single terrain quad.
    /// </summary>
    public struct TerrainQuad : IBinarySerializable
    {
        private byte byte1;
        private byte byte2;
        private byte byte3;
        private byte byte4;

        const int n1Mask = 0x0F;
        const int n2Mask = 0xF0;
        const byte noDetVegMask = 16;

        /// <summary>
        /// Index of the main terrain material of this quad.
        /// </summary>
        public Nibble MainMaterial
        {
            get => (Nibble)(byte1 & n1Mask);
            set => byte1 |= (byte)value;
        }

        /// <summary>
        /// Additional material that will be drawn on top with the specified
        /// opacity value.
        /// </summary>
        public Nibble BlendMaterial
        {
            get => (Nibble)((byte1 & n2Mask) >> 4);
            set => byte1 |= (byte)((byte)value << 4);
        }

        /// <summary>
        /// Opacity for the blend material.
        /// </summary>
        public Nibble Opacity
        {
            get => (Nibble)(byte2 & n1Mask);
            set => byte2 |= (byte)value;
        }

        /// <summary>
        /// Texture color in the bottom left corner.
        /// </summary>
        public Nibble ColorBottomLeft
        {
            get => (Nibble)((byte2 & n2Mask) >> 4);
            set => byte2 |= (byte)((byte)value << 4);
        }

        /// <summary>
        /// Texture color in the bottom right corner.
        /// </summary>
        public Nibble ColorBottomRight
        {
            get => (Nibble)(byte3 & n1Mask);
            set => byte3 |= (byte)value;
        }

        /// <summary>
        /// Texture color in the top left corner.
        /// </summary>
        public Nibble ColorTopLeft
        {
            get => (Nibble)((byte3 & n2Mask) >> 4);
            set => byte3 |= (byte)((byte)value << 4);
        }

        /// <summary>
        /// Texture color in the top right corner.
        /// </summary>
        public Nibble ColorTopRight
        {
            get => (Nibble)(byte4 & n1Mask);
            set => byte4 |= (byte)value;
        }

        /// <summary>
        /// Vegetation setting for this quad.
        /// </summary>
        public QuadVegetation Vegetation
        {
            get => (QuadVegetation)((byte4 & n2Mask) >> 5);
            set => byte4 |= (byte)((byte)value << 5);
        }

        public bool NoDetailVegetation
        {
            get => (byte4 & noDetVegMask) == noDetVegMask;
            set
            {
                if (value)
                    byte4 |= noDetVegMask;
                else
                    byte4 &= noDetVegMask ^ 0xFF;
            }
        }

        public void Deserialize(BinaryReader r)
        {
            byte1 = r.ReadByte();
            byte2 = r.ReadByte();
            byte3 = r.ReadByte();
            byte4 = r.ReadByte();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(byte1);
            w.Write(byte2);
            w.Write(byte3);
            w.Write(byte4);
        }

        public override int GetHashCode() =>
            (byte1, byte2, byte3, byte4).GetHashCode();

        public override bool Equals(object obj) =>
            obj is TerrainQuad quad
            && quad.byte1 == this.byte1
            && quad.byte2 == this.byte2
            && quad.byte3 == this.byte3
            && quad.byte4 == this.byte4;

        public static bool operator ==(TerrainQuad left, TerrainQuad right) =>
            left.Equals(right);

        public static bool operator !=(TerrainQuad left, TerrainQuad right) =>
            !(left == right);
    }
}
