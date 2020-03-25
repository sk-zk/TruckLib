using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class MapOverlaySerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var overlay = new MapOverlay();
            ReadKdop(r, overlay);

            overlay.Look = r.ReadToken();
            overlay.Node = new UnresolvedNode(r.ReadUInt64());

            return overlay;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var overlay = item as MapOverlay;
            WriteKdop(w, overlay); 
            w.Write(overlay.Look);
            w.Write(overlay.Node.Uid);
        }
    }
}
