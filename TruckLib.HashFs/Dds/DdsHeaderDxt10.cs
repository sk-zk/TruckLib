using System.IO;

namespace TruckLib.HashFs.Dds
{
    /// <summary>
    /// <para>DDS header extension to handle resource arrays, DXGI pixel formats that don't map to the
    /// legacy Microsoft DirectDraw pixel format structures, and additional metadata.</para>
    /// <para>This header is present if the FourCC member of the DdsPixelFormat structure is set to "DX10".</para>
    /// </summary>
    internal class DdsHeaderDxt10
    {
        /// <summary>
        /// The surface pixel format (see 
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format">
        /// DXGI_FORMAT</see>).
        /// </summary>
        public DxgiFormat Format { get; set; }

        /// <summary>
        /// Identifies the type of resource.
        /// </summary>
        public D3d10ResourceDimension ResourceDimension { get; set; }

        /// <summary>
        /// Identifies other, less common options for resources.
        /// </summary>
        public D3d10ResourceMiscFlag MiscFlag { get; set; }

        /// <summary>
        /// <para>The number of elements in the array.</para>
        /// <para>For a 2D texture that is also a cube-map texture, this number represents the number
        /// of cubes. This number is the same as the number in the NumCubes member of
        /// D3D10_TEXCUBE_ARRAY_SRV1 or D3D11_TEXCUBE_ARRAY_SRV). In this case, the DDS file contains
        /// ArraySize*6 2D textures. For more information about this case, see the MiscFlag description.</para>
        /// <para>For a 3D texture, you must set this number to 1.</para>
        /// </summary>
        public uint ArraySize { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public uint MiscFlags2 { get; set; }

        public void Deserialize(BinaryReader r)
        {
            Format = (DxgiFormat)r.ReadUInt32();
            ResourceDimension = (D3d10ResourceDimension)r.ReadUInt32();
            MiscFlag = (D3d10ResourceMiscFlag)r.ReadUInt32();
            ArraySize = r.ReadUInt32();
            MiscFlags2 = r.ReadUInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write((int)Format);
            w.Write((int)ResourceDimension);
            w.Write((int)MiscFlag);
            w.Write(ArraySize);
            w.Write(MiscFlags2);
        }
    }
}
