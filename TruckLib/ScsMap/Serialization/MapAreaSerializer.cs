using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class MapAreaSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var area = new MapArea();
            ReadKdop(r, area);

            area.Nodes = ReadNodeRefList(r);
            area.Color = (MapAreaColor)r.ReadUInt32();

            return area;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var area = item as MapArea;
            WriteKdop(w, area);

            WriteNodeRefList(w, area.Nodes);
            w.Write((uint)area.Color);
        }
    }
}
