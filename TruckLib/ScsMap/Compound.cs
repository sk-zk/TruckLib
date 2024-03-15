using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A compound item, which groups multiple
    /// <see href="https://github.com/sk-zk/map-docs/wiki/base%2C-aux-and-snd">aux items</see>
    /// into one, with an additional parent node to which they are tethered.
    /// </summary>
    /// <remarks>
    /// <para>Both the items and the nodes of compounded items are contained within
    /// its compound parent rather than belonging to the sector itself.</para>
    /// <para>The editor does not allow signs to be added to compounds, but signs without
    /// traffic rules can be added externally and the game will load them without issues.
    /// Likewise, the editor requires a compound to consist of at least two items,
    /// but a compound of only one item is supported (albeit somewhat pointless) if
    /// created here.</para>
    /// </remarks>
    public class Compound : SingleNodeItem, IItemContainer
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Compound;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// Contains all map items owned by this compound.
        /// </summary>
        public Dictionary<ulong, MapItem> MapItems { get; set; }

        IDictionary<ulong, INode> IItemContainer.Nodes => Nodes;

        /// <summary>
        /// Contains all nodes owned by this compound.
        /// </summary>
        public Dictionary<ulong, INode> Nodes { get; set; }

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

        public bool Collision
        {
            get => !Kdop.Flags[4];
            set => Kdop.Flags[4] = !value;
        }

        public bool Shadows
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        public bool MirrorReflection
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        public Compound() : base() { }

        internal Compound(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            MapItems = new();
            Nodes = new();
        }

        /// <summary>
        /// Adds a new, empty compound container to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the parent node.</param>
        /// <returns>The newly created compound.</returns>
        public static Compound Add(IItemContainer map, Vector3 position)
        {
            return Add<Compound>(map, position);
        }

        /// <summary>
        /// Creates a new node and adds it to the compound.
        /// </summary>
        /// <param name="position">The position of the new node.</param>
        /// <param name="isRed">Whether the node is a red node.</param>
        /// <returns>The newly created node.</returns>
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
        /// Creates a new node and adds it to the compound.
        /// </summary>
        /// <param name="position">The position of the new node.</param>
        /// <returns>The newly created node.</returns>
        public Node AddNode(Vector3 position)
        {
            return AddNode(position, false);
        }

        /// <summary>
        /// Adds an item to the compound. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        void IItemContainer.AddItem(MapItem item)
        {
            if (item.ItemFile != ItemFile.Aux)
            {
                // The game will crash without logging an error message if you try to do that.
                throw new InvalidOperationException("A compound can only contain .aux items.");
            }
            MapItems.Add(item.Uid, item);
        }

        /// <summary>
        /// Deletes an item. Nodes that are only used by this item 
        /// will also be deleted.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void Delete(MapItem item)
        {
            // delete item from compound
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
        /// Deletes a node and the items attached to it.
        /// </summary>
        /// <param name="node">The node to delete.</param>
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

        /// <summary>
        /// Moves the parent node to a different location and moves the items contained in
        ///  this compound relative to it.
        /// </summary>
        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            Translate(newPos - Node.Position);
        }

        /// <summary>
        /// Translates the parent node and its children by the given vector.
        /// </summary>
        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);
            foreach (var (_, item) in MapItems)
            {
                item.Translate(translation);
            }
        }
    }  
}
