using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class BusStopSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var bs = new BusStop();
            ReadKdop(r, bs);

            bs.CityName = r.ReadToken();
            bs.PrefabLink = new UnresolvedItem(r.ReadUInt64());
            bs.Node = new UnresolvedNode(r.ReadUInt64());

            return bs;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var bs = item as BusStop;
            WriteKdop(w, bs);
            w.Write(bs.CityName);
            w.Write(bs.PrefabLink.Uid);
            w.Write(bs.Node.Uid);
        }
    }
}
