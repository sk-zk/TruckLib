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
            var nwa = new NoWeatherArea();
            ReadKdop(r, nwa);

            nwa.Width = r.ReadSingle() * sizeFactor;
            nwa.Height = r.ReadSingle() * sizeFactor;
            nwa.Node = new UnresolvedNode(r.ReadUInt64());

            return nwa;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var nwa = item as NoWeatherArea;
            WriteKdop(w, nwa);
            w.Write(nwa.Width / sizeFactor);
            w.Write(nwa.Height / sizeFactor);
            w.Write(nwa.Node.Uid);
        }
    }
}
