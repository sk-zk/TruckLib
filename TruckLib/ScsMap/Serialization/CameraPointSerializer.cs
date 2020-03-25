using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class CameraPointSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var point = new CameraPoint();
            ReadKdopItem(r, point);

            point.Tags = ReadObjectList<Token>(r);
            point.Node = new UnresolvedNode(r.ReadUInt64());

            return point;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var point = item as CameraPoint;
            WriteKdopItem(w, point);
            WriteObjectList(w, point.Tags);
            w.Write(point.Node.Uid);
        }
    }
}
