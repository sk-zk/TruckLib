using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A compound item which holds multiple scenery items. 
    /// </summary>
    public class Compound : SingleNodeItem, IItemContainer
    {
        public override ItemType ItemType => ItemType.Compound;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// Contains all map items owned by this compound.
        /// </summary>
        public Dictionary<ulong, MapItem> CompoundItems { get; set; } 
            = new Dictionary<ulong, MapItem>();

        /// <summary>
        /// Contains all nodes owned by this compound.
        /// </summary>
        public Dictionary<ulong, Node> CompoundNodes { get; set; } 
            = new Dictionary<ulong, Node>();

        /// <summary>
        /// Determines if the compounded items are reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Determines if the compounded items will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Creates a new, empty Compound container.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Compound Add(IItemContainer map, Vector3 position)
        {
            return Add<Compound>(map, position);
        }

        public Node AddNode(Vector3 position, bool isRed)
        {
            var node = new Node
            {
                Sectors = null,
                Position = position,
                IsRed = isRed
            };
            CompoundNodes.Add(node.Uid, node);
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
        void IItemContainer.AddItem(MapItem item, Node mainNode)
        {
            if(item.ItemFile != ItemFile.Aux)
            {
                // The game will crash without logging an error message if you try to do that.
                throw new InvalidOperationException("A compound can only contain .aux items.");
            }
            CompoundItems.Add(item.Uid, item);
        }

        public Dictionary<ulong, MapItem> GetAllItems()
        {
            return CompoundItems;
        }

        public List<T> GetAllItems<T>() where T : MapItem
        {
            var allItems = new List<T>();
            foreach (var item in CompoundItems)
            {
                if (item is T)
                {
                    allItems.Add((T)item.Value);
                }
            }       
            return allItems;
        }

        public Dictionary<ulong, Node> GetAllNodes()
        {
            return CompoundNodes;
        }

        /// <summary>
        /// Deletes an item. Nodes that are only used by this item 
        /// will also be deleted.
        /// </summary>
        /// <param name="item"></param>
        public void Delete(MapItem item)
        {
            // delete item from compound
            if (CompoundItems.ContainsKey(item.Uid))
            {
                CompoundItems.Remove(item.Uid);
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
        /// Deletes a node and the items attached to it.
        /// </summary>
        /// <param name="node"></param>
        public void Delete(Node node)
        {
            if (CompoundNodes.ContainsKey(node.Uid))
            {
                CompoundNodes.Remove(node.Uid);
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

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Node = new UnresolvedNode(r.ReadUInt64());

            var itemCount = r.ReadUInt32();
            for(int i = 0; i < itemCount; i++)
            {
                var itemType = (ItemType)r.ReadInt32();

                var item = MapItemFactory.Create(itemType);
                item.ReadFromStream(r);
                CompoundItems.Add(item.Uid, item);
            }

            var nodeCount = r.ReadUInt32();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new Node();
                node.ReadFromStream(null, r);
                if (!CompoundNodes.ContainsKey(node.Uid))
                {
                    CompoundNodes.Add(node.Uid, node);
                }
            }

            UpdateInternalNodeReferences();
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Node.Uid);

            w.Write(CompoundItems.Count);
            foreach(var item in CompoundItems)
            {
                var itemType = (int)item.Value.ItemType;
                w.Write(itemType);
                item.Value.WriteToStream(w);
            }

            w.Write(CompoundNodes.Count);
            foreach(var nodeKvp in CompoundNodes)
            {
                nodeKvp.Value.WriteToStream(w);
            }
        }

        private void UpdateInternalNodeReferences()
        {
            // first of all, find map items referenced in nodes
            foreach (var kvp in CompoundNodes)
            {
                kvp.Value.UpdateItemReferences(CompoundItems);
            }

            // then find nodes referenced in map items
            // and map items referenced in map items
            foreach (var item in CompoundItems)
            {
                item.Value.UpdateNodeReferences(CompoundNodes);
                if (item.Value is IItemReferences hasItemRef)
                {
                    hasItemRef.UpdateItemReferences(CompoundItems);
                }
            }
        }
    }  
}
