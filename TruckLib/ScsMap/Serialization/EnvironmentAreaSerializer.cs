using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class EnvironmentAreaSerializer : MapItemSerializer
    {
        private const float sizeFactor = 2f;

        public override MapItem Deserialize(BinaryReader r)
        {
            var ea = new EnvironmentArea(false);
            ReadKdopItem(r, ea);

            ea.Width = r.ReadSingle() * sizeFactor;
            ea.Height = r.ReadSingle() * sizeFactor;
            ea.FogBehavior = (FogMask)r.ReadInt32();
            ea.Climate = r.ReadToken();
            ea.ReflectionCube = r.ReadToken();
            ea.Node = new UnresolvedNode(r.ReadUInt64());

            return ea;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var ea = item as EnvironmentArea;
            WriteKdopItem(w, ea);
            w.Write(ea.Width / sizeFactor);
            w.Write(ea.Height / sizeFactor);
            w.Write((int)ea.FogBehavior);
            w.Write(ea.Climate);
            w.Write(ea.ReflectionCube);
            w.Write(ea.Node.Uid);
        }
    }
}
