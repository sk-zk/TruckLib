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
    /// A compound item which holds multiple aux items. 
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
        public List<MapItem> Items { get; set; } 


        /// <summary>
        /// Contains all nodes owned by this compound.
        /// </summary>
        public List<INode> Nodes { get; set; }

        /// <summary>
        /// Gets or sets if the items are reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Gets or sets if the compounded items will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public Compound() : base() { }

        internal Compound(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Items = new List<MapItem>();
            Nodes = new List<INode>();
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
        void IItemContainer.AddItem(MapItem item)
        {
            if(item.ItemFile != ItemFile.Aux)
            {
                // The game will crash without logging an error message if you try to do that.
                throw new InvalidOperationException("A compound can only contain .aux items.");
            }
            Items.Add(item);
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

        /// <summary>
        /// Deletes an item. Nodes that are only used by this item 
        /// will also be deleted.
        /// </summary>
        /// <param name="item"></param>
        public void Delete(MapItem item)
        {
            // delete item from compound
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

        /// <summary>
        /// Deletes a node and the items attached to it.
        /// </summary>
        /// <param name="node"></param>
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
