using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class BuildingSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var bld = new Building(false);
            ReadKdopItem(r, bld);

            bld.Name = r.ReadToken();
            bld.Look = r.ReadToken();

            bld.Node = new UnresolvedNode(r.ReadUInt64());
            bld.ForwardNode = new UnresolvedNode(r.ReadUInt64());

            bld.Length = r.ReadSingle();
            bld.RandomSeed = r.ReadUInt32();
            bld.Stretch = r.ReadSingle();
            bld.HeightOffsets = ReadObjectList<float>(r);

            return bld;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var bld = item as Building;
            WriteKdopItem(w, bld);

            w.Write(bld.Name);
            w.Write(bld.Look);

            w.Write(bld.Node.Uid);
            w.Write(bld.ForwardNode.Uid);

            w.Write(bld.Length);
            w.Write(bld.RandomSeed);
            w.Write(bld.Stretch);
            WriteObjectList(w, bld.HeightOffsets);
        }
    }
}
