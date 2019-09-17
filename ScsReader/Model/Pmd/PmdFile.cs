using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScsReader.Model.Pmd
{
    public class PmdFile : IBinarySerializable
    {
        uint Version { get; set; } = 4;

        public List<Token> Looks { get; set; } = new List<Token>();

        public List<Variant> Variants { get; set; } = new List<Variant>();

        public List<string> Materials { get; set; } = new List<string>();

        public void Open(string path)
        {
            using (var r = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                ReadFromStream(r);
            }
        }

        public void Save(string path)
        {
            using (var w = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                WriteToStream(w);
            }
        }

        public void ReadFromStream(BinaryReader r)
        {
            Version = r.ReadUInt32();

            var materialCount = r.ReadUInt32();
            var lookCount = r.ReadUInt32();
            var pieceCount = r.ReadUInt32();
            var variantCount = r.ReadUInt32();
            var partCount = r.ReadUInt32();
            var attribsCount = r.ReadUInt32();

            var attribsValuesSize = r.ReadUInt32();
            var materialBlockSize = r.ReadUInt32();

            var lookOffset = r.ReadUInt32();
            var variantOffset = r.ReadUInt32();
            var partAttribsOffset = r.ReadUInt32();
            var attribsValueOffset = r.ReadUInt32();
            var attribsOffset = r.ReadUInt32();
            var materialOffset = r.ReadUInt32();
            var materialDataOffset = r.ReadUInt32();

            for (int i = 0; i < lookCount; i++)
            {
                Looks.Add(r.ReadToken());
            }

            for (int i = 0; i < variantCount; i++)
            {
                Variants.Add(new Variant() { Name = r.ReadToken() });
            }

            // TODO: what is this?
            for(int i = 0; i < partCount; i++)
            {
                var from = r.ReadInt32();
                var to = r.ReadInt32();
                Console.WriteLine($"{to - from}");
            }

            // attribs header
            // each variant has the same attribs
            for (int i = 0; i < attribsCount; i++)
            {
                var name = r.ReadToken();
                var type = r.ReadInt32();
                var offset = r.ReadInt32();
                foreach (var variant in Variants)
                {
                    var part = new Part();
                    variant.Parts.Add(part);
                    var attrib = new PartAttribute();
                    part.Attributes.Add(attrib);
                    attrib.Tag = name;
                    attrib.Type = type;
                }
            }

            // attribs values
            // TODO: Find out if there are any files where a part has 
            // more than one attrib or if "visible" is actually the only attrib
            // that exists
            for (int i = 0; i < variantCount; i++)
            {
                for (int j = 0; j < attribsCount; j++)
                {
                    Variants[i].Parts[j].Attributes[0].Value = r.ReadUInt32();
                }
            }

            // material offsets; I think we can get away with ignoring this?
            var materialsOffset = new List<uint>();
            for (int i = 0; i < lookCount * materialCount; i++)
            {
                materialsOffset.Add(r.ReadUInt32());
            }

            // look material paths
            var materialsData = r.ReadBytes((int)materialBlockSize);
            Materials = StringUtils.CStringBytesToList(materialsData);
            // TODO: How to find out which materials belong to which look?

        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
