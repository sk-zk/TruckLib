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

            // economy_link_item
            ReadKdopItem(r, bs);
            bs.CityName = r.ReadToken();
            bs.Prefab = new UnresolvedItem(r.ReadUInt64());

            bs.Node = new UnresolvedNode(r.ReadUInt64());

            return bs;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var bs = item as BusStop;

            // economy_link_item
            WriteKdopItem(w, bs);
            w.Write(bs.CityName);
            if (bs.Prefab is null)
            {
                w.Write(0UL);
            }
            else
            {
                w.Write(bs.Prefab.Uid);
            }

            w.Write(bs.Node.Uid);
        }
    }
}
