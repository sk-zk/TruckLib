using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class NoWeatherAreaSerializer : MapItemSerializer
    {
        private const float sizeFactor = 2f;

        public override MapItem Deserialize(BinaryReader r)
        {
            var nwa = new NoWeatherArea(false);
            ReadKdopItem(r, nwa);

            nwa.Width = r.ReadSingle() * sizeFactor;
            nwa.Height = r.ReadSingle() * sizeFactor;
            nwa.FogBehavior = (FogMask)r.ReadInt32();
            nwa.Node = new UnresolvedNode(r.ReadUInt64());

            return nwa;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var nwa = item as NoWeatherArea;
            WriteKdopItem(w, nwa);
            w.Write(nwa.Width / sizeFactor);
            w.Write(nwa.Height / sizeFactor);
            w.Write((int)nwa.FogBehavior);
            w.Write(nwa.Node.Uid);
        }
    }
}
