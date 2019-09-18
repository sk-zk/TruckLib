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
            
        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
