using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap.Serialization
{
    internal class FuelPumpSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var pump = new FuelPump(false);
            ReadKdopItem(r, pump);

            pump.Node = new UnresolvedNode(r.ReadUInt64());
            pump.Prefab = new UnresolvedItem(r.ReadUInt64());
            pump.Nodes = ReadNodeRefList(r);

            return pump;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var pump = item as FuelPump;
            WriteKdopItem(w, pump);
            w.Write(pump.Node.Uid);

            if (pump.Prefab is null)
            {
                w.Write(0UL);
            }
            else
            {
                w.Write(pump.Prefab.Uid);
            }

            WriteNodeRefList(w, pump.Nodes);
        }
    }
}
