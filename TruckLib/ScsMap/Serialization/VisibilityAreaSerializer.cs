using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class VisibilityAreaSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var va = new VisibilityArea(false);
            ReadKdopItem(r, va);

            va.Node = new UnresolvedNode(r.ReadUInt64());
            va.Width = r.ReadSingle();
            va.Height = r.ReadSingle();
            va.Children = ReadItemRefList(r);

            return va;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var va = item as VisibilityArea;
            WriteKdopItem(w, va);

            w.Write(va.Node.Uid);
            w.Write(va.Width);
            w.Write(va.Height);
            WriteItemRefList(w, va.Children);
        }
    }
}
