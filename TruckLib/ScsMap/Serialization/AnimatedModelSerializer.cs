using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class AnimatedModelSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var am = new AnimatedModel();
            ReadKdop(r, am);
            am.Tags = ReadObjectList<Token>(r);
            am.Model = r.ReadToken();
            am.Node = new UnresolvedNode(r.ReadUInt64());
            return am;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var am = item as AnimatedModel;
            WriteKdop(w, am);
            WriteObjectList(w, am.Tags);
            w.Write(am.Model);
            w.Write(am.Node.Uid);
        }
    }
}
