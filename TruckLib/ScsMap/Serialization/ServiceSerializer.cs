using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class ServiceSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var service = new Service();
            ReadKdopItem(r, service);

            service.Node = new UnresolvedNode(r.ReadUInt64());
            service.PrefabLink = new UnresolvedItem(r.ReadUInt64());
            service.Nodes = ReadNodeRefList(r);

            return service;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var service = item as Service;
            WriteKdopItem(w, service);
            w.Write(service.Node.Uid);
            w.Write(service.PrefabLink.Uid);
            WriteNodeRefList(w, service.Nodes);
        }
    }
}
