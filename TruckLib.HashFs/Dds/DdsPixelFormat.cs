using System.IO;

namespace TruckLib.HashFs.Dds
{
    /// <summary>
    /// Describes the surface pixel format of a DDS file.
    /// </summary>
    internal class DdsPixelFormat
    {
        private const int StructSize = 32;

        /// <summary>
        /// Values which indicate what type of data is in the surface.
        /// </summary>
        private FlagField Flags;

        /// <summary>
        /// Texture contains alpha data; RGBAlphaBitMask contains valid data.
        /// </summary>
        public bool HasAlphaPixels
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Used in some older DDS files for alpha channel only uncompressed data
        /// (RGBBitCount contains the alpha channel bitcount; ABitMask contains valid data).
        /// </summary>
        public bool IsAlpha
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Texture contains compressed RGB data; FourCC contains valid data.
        /// </summary>
        public bool HasCompressedRgbData
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Texture contains uncompressed RGB data; RGBBitCount and the RGB masks
        /// (RBitMask, GBitMask, BBitMask) contain valid data.
        /// </summary>
        public bool HasUncompressedRgbData
        {
            get => Flags[6];
            set => Flags[6] = value;
        }

        /// <summary>
        /// Used in some older DDS files for YUV uncompressed data (RGBBitCount contains
        /// the YUV bit count; RBitMask contains the Y mask, GBitMask contains the U mask,
        /// BBitMask contains the V mask).
        /// </summary>
        public bool HasUncompressedYuvData
        {
            get => Flags[9];
            set => Flags[9] = value;
        }

        /// <summary>
        /// Used in some older DDS files for single channel color uncompressed data (RGBBitCount
        /// contains the luminance channel bit count; RBitMask contains the channel mask).
        /// Can be combined with HasAlphaPixels for a two-channel DDS file.
        /// </summary>
        public bool HasLuminanceData
        {
            get => Flags[17];
            set => Flags[17] = value;
        }

        /// <summary>
        /// Four-character codes for specifying compressed or custom formats.
        /// Possible values include: DXT1, DXT2, DXT3, DXT4, or DXT5. A FourCC of DX10 indicates
        /// the prescense of the DdsHeaderDxt10 extended header, and the DxgiFormat member of
        /// that structure indicates the true format. When using a four-character code,
        /// Flags must include DDPF_FOURCC.
        /// </summary>
        public uint FourCC { get; set; }

        public const uint FourCC_DXT1 = 0x31545844;
        public const uint FourCC_DX10 = 0x30315844;

        /// <summary>
        /// Number of bits in an RGB (possibly including alpha) format.
        /// Valid when Flags includes DDPF_RGB, DDPF_LUMINANCE, or DDPF_YUV.
        /// </summary>
        public uint RgbBitCount { get; set; }

        /// <summary>
        /// Red (or luminance or Y) mask for reading color data.
        /// For instance, given the A8R8G8B8 format, the red mask would be 0x00ff0000.
        /// </summary>
        public uint RBitMask { get; set; }

        /// <summary>
        /// Green (or U) mask for reading color data.
        /// For instance, given the A8R8G8B8 format, the green mask would be 0x0000ff00.
        /// </summary>
        public uint GBitMask { get; set; }

        /// <summary>
        /// Blue (or V) mask for reading color data.
        /// For instance, given the A8R8G8B8 format, the blue mask would be 0x000000ff.
        /// </summary>
        public uint BBitMask { get; set; }

        /// <summary>
        /// Alpha mask for reading alpha data. Flags must include DDPF_ALPHAPIXELS or DDPF_ALPHA.
        /// For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
        /// </summary>
        public uint ABitMask { get; set; }

        public void Deserialize(BinaryReader r)
        {
            var size = r.ReadUInt32();
            if (size != StructSize)
            {
                throw new InvalidDataException();
            }

            Flags = new FlagField(r.ReadUInt32());
            FourCC = r.ReadUInt32();
            RgbBitCount = r.ReadUInt32();
            RBitMask = r.ReadUInt32();
            GBitMask = r.ReadUInt32();
            BBitMask = r.ReadUInt32();
            ABitMask = r.ReadUInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(StructSize);
            w.Write(Flags.Bits);
            w.Write(FourCC);
            w.Write(RgbBitCount);
            w.Write(RBitMask);
            w.Write(GBitMask);
            w.Write(BBitMask);
            w.Write(ABitMask);
        }
    }
}
