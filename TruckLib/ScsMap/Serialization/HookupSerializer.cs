using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class HookupSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var hookup = new Hookup(false);
            ReadKdopItem(r, hookup);

            hookup.Name = r.ReadPascalString();
            hookup.Node = new UnresolvedNode(r.ReadUInt64());

            return hookup;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var hookup = item as Hookup;

            WriteKdopItem(w, hookup);
            w.WritePascalString(hookup.Name);
            w.Write(hookup.Node.Uid);
        }
    }
}
