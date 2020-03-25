using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class CityAreaSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var city = new CityArea();
            ReadKdop(r, city);

            city.Name = r.ReadToken();
            city.Width = r.ReadSingle();
            city.Height = r.ReadSingle();
            city.Node = new UnresolvedNode(r.ReadUInt64());

            return city;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var city = item as CityArea;
            WriteKdop(w, city);
            w.Write(city.Name);
            w.Write(city.Width);
            w.Write(city.Height);
            w.Write(city.Node.Uid);
        }
    }
}
