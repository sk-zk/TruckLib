using System.IO;

namespace TruckLib.HashFs.Dds
{
    /// <summary>
    /// Reperesents the header of a DDS file.
    /// </summary>
    internal class DdsHeader
    {
        private const int StructSize = 124;

        /// <summary>
        /// Flags to indicate which members contain valid data.
        /// </summary>
        private FlagField Flags;

        /// <summary>
        /// Indicates whether Caps contains valid data. Required in every .dds file.
        /// </summary>
        public bool IsCapsValid
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Indicates whether Height contains valid data. Required in every .dds file.
        /// </summary>
        public bool IsHeightValid
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Indicates whether Width contains valid data. Required in every .dds file.
        /// </summary>
        public bool IsWidthValid
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Indicates whether PitchOrLinearSize contains valid data.
        /// Required when pitch is provided for an uncompressed texture.
        /// </summary>
        public bool IsPitchValid
        {
            get => Flags[3];
            set => Flags[3] = value;
        }

        /// <summary>
        /// Indicates whether PixelFormat contains valid data. Required in every .dds file.
        /// </summary>
        public bool IsPixelFormatValid
        {
            get => Flags[12];
            set => Flags[12] = value;
        }

        /// <summary>
        /// Indicates whether MipMapCount contains valid data. Required in a mipmapped texture.
        /// </summary>
        public bool IsMipMapCountValid
        {
            get => Flags[17];
            set => Flags[17] = value;
        }

        /// <summary>
        /// Indicates whether PitchOrLinearSize contains valid data.
        /// Required when pitch is provided for a compressed texture.
        /// </summary>
        public bool IsLinearSizeValid
        {
            get => Flags[19];
            set => Flags[19] = value;
        }

        /// <summary>
        /// Indicates whether Depth contains valid data.
        /// Required in a depth texture.
        /// </summary>
        public bool IsDepthValid
        {
            get => Flags[23];
            set => Flags[23] = value;
        }

        /// <summary>
        /// Surface height (in pixels).
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Surface width (in pixels).
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// The pitch or number of bytes per scan line in an uncompressed texture;
        /// the total number of bytes in the top level texture for a compressed texture.
        /// For information about how to compute the pitch, see the DDS File Layout section
        /// of the <see 
        ///     href="https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide"
        /// >Programming Guide for DDS</see>.
        /// </summary>
        public uint PitchOrLinearSize { get; set; }

        /// <summary>
        /// Depth of a volume texture (in pixels), otherwise unused.
        /// </summary>
        public uint Depth { get; set; }

        /// <summary>
        /// Number of mipmap levels, otherwise unused.
        /// </summary>
        public uint MipMapCount { get; set; }

        /// <summary>
        /// The pixel format.
        /// </summary>
        public DdsPixelFormat PixelFormat { get; set; }

        /// <summary>
        /// Specifies the complexity of the surfaces stored.
        /// </summary>
        private FlagField Caps;

        /// <summary>
        /// Required.
        /// </summary>
        public bool CapsTexture
        {
            get => Caps[12];
            set => Caps[12] = value;
        }

        /// <summary>
        /// Optional; should be used for a mipmap.
        /// </summary>
        public bool CapsMipMap
        {
            get => Caps[22];
            set => Caps[22] = value;
        }

        /// <summary>
        /// Optional; must be used on any file that contains more than one surface
        /// (a mipmap, a cubic environment map, or mipmapped volume texture).
        /// </summary>
        public bool CapsComplex
        {
            get => Caps[3];
            set => Caps[3] = value;
        }

        /// <summary>
        /// Additional detail about the surfaces stored.
        /// </summary>
        private FlagField Caps2 { get; set; }

        /// <summary>
        /// Required for a cubemap.
        /// </summary>
        public bool Caps2Cubemap
        {
            get => Caps[9];
            set => Caps[9] = value;
        }

        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        public bool Caps2CubemapPositiveX
        {
            get => Caps[10];
            set => Caps[10] = value;
        }

        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        public bool Caps2CubemapNegativeX
        {
            get => Caps[11];
            set => Caps[11] = value;
        }

        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        public bool Caps2CubemapPositiveY
        {
            get => Caps[12];
            set => Caps[12] = value;
        }

        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        public bool Caps2CubemapNegativeY
        {
            get => Caps[13];
            set => Caps[13] = value;
        }

        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        public bool Caps2CubemapPositiveZ
        {
            get => Caps[14];
            set => Caps[14] = value;
        }

        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        public bool Caps2CubemapNegativeZ
        {
            get => Caps[15];
            set => Caps[15] = value;
        }

        /// <summary>
        /// Required for a volume texture.
        /// </summary>
        public bool Caps2Volume
        {
            get => Caps[21];
            set => Caps[21] = value;
        }


        public void Deserialize(BinaryReader r)
        {
            var size = r.ReadUInt32();
            if (size != StructSize)
            {
                throw new InvalidDataException();
            }

            Flags = new FlagField(r.ReadUInt32());
            Height = r.ReadUInt32();
            Width = r.ReadUInt32();
            PitchOrLinearSize = r.ReadUInt32();
            Depth = r.ReadUInt32();
            MipMapCount = r.ReadUInt32();

            // Reserved1; unused
            for (int i = 0; i < 11; i++)
            {
                r.ReadUInt32();
            }

            PixelFormat = new DdsPixelFormat();
            PixelFormat.Deserialize(r);

            Caps = new FlagField(r.ReadUInt32());
            Caps2 = new FlagField(r.ReadUInt32());
            r.ReadUInt32(); // Caps3; unused
            r.ReadUInt32(); // Caps4; unused
            r.ReadUInt32(); // Reserved2; unused
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(StructSize);
            w.Write(Flags.Bits);
            w.Write(Height);
            w.Write(Width);
            w.Write(PitchOrLinearSize);
            w.Write(Depth);
            w.Write(MipMapCount);

            // Reserved1; unused
            for (int i = 0; i < 11; i++)
            {
                w.Write(0);
            }

            PixelFormat.Serialize(w);

            w.Write(Caps.Bits);
            w.Write(Caps2.Bits);
            w.Write(0); // Caps3; unused
            w.Write(0); // Caps4; unused
            w.Write(0); // Reserved2; unused
        }
    }
}
