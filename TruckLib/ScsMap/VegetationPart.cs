﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
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

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            PartName = r.ReadToken();
            for (int i = 0; i < Vegetation.Length; i++)
            {
                Vegetation[i].Name = r.ReadToken();
                Vegetation[i].Density = r.ReadUInt16() / 10f;
                Vegetation[i].Scale = (VegetationScale)r.ReadUInt16();
            }
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(PartName);
            for (int i = 0; i < Vegetation.Length; i++)
            {
                w.Write(Vegetation[i].Name);
                w.Write((ushort)(Vegetation[i].Density * 10f));
                w.Write((ushort)Vegetation[i].Scale);
            }
        }
    }
}
