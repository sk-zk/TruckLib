using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd
{
    public class TerrainPointVariant : IBinarySerializable
    {
        public uint Attach0 { get; set; }

        public uint Attach1 { get; set; }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Attach0 = r.ReadUInt32();
            Attach1 = r.ReadUInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Attach0);
            w.Write(Attach1);
        }
    }
}
