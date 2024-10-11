using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TruckLib;

namespace TruckLib.Models
{
    /// <summary>
    /// Represents a texture object file.
    /// </summary>
    public class Tobj : IBinarySerializable
    {
        // TODO:
        // self.transparent
        // TARGET_TYPES = ("", "2d", "cube")
        // self.border_color = []
        // self.black_border = False
        // self.usage = ""
        // USAGE_TYPES = ("", "default", "tsnormal", "ui", "projected")
        // and probably more

        private const uint SupportedVersion = 0x70b10a01;

        public byte Bias { get; set; }

        public TobjType Type { get; set; }

        public TobjFilter MagFilter { get; set; }

        public TobjFilter MinFilter { get; set; }

        public TobjMipFilter MipFilter { get; set; }

        public TobjAddr AddrU { get; set; }

        public TobjAddr AddrV { get; set; }

        public TobjAddr AddrW { get; set; }

        public bool Compress { get; set; }

        public bool Anisotropic { get; set; }

        public byte ColorSpace { get; set; }

        public List<string> TexturePaths { get; set; } = new List<string>(1);

        private uint unknown0;
        private uint unknown1;
        private uint unknown2;
        private uint unknown3;
        public ushort Unknown4 { get; set; }
        private byte unknown5;
        private byte unknown6;
        private byte unknown7;
        private byte unknown8;
        private byte unknown9;
        public byte Unknown10 { get; set; }
        private byte unknown11;

        public static Tobj Open(string tobjPath)
        {
            var tobj = new Tobj();

            using var fs = new FileStream(tobjPath, FileMode.Open);
            using var r = new BinaryReader(fs);
            tobj.Deserialize(r);

            return tobj;
        }

        public void Save(string tobjPath)
        {
            using var fs = new FileStream(tobjPath, FileMode.Create);
            using var w = new BinaryWriter(fs);
            Serialize(w);
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            if (version != SupportedVersion)
                throw new UnsupportedVersionException($"Version {version} is not supported.");

            version = r.ReadUInt32();
            if (version != SupportedVersion)
                throw new UnsupportedVersionException($"Version {version} is not supported.");

            unknown0 = r.ReadUInt32();
            unknown1 = r.ReadUInt32();
            unknown2 = r.ReadUInt32();
            unknown3 = r.ReadUInt32();
            Unknown4 = r.ReadUInt16();
            Bias = r.ReadByte();
            unknown5 = r.ReadByte();
            Type = (TobjType)r.ReadByte();
            unknown6 = r.ReadByte(); 
            MagFilter = (TobjFilter)r.ReadByte();
            MinFilter = (TobjFilter)r.ReadByte();
            MipFilter = (TobjMipFilter)r.ReadByte();
            unknown7 = r.ReadByte(); 
            AddrU = (TobjAddr)r.ReadByte();
            AddrV = (TobjAddr)r.ReadByte();
            AddrW = (TobjAddr)r.ReadByte();
            Compress = r.ReadByte() != 1;
            unknown8 = r.ReadByte(); 
            Anisotropic = r.ReadByte() != 1;
            unknown9 = r.ReadByte();            
            Unknown10 = r.ReadByte();
            ColorSpace = r.ReadByte();
            unknown11 = r.ReadByte(); 

            var txCount = Type == TobjType.CubeMap ? 6 : 1;
            for (int i = 0; i < txCount; i++)
            {
                TexturePaths.Add(r.ReadPascalString());
            }
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(SupportedVersion);

            w.Write(unknown0);
            w.Write(unknown1);
            w.Write(unknown2);
            w.Write(unknown3);
            w.Write(Unknown4);
            w.Write(Bias);
            w.Write(unknown5);
            w.Write((byte)Type);
            w.Write(unknown6);
            w.Write((byte)MagFilter);
            w.Write((byte)MinFilter);
            w.Write((byte)MipFilter);
            w.Write(unknown7);
            w.Write((byte)AddrU);
            w.Write((byte)AddrV);
            w.Write((byte)AddrW);
            w.Write((!Compress).ToByte());
            w.Write(unknown8);
            w.Write((!Anisotropic).ToByte());
            w.Write(unknown9);
            w.Write(Unknown10);
            w.Write(ColorSpace);
            w.Write(unknown11);

            foreach (var path in TexturePaths)
            {
                w.WritePascalString(path);
            }
        }
    }

    public enum TobjType
    {
        Map1D = 1,
        Map2D = 2,
        Map3D = 3,
        CubeMap = 5
    }

    public enum TobjFilter
    {
        Nearest = 0,
        Linear = 1,
        NoMips = 2,
        Default = 3
    }

    public enum TobjMipFilter
    {
        Nearest = 0,
        Trilinear = 1,
        NoMips = 2,
        Default = 3
    }

    public enum TobjAddr
    {
        Repeat = 0, 
        Clamp = 1, 
        ClampToEdge = 2, 
        ClampToBorder = 3,
        Mirror = 4, 
        MirrorClamp = 5, 
        MirrorClampToEdge = 6
    }
}
