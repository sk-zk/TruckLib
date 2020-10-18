using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class CutsceneSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var cs = new Cutscene(false);
            ReadKdopItem(r, cs);

            cs.Tags = ReadObjectList<Token>(r);
            cs.Node = new UnresolvedNode(r.ReadUInt64());
            cs.Actions = ReadObjectList<CutsceneAction>(r);

            return cs;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var cs = item as Cutscene;
            WriteKdopItem(w, cs);

            WriteObjectList(w, cs.Tags);
            w.Write(cs.Node.Uid);
            WriteObjectList(w, cs.Actions);
        }
    }
}
