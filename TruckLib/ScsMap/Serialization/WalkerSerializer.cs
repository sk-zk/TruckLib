using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap.Serialization
{
    class WalkerSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var walker = new Walker(false);
            ReadKdopItem(r, walker);

            walker.NamePrefix = r.ReadToken();
            walker.Speed = r.ReadSingle();
            walker.EndDelay = r.ReadSingle();
            walker.Count = r.ReadUInt32();
            walker.Width = r.ReadSingle();
            walker.Angle = r.ReadSingle();
            var lengths = ReadObjectList<float>(r);
            walker.Nodes = new PathNodeList(walker);
            walker.Nodes.AddRange(ReadNodeRefList(r), false);
            walker.Nodes.Lengths = lengths;

            return walker;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var walker = item as Walker;
            WriteKdopItem(w, walker);

            w.Write(walker.NamePrefix);
            w.Write(walker.Speed);
            w.Write(walker.EndDelay);
            w.Write(walker.Count);
            w.Write(walker.Width);
            w.Write(walker.Angle);
            WriteObjectList(w, walker.Lengths);
            WriteNodeRefList(w, walker.Nodes);
        }
    }
}
