using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class FerrySerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var ferry = new Ferry(false);
            ReadKdopItem(r, ferry);

            ferry.Port = r.ReadToken();
            ferry.PrefabLink = new UnresolvedItem(r.ReadUInt64());
            ferry.Node = new UnresolvedNode(r.ReadUInt64());
            ferry.UnloadOffset = r.ReadVector3();

            return ferry;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var ferry = item as Ferry;
            WriteKdopItem(w, ferry);
            w.Write(ferry.Port);
            w.Write(ferry.PrefabLink.Uid);
            w.Write(ferry.Node.Uid);
            w.Write(ferry.UnloadOffset);
        }
    }
}
