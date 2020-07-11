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
            var comp = new Compound(false);
            ReadKdopItem(r, comp);

            comp.Node = new UnresolvedNode(r.ReadUInt64());

            var itemCount = r.ReadUInt32();
            comp.Items = new List<MapItem>((int)itemCount);
            for (int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var serializer = MapItemSerializerFactory.Get(itemType);
                var item = serializer.Deserialize(r);
                comp.Items.Add(item);
            }

            var nodeCount = r.ReadUInt32();
            comp.Nodes = new List<INode>((int)nodeCount);
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node(false);
                node.Deserialize(r);
                if (!comp.Nodes.Contains(node))
                {
                    comp.Nodes.Add(node);
                }
            }

            comp.UpdateInternalReferences();

            return comp;
        }

        public override void Serialize(BinaryWriter w, MapItem item)
        {
            var comp = item as Compound;
            WriteKdopItem(w, comp);

            w.Write(comp.Node.Uid);

            w.Write(comp.Items.Count);
            foreach (var compItem in comp.Items)
            {
                var itemType = compItem.ItemType;
                w.Write((int)itemType);
                var serializer = MapItemSerializerFactory
                    .Get(itemType);
                serializer.Serialize(w, compItem);
            }

            w.Write(comp.Nodes.Count);
            foreach (var node in comp.Nodes)
            {
                node.Serialize(w);
            }
        }
    }
}
