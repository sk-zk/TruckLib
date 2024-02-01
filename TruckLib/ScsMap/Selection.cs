using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using TruckLib.ScsMap.Serialization;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A selection (.sbd) file.
    /// </summary>
    public class Selection : IItemContainer
    {
        private Header header;

        public Vector3 Origin { get; set; }

        protected KdopBounds KdopBounds;

        /// <summary>
        /// Map items in this selection.
        /// </summary>
        public Dictionary<ulong, MapItem> MapItems { get; set; }

        /// <summary>
        /// Nodes in this selection.
        /// </summary>
        public Dictionary<ulong, INode> Nodes { get; set; }

        /// <summary>
        /// Adds a node to the selection.
        /// </summary>
        /// <inheritdoc/>
        public Node AddNode(Vector3 position, bool isRed)
        {
            var node = new Node
            {
                Position = position,
                IsRed = isRed,
                Parent = this,
            };
            Nodes.Add(node.Uid, node);
            return node;
        }

        /// <summary>
        /// Adds a node to the selection.
        /// </summary>
        /// <inheritdoc/>
        public Node AddNode(Vector3 position)
        {
            return AddNode(position, false);
        }

        /// <summary>
        /// Adds an item to the selection. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="mainNode">The main node of the item. This will determine which sector
        /// contains the item.</param>
        void IItemContainer.AddItem(MapItem item)
        {
            MapItems.Add(item.Uid, item);
        }

        /// <summary>
        /// Deletes an item from the selection.
        /// </summary>
        /// <inheritdoc/>
        public void Delete(MapItem item)
        {
            // delete item
            if (MapItems.ContainsKey(item.Uid))
            {
                MapItems.Remove(item.Uid);
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

        /// <summary>
        /// Deletes a node from the selection.
        /// </summary>
        /// <inheritdoc/>
        public void Delete(INode node)
        {
            if (Nodes.ContainsKey(node.Uid))
            {
                Nodes.Remove(node.Uid);
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

        /// <summary>
        /// Reads a selection file from disk.
        /// </summary>
        /// <param name="sbdPath">Path to the .sbd file.</param>
        /// <returns>A Selection object.</returns>
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
            Nodes = new();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node(false);
                node.Deserialize(r);
                if (!Nodes.ContainsKey(node.Uid))
                {
                    Nodes.Add(node.Uid, node);
                }
                node.Parent = this;
            }
        }

        private void ReadItems(BinaryReader r, uint itemCount)
        {
            MapItems = new();
            for (int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var serializer = MapItemSerializerFactory.Get(itemType);
                var item = serializer.Deserialize(r);

                if (item.HasDataPayload)
                {
                    (serializer as IDataPayload).DeserializeDataPayload(r, item);
                }

                MapItems.Add(item.Uid, item);
            }
        }

        internal void UpdateInternalReferences()
        {
            // first of all, find map items referenced in nodes
            foreach (var (_, node) in Nodes)
            {
                node.UpdateItemReferences(MapItems);
            }

            // then find nodes referenced in map items
            // and map items referenced in map items
            foreach (var (_, item) in MapItems)
            {
                item.UpdateNodeReferences(Nodes);
                if (item is IItemReferences hasItemRef)
                {
                    hasItemRef.UpdateItemReferences(MapItems);
                }
            }
        }

    }
}
