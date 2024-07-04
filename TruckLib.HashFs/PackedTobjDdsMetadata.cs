using System.Collections.Generic;
using System.IO;
using TruckLib.Models;

namespace TruckLib.HashFs
{
    /// <summary>
    /// Represents the metadata table values of a packed .tobj/.dds entry.
    /// </summary>
    public struct PackedTobjDdsMetadata
    {
        public int TextureWidth;
        public int TextureHeight;

        internal FlagField ImgFlags;
        internal FlagField SampleFlags;

        public uint MipmapCount => ImgFlags.GetBitString(0, 4) + 1;
        public uint Format => ImgFlags.GetBitString(4, 8);
        public bool IsCube => ImgFlags.GetBitString(12, 2) != 0;
        public uint FaceCount => ImgFlags.GetBitString(14, 6) + 1;
        public int PitchAlignment => 1 << (int)ImgFlags.GetBitString(20, 4);
        public int ImageAlignment => 1 << (int)ImgFlags.GetBitString(24, 4);

        public TobjFilter MagFilter => (TobjFilter)(SampleFlags[0] ? 1 : 0);
        public TobjFilter MinFilter => (TobjFilter)(SampleFlags[1] ? 1 : 0);
        public TobjMipFilter MipFilter => (TobjMipFilter)SampleFlags.GetBitString(2, 2);
        public TobjAddr AddrU => (TobjAddr)SampleFlags.GetBitString(4, 3);
        public TobjAddr AddrV => (TobjAddr)SampleFlags.GetBitString(7, 3);
        public TobjAddr AddrW => (TobjAddr)SampleFlags.GetBitString(10, 3);

        /// <summary>
        /// Creates a <see cref="Tobj"/> object from the metadata.
        /// </summary>
        /// <param name="tobjPath">The absolute path of the .tobj file, 
        /// e.g. <c>"/model/wall/anti_noise.tobj"</c>.</param>
        /// <returns>A <see cref="Tobj"/> object.</returns>
        public Tobj AsTobj(string tobjPath)
        {
            var ddsPath = Path.ChangeExtension(tobjPath, "dds");
            var tobj = new Tobj
            {
                Type = IsCube ? TobjType.CubeMap : TobjType.Map2D,
                MagFilter = MagFilter,
                MinFilter = MinFilter,
                MipFilter = MipFilter,
                AddrU = AddrU,
                AddrV = AddrV,
                AddrW = AddrW,
                Anisotropic = true,
                Compress = true,
                Unknown4 = 1,
                Unknown10 = 1,
                TexturePaths = new List<string> { ddsPath }
            };
            return tobj;
        }
    }
}