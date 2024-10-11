using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap.Serialization
{
    class MapAreaSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var area = new MapArea(false);
            ReadKdopItem(r, area);

            area.Nodes = new PolygonNodeList(area);
            area.Nodes.AddRange(ReadNodeRefList(r));
            area.Color = r.ReadUInt32();

            return area;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var area = item as MapArea;
            WriteKdopItem(w, area);

            WriteNodeRefList(w, area.Nodes);
            w.Write(area.Color);
        }
    }
}
