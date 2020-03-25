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
            ReadKdopItem(r, comp);

            comp.Node = new UnresolvedNode(r.ReadUInt64());

            var itemCount = r.ReadUInt32();
            
            for (int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var serializer = MapItemSerializerFactory.Get(itemType);
                var item = serializer.Deserialize(r);
                comp.CompoundItems.Add(item);
            }

            var nodeCount = r.ReadUInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node();
                node.ReadFromStream(null, r);
                if (!comp.CompoundNodes.Contains(node))
                {
                    comp.CompoundNodes.Add(node);
                }
            }

            comp.UpdateInternalNodeReferences();

            return comp;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var comp = item as Compound;
            WriteKdopItem(w, comp);

            w.Write(comp.Node.Uid);

            w.Write(comp.CompoundItems.Count);
            foreach (var compItem in comp.CompoundItems)
            {
                var itemType = compItem.ItemType;
                w.Write((int)itemType);
                var serializer = MapItemSerializerFactory
                    .Get(itemType);
                serializer.Serialize(w, compItem);
            }

            w.Write(comp.CompoundNodes.Count);
            foreach (var node in comp.CompoundNodes)
            {
                node.WriteToStream(w);
            }
        }
    }
}
