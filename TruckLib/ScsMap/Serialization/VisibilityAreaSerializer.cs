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

            var childrenCount = r.ReadUInt32();
            if (childrenCount > 0)
            {
                throw new NotImplementedException();
            }

            return va;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            throw new NotImplementedException();
        }
    }
}
