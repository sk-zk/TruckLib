using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class HingeSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var hinge = new Hinge();
            ReadKdopItem(r, hinge);

            hinge.Model = r.ReadToken();
            hinge.Variant = r.ReadToken();
            hinge.Node = new UnresolvedNode(r.ReadUInt64());
            hinge.MinRotation = r.ReadSingle();
            hinge.MaxRotation = r.ReadSingle();

            return hinge;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var hinge = item as Hinge;
            WriteKdopItem(w, hinge);
            w.Write(hinge.Model);
            w.Write(hinge.Variant);
            w.Write(hinge.Node.Uid);
            w.Write(hinge.MinRotation);
            w.Write(hinge.MaxRotation);
        }
    }
}
