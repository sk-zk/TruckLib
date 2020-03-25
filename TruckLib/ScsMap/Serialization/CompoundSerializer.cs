using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TruckLib.ScsMap.Serialization
{
    class CompoundSerializer : MapItemSerializer
    {
        public override MapItem Deserialize(BinaryReader r)
        {
            var comp = new Compound();
            ReadKdop(r, comp);

            comp.Node = new UnresolvedNode(r.ReadUInt64());

            var itemCount = r.ReadUInt32();
            /*
            for (int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var item = MapItemFactory.Create(itemType);
                item.ReadFromStream(r);
                comp.CompoundItems.Add(item.Uid, item);
            }

            var nodeCount = r.ReadUInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node();
                node.ReadFromStream(null, r);
                if (!comp.CompoundNodes.ContainsKey(node.Uid))
                {
                    comp.CompoundNodes.Add(node.Uid, node);
                }
            }
            */ // TODO

            comp.UpdateInternalNodeReferences();

            return comp;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var comp = item as Compound;
            WriteKdop(w, comp);

            w.Write(comp.Node.Uid);

            w.Write(comp.CompoundItems.Count);
            foreach (var compItem in comp.CompoundItems)
            {
                var itemType = (int)compItem.Value.ItemType;
                w.Write(itemType);
                // TODO item.Value.WriteToStream(w);
            }

            w.Write(comp.CompoundNodes.Count);
            foreach (var nodeKvp in comp.CompoundNodes)
            {
                nodeKvp.Value.WriteToStream(w);
            }
        }
    }
}
