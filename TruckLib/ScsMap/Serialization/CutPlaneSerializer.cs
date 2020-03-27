using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class CutPlaneSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var plane = new CutPlane(false);
            ReadKdopItem(r, plane);

            plane.Nodes = ReadNodeRefList(r);

            return plane;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var plane = item as CutPlane;
            WriteKdopItem(w, plane);

            WriteNodeRefList(w, plane.Nodes);
        }
    }
}
