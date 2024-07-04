using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.HashFs.Dds
{
    internal static class DdsUtils
    {
        public static byte[] ConvertDecompBytesToDdsBytes(EntryV2 entry, DdsFile dds, byte[] decomp)
        {
            var subData = Enumerable.Range(0, 32).Select(x => new SubresourceData()).ToArray();
            var initData = FillInitData(dds, decomp, subData);

            var ddsDataLength = CalculateDdsDataLength(entry, subData);
            var dst = new byte[ddsDataLength];

            var srcOffset = 0;
            var dstOffset = 0;
            for (int currentFaceIdx = 0; currentFaceIdx < entry.TobjMetadata.Value.FaceCount; currentFaceIdx++)
            {
                for (int mipmapIdx = 0; mipmapIdx < dds.Header.MipMapCount; mipmapIdx++)
                {
                    srcOffset = NearestMultiple(srcOffset, entry.TobjMetadata.Value.ImageAlignment);
                    var s = subData[mipmapIdx];
                    for (int doneBytes = 0; doneBytes < s.SlicePitch; doneBytes += s.RowPitch)
                    {
                        srcOffset = NearestMultiple(srcOffset, entry.TobjMetadata.Value.PitchAlignment);
                        Array.Copy(decomp, srcOffset, dst, dstOffset, s.RowPitch);
                        srcOffset += s.RowPitch;
                        dstOffset += s.RowPitch;
                    }
                }
            }

            return dst;
        }

        public static int CalculateDdsDataLength(EntryV2 entry, SubresourceData[] subData)
        {
            int length = 0;
            for (int i = 0; i < subData.Length; i++)
            {
                var faceCount = (int)entry.TobjMetadata.Value.FaceCount;
                var rowPitch = subData[i].RowPitch;
                var slicePitch = subData[i].SlicePitch;
                if (rowPitch == 0) continue;
                length += faceCount * (slicePitch / rowPitch) * rowPitch;
            }
            return length;
        }

        public static FillInitDataResult FillInitData(DdsFile dds, byte[] data, SubresourceData[] subdata)
        {
            const int maxsize = 0;
            var ret = new FillInitDataResult();

            var srcBitsIdx = 0;
            var endBitsIdx = data.Length;

            var index = 0;
            for (int j = 0; j < dds.HeaderDxt10.ArraySize; j++)
            {
                var width = (int)dds.Header.Width;
                var height = (int)dds.Header.Height;
                var depth = (int)dds.Header.Depth;
                for (int i = 0; i < dds.Header.MipMapCount; i++)
                {
                    var s = GetSurfaceInfo(width, height, dds.HeaderDxt10.Format);
                    if ((dds.Header.MipMapCount <= 1) 
                        || maxsize == 0 
                        || (width <= maxsize && height <= maxsize && depth <= maxsize))
                    {
                        if (ret.TWidth == 0)
                        {
                            ret.TWidth = width;
                            ret.THeight = height;
                            ret.TDepth = depth;
                        }
                        subdata[index].DataIdx = srcBitsIdx;
                        subdata[index].RowPitch = s.RowBytes;
                        subdata[index].SlicePitch = s.NumBytes;
                        index++;
                    }
                    else if (j != 0)
                    {
                        // Count number of skipped mipmaps (first item only)
                        ret.SkipMip++;
                    }

                    if (srcBitsIdx + (s.NumBytes * depth) > endBitsIdx)
                    {
                        throw new EndOfStreamException();
                    }

                    srcBitsIdx += s.NumBytes * depth;

                    width >>= 1;
                    height >>= 1;
                    depth >>= 1;
                    if (width == 0)
                    {
                        width = 1;
                    }
                    if (height == 0)
                    {
                        height = 1;
                    }
                    if (depth == 0)
                    {
                        depth = 1;
                    }
                }
            }

            return ret;
        }

        public static SurfaceInfo GetSurfaceInfo(int width, int height, DxgiFormat format)
        {
            var ret = new SurfaceInfo();

            var bc = false;
            var packed = false;
            var planar = false;
            var bpe = 0;
            switch(format)
            {
                case DxgiFormat.BC1_TYPELESS:
                case DxgiFormat.BC1_UNORM:
                case DxgiFormat.BC1_UNORM_SRGB:
                case DxgiFormat.BC4_TYPELESS:
                case DxgiFormat.BC4_UNORM:
                case DxgiFormat.BC4_SNORM:
                    bc = true;
                    bpe = 8;
                    break;

                case DxgiFormat.BC2_TYPELESS:
                case DxgiFormat.BC2_UNORM:
                case DxgiFormat.BC2_UNORM_SRGB:
                case DxgiFormat.BC3_TYPELESS:
                case DxgiFormat.BC3_UNORM:
                case DxgiFormat.BC3_UNORM_SRGB:
                case DxgiFormat.BC5_TYPELESS:
                case DxgiFormat.BC5_UNORM:
                case DxgiFormat.BC5_SNORM:
                case DxgiFormat.BC6H_TYPELESS:
                case DxgiFormat.BC6H_UF16:
                case DxgiFormat.BC6H_SF16:
                case DxgiFormat.BC7_TYPELESS:
                case DxgiFormat.BC7_UNORM:
                case DxgiFormat.BC7_UNORM_SRGB:
                    bc = true;
                    bpe = 16;
                    break;

                case DxgiFormat.R8G8_B8G8_UNORM:
                case DxgiFormat.G8R8_G8B8_UNORM:
                case DxgiFormat.YUY2:
                    packed = true;
                    bpe = 4;
                    break;

                case DxgiFormat.Y210:
                case DxgiFormat.Y216:
                    packed = true;
                    bpe = 8;
                    break;

                case DxgiFormat.NV12:
                case DxgiFormat._420_OPAQUE:
                    planar = true;
                    bpe = 2;
                    break;

                case DxgiFormat.P010:
                case  DxgiFormat.P016:
                    planar = true;
                    bpe = 4;
                    break;
            }

            if (bc)
            {
                var numBlocksWide = 0;
                if (width > 0)
                {
                    numBlocksWide = Math.Max(1, (width + 3) / 4);
                }
                var numBlocksHigh = 0;
                if (height > 0)
                {
                    numBlocksHigh = Math.Max(1, (height + 3) / 4);
                }
                ret.RowBytes = numBlocksWide * bpe;
                ret.NumRows = numBlocksHigh;
                ret.NumBytes = ret.RowBytes * numBlocksHigh;
            }
            else if (packed)
            {
                ret.RowBytes = ((width + 1) >> 1) * bpe;
                ret.NumRows = height;
                ret.NumBytes = ret.RowBytes * height;
            }
            else if (format == DxgiFormat.NV11)
            {
                ret.RowBytes = ((width + 3) >> 2) * 4;
                // Direct3D makes this simplifying assumption, although it is larger than the 4:1:1 data
                ret.NumRows = height * 2; 
                ret.NumBytes = ret.RowBytes * ret.NumRows;
            }
            else if (planar)
            {
                ret.RowBytes = ((width + 1) >> 1) * bpe;
                ret.NumBytes = (ret.RowBytes * height) + ((ret.RowBytes * height + 1) >> 1);
                ret.NumRows = height + ((height + 1) >> 1);
            }
            else
            {
                int bpp = BitsPerPixel(format);
                ret.RowBytes = (width * bpp + 7) / 8; // round up to nearest byte
                ret.NumRows = height;
                ret.NumBytes = ret.RowBytes * height;
            }

            return ret;
        }

        public static int BitsPerPixel(DxgiFormat format)
        {
            switch(format)
            {
                case DxgiFormat.R32G32B32A32_TYPELESS:
                case DxgiFormat.R32G32B32A32_FLOAT:
                case DxgiFormat.R32G32B32A32_UINT:
                case DxgiFormat.R32G32B32A32_SINT:
                    return 128;

                case DxgiFormat.R32G32B32_TYPELESS:
                case DxgiFormat.R32G32B32_FLOAT:
                case DxgiFormat.R32G32B32_UINT:
                case DxgiFormat.R32G32B32_SINT:
                    return 96;

                case DxgiFormat.R16G16B16A16_TYPELESS:
                case DxgiFormat.R16G16B16A16_FLOAT:
                case DxgiFormat.R16G16B16A16_UNORM:
                case DxgiFormat.R16G16B16A16_UINT:
                case DxgiFormat.R16G16B16A16_SNORM:
                case DxgiFormat.R16G16B16A16_SINT:
                case DxgiFormat.R32G32_TYPELESS:
                case DxgiFormat.R32G32_FLOAT:
                case DxgiFormat.R32G32_UINT:
                case DxgiFormat.R32G32_SINT:
                case DxgiFormat.R32G8X24_TYPELESS:
                case DxgiFormat.D32_FLOAT_S8X24_UINT:
                case DxgiFormat.R32_FLOAT_X8X24_TYPELESS:
                case DxgiFormat.X32_TYPELESS_G8X24_UINT:
                case DxgiFormat.Y416:
                case DxgiFormat.Y210:
                case DxgiFormat.Y216:
                    return 64;

                case DxgiFormat.R10G10B10A2_TYPELESS:
                case DxgiFormat.R10G10B10A2_UNORM:
                case DxgiFormat.R10G10B10A2_UINT:
                case DxgiFormat.R11G11B10_FLOAT:
                case DxgiFormat.R8G8B8A8_TYPELESS:
                case DxgiFormat.R8G8B8A8_UNORM:
                case DxgiFormat.R8G8B8A8_UNORM_SRGB:
                case DxgiFormat.R8G8B8A8_UINT:
                case DxgiFormat.R8G8B8A8_SNORM:
                case DxgiFormat.R8G8B8A8_SINT:
                case DxgiFormat.R16G16_TYPELESS:
                case DxgiFormat.R16G16_FLOAT:
                case DxgiFormat.R16G16_UNORM:
                case DxgiFormat.R16G16_UINT:
                case DxgiFormat.R16G16_SNORM:
                case DxgiFormat.R16G16_SINT:
                case DxgiFormat.R32_TYPELESS:
                case DxgiFormat.D32_FLOAT:
                case DxgiFormat.R32_FLOAT:
                case DxgiFormat.R32_UINT:
                case DxgiFormat.R32_SINT:
                case DxgiFormat.R24G8_TYPELESS:
                case DxgiFormat.D24_UNORM_S8_UINT:
                case DxgiFormat.R24_UNORM_X8_TYPELESS:
                case DxgiFormat.X24_TYPELESS_G8_UINT:
                case DxgiFormat.R9G9B9E5_SHAREDEXP:
                case DxgiFormat.R8G8_B8G8_UNORM:
                case DxgiFormat.G8R8_G8B8_UNORM:
                case DxgiFormat.B8G8R8A8_UNORM:
                case DxgiFormat.B8G8R8X8_UNORM:
                case DxgiFormat.R10G10B10_XR_BIAS_A2_UNORM:
                case DxgiFormat.B8G8R8A8_TYPELESS:
                case DxgiFormat.B8G8R8A8_UNORM_SRGB:
                case DxgiFormat.B8G8R8X8_TYPELESS:
                case DxgiFormat.B8G8R8X8_UNORM_SRGB:
                case DxgiFormat.AYUV:
                case DxgiFormat.Y410:
                case DxgiFormat.YUY2:
                    return 32;

                case DxgiFormat.P010:
                case DxgiFormat.P016:
                    return 24;

                case DxgiFormat.R8G8_TYPELESS:
                case DxgiFormat.R8G8_UNORM:
                case DxgiFormat.R8G8_UINT:
                case DxgiFormat.R8G8_SNORM:
                case DxgiFormat.R8G8_SINT:
                case DxgiFormat.R16_TYPELESS:
                case DxgiFormat.R16_FLOAT:
                case DxgiFormat.D16_UNORM:
                case DxgiFormat.R16_UNORM:
                case DxgiFormat.R16_UINT:
                case DxgiFormat.R16_SNORM:
                case DxgiFormat.R16_SINT:
                case DxgiFormat.B5G6R5_UNORM:
                case DxgiFormat.B5G5R5A1_UNORM:
                case DxgiFormat.A8P8:
                case DxgiFormat.B4G4R4A4_UNORM:
                    return 16;

                case DxgiFormat.NV12:
                case DxgiFormat._420_OPAQUE:
                case DxgiFormat.NV11:
                    return 12;

                case DxgiFormat.R8_TYPELESS:
                case DxgiFormat.R8_UNORM:
                case DxgiFormat.R8_UINT:
                case DxgiFormat.R8_SNORM:
                case DxgiFormat.R8_SINT:
                case DxgiFormat.A8_UNORM:
                case DxgiFormat.AI44:
                case DxgiFormat.IA44:
                case DxgiFormat.P8:
                    return 8;

                case DxgiFormat.R1_UNORM:
                    return 1;

                case DxgiFormat.BC1_TYPELESS:
                case DxgiFormat.BC1_UNORM:
                case DxgiFormat.BC1_UNORM_SRGB:
                case DxgiFormat.BC4_TYPELESS:
                case DxgiFormat.BC4_UNORM:
                case DxgiFormat.BC4_SNORM:
                    return 4;

                case DxgiFormat.BC2_TYPELESS:
                case DxgiFormat.BC2_UNORM:
                case DxgiFormat.BC2_UNORM_SRGB:
                case DxgiFormat.BC3_TYPELESS:
                case DxgiFormat.BC3_UNORM:
                case DxgiFormat.BC3_UNORM_SRGB:
                case DxgiFormat.BC5_TYPELESS:
                case DxgiFormat.BC5_UNORM:
                case DxgiFormat.BC5_SNORM:
                case DxgiFormat.BC6H_TYPELESS:
                case DxgiFormat.BC6H_UF16:
                case DxgiFormat.BC6H_SF16:
                case DxgiFormat.BC7_TYPELESS:
                case DxgiFormat.BC7_UNORM:
                case DxgiFormat.BC7_UNORM_SRGB:
                    return 8;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Rounds up <paramref name="x"/> to the nearest multiple of <paramref name="n"/>.
        /// </summary>
        /// <param name="x">The number to round.</param>
        /// <param name="n">The multiple to round to.</param>
        /// <returns>The smallest multiple of <paramref name="n"/> that is greater
        /// than or equal to <paramref name="x"/>.</returns>
        public static int NearestMultiple(int x, int n)
        {
            return n == 0
                ? x
                : (x + n - 1) / n * n;
        }
    }

    internal struct FillInitDataResult
    {
        public int TWidth;
        public int THeight;
        public int TDepth;
        public int SkipMip;
    }

    internal struct SurfaceInfo
    {
        public int NumBytes;
        public int RowBytes;
        public int NumRows;
    }
}
