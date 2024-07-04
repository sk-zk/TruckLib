using System.IO;

namespace TruckLib.HashFs.Dds
{
    /// <summary>
    /// Represents a DDS file.
    /// Only the header is properly de/serialized; the rest of the file is just a binary blob.
    /// </summary>
    internal class DdsFile
    {
        private const int Magic = 0x20534444; // "DDS "

        /// <summary>
        /// The DDS file header.
        /// </summary>
        public DdsHeader Header { get; set; }

        /// <summary>
        /// If the DdsPixelFormat Flags is set to DDPF_FOURCC and FourCC is set to "DX10",
        /// an additional DdsHeaderDxt10 structure will be present to accommodate texture arrays
        /// or DXGI formats that cannot be expressed as an RGB pixel format
        /// such as floating point formats, sRGB formats etc. 
        /// </summary>
        public DdsHeaderDxt10 HeaderDxt10 { get; set; }

        /// <summary>
        /// The surface data of the DDS file.
        /// </summary>
        public byte[] Data { get; set; }

        public void Deserialize(BinaryReader r)
        {
            var magic = r.ReadInt32();
            if (magic != Magic) 
            {
                throw new InvalidDataException("Not a DDS filo");
            }

            Header = new DdsHeader();
            Header.Deserialize(r);

            if (Header.PixelFormat.FourCC == DdsPixelFormat.FourCC_DX10)
            {
                HeaderDxt10 = new DdsHeaderDxt10();
                HeaderDxt10.Deserialize(r);
            }

            Data = r.ReadBytes((int)(r.BaseStream.Length - r.BaseStream.Position));          
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Magic);

            Header.Serialize(w);
            if (Header.PixelFormat.FourCC == DdsPixelFormat.FourCC_DX10)
            {
                HeaderDxt10.Serialize(w);
            }

            w.Write(Data);
        }
    }
}
