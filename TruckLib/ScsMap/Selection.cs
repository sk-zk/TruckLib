using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using TruckLib.ScsMap.Serialization;

namespace TruckLib.ScsMap
{
    public class Selection : IItemContainer
    {
        private Header header;

        public Vector3 Origin { get; set; }

        protected KdopBounds KdopBounds;

        public List<MapItem> Items { get; set; }


        public List<INode> Nodes { get; set; }


        public Node AddNode(Vector3 position, bool isRed)
        {
            var node = new Node
            {
                Sectors = null,
                Position = position,
                IsRed = isRed
            };
            Nodes.Add(node);
            return node;
        }

        public Node AddNode(Vector3 position)
        {
            return AddNode(position, false);
        }

        /// <summary>
        /// Adds an item to the map. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="mainNode">The main node of the item. This will determine which sector
        /// contains the item.</param>
        void IItemContainer.AddItem(MapItem item)
        {
            Items.Add(item);
        }

        public void Delete(MapItem item)
        {
            // delete item
            if (Items.Contains(item))
            {
                Items.Remove(item);
            }

            // remove item from its nodes, 
            // and delete them if they're orphaned now
            foreach (var node in item.GetItemNodes())
            {
                if (node.ForwardItem == item)
                {
                    node.ForwardItem = null;
                    node.IsRed = false;
                }
                if (node.BackwardItem == item)
                {
                    node.BackwardItem = null;
                }
                if (node.IsOrphaned())
                {
                    Delete(node);
                }
            }
        }

        public void Delete(INode node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
            }

            if (node.ForwardItem is MapItem fw)
            {
                node.ForwardItem = null;
                Delete(fw);
            }

            if (node.BackwardItem is MapItem bw)
            {
                node.BackwardItem = null;
                Delete(bw);
            }
        }
        public IEnumerable<T> GetAllItems<T>() where T : MapItem
        {
            return Items.Where(x => x is T).Cast<T>();
        }

        public Dictionary<ulong, MapItem> GetAllItems()
        {
            return Items.ToDictionary(k => k.Uid, v => v);
        }

        Dictionary<ulong, T> IItemContainer.GetAllItems<T>()
        {
            return GetAllItems<T>().ToDictionary(k => k.Uid, v => v);
        }

        public Dictionary<ulong, INode> GetAllNodes()
        {
            return Nodes.ToDictionary(k => k.Uid, v => v);
        }

        public static Selection Open(string sbdPath)
        {
            var s = new Selection();
            using var r = new BinaryReader(new MemoryStream(File.ReadAllBytes(sbdPath)));

            s.header = new Header();
            s.header.Deserialize(r);

            s.Origin = new Vector3(r.ReadInt32() / 256f, r.ReadInt32() / 256f, r.ReadInt32() / 256f);
            s.KdopBounds = new KdopBounds();
            MapItemSerializer.ReadKdopBounds(r, s.KdopBounds);

            var itemCount = r.ReadUInt32();
            var nodeCount = r.ReadUInt32();

            s.ReadItems(r, itemCount);
            s.ReadNodes(r, nodeCount);

            s.UpdateInternalReferences();

            return s;
        }

        private void ReadNodes(BinaryReader r, uint nodeCount)
        {
            Nodes = new List<INode>((int)nodeCount);
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node(false);
                node.Deserialize(r);
                if (!Nodes.Contains(node))
                {
                    Nodes.Add(node);
                }
            }
        }

        private void ReadItems(BinaryReader r, uint itemCount)
        {
            Items = new List<MapItem>((int)itemCount);
            for (int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var serializer = MapItemSerializerFactory.Get(itemType);
                var item = serializer.Deserialize(r);

                if (item.HasDataPayload)
                {
                    (serializer as IDataPayload).DeserializeDataPayload(r, item);
                }

                Items.Add(item);
            }
        }

        internal void UpdateInternalReferences()
        {
            var itemsDict = GetAllItems();
            var nodesDict = GetAllNodes();

            // first of all, find map items referenced in nodes
            foreach (var node in Nodes)
            {
                node.UpdateItemReferences(itemsDict);
            }

            // then find nodes referenced in map items
            // and map items referenced in map items
            foreach (var item in Items)
            {
                item.UpdateNodeReferences(nodesDict);
                if (item is IItemReferences hasItemRef)
                {
                    hasItemRef.UpdateItemReferences(itemsDict);
                }
            }
        }

    }
}
