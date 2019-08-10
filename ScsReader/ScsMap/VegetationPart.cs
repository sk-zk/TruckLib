using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    public class VegetationPart : IBinarySerializable
    {
        public Token PartName { get; set; }

        public Vegetation[] Vegetation { get; set; }

        public VegetationPart()
        {
            const int vegetationCount = 2;
            Vegetation = (new Vegetation[vegetationCount]).Select(x => new Vegetation()).ToArray();
        }

        public void ReadFromStream(BinaryReader r)
        {
            PartName = r.ReadToken();
            for (int i = 0; i < Vegetation.Length; i++)
            {
                Vegetation[i].VegetationName = r.ReadToken();
                Vegetation[i].Density = r.ReadUInt16() / 10f;
                Vegetation[i].Scale = (VegetationScale)r.ReadUInt16();
            }
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(PartName);
            for (int i = 0; i < Vegetation.Length; i++)
            {
                w.Write(Vegetation[i].VegetationName);
                w.Write((ushort)(Vegetation[i].Density * 10f));
                w.Write((ushort)Vegetation[i].Scale);
            }
        }
    }
}
