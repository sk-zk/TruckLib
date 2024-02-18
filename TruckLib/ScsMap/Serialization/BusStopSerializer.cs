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
            var bs = new BusStop(false);
            ReadKdopItem(r, bs);

            bs.CityName = r.ReadToken();
            bs.Prefab = new UnresolvedItem(r.ReadUInt64());
            bs.Node = new UnresolvedNode(r.ReadUInt64());

            return bs;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var bs = item as BusStop;
            WriteKdopItem(w, bs);
            w.Write(bs.CityName);
            w.Write(bs.Prefab.Uid);
            w.Write(bs.Node.Uid);
        }
    }
}
