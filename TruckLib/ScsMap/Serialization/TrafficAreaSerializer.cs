using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap.Serialization
{
    class TrafficAreaSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var ta = new TrafficArea(false);
            ReadKdopItem(r, ta);

            ta.Tags = ReadObjectList<Token>(r);
            ta.Nodes = new PolygonNodeList(ta);
            ta.Nodes.AddRange(ReadNodeRefList(r));
            ta.Rule = r.ReadToken();
            ta.Range = r.ReadSingle();

            return ta;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var ta = item as TrafficArea;
            WriteKdopItem(w, ta);
            WriteObjectList(w, ta.Tags);
            WriteNodeRefList(w, ta.Nodes);
            w.Write(ta.Rule);
            w.Write(ta.Range);
        }
    }
}
