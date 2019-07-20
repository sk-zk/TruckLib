using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.Model.Ppd
{
    public class TerrainPointVariant : IBinarySerializable
    {
        public uint Attach0;

        public uint Attach1;

        public void ReadFromStream(BinaryReader r)
        {
            Attach0 = r.ReadUInt32();
            Attach1 = r.ReadUInt32();
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Attach0);
            w.Write(Attach1);
        }
    }
}
