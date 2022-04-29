using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class MoverSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var mover = new Mover(false);
            ReadKdopItem(r, mover);

            mover.Tags = ReadObjectList<Token>(r);
            mover.Model = r.ReadToken();
            mover.Look = r.ReadToken();
            mover.Variant = r.ReadToken();
            mover.Speed = r.ReadSingle();
            mover.EndDelay = r.ReadSingle();
            mover.Width = r.ReadSingle();
            mover.Count = r.ReadUInt32();
            mover.Lengths = ReadObjectList<float>(r);
            mover.Nodes = ReadNodeRefList(r);

            return mover;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var mover = item as Mover;
            WriteKdopItem(w, mover);

            WriteObjectList(w, mover.Tags);
            w.Write(mover.Model);
            w.Write(mover.Look);
            w.Write(mover.Variant);
            w.Write(mover.Speed);
            w.Write(mover.EndDelay);
            w.Write(mover.Width);
            w.Write(mover.Count);
            WriteObjectList(w, mover.Lengths);
            WriteNodeRefList(w, mover.Nodes);
        }
    }
}
