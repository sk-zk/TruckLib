using System;
using System.Collections.Generic;
using System.Numerics;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for <see cref="PathItem"/> and <see cref="PolygonItem"/>.
    /// </summary>
    public abstract class MultiNodeItem : MapItem
    {
        /// <summary>
        /// The nodes of the item.
        /// </summary>
        public List<INode> Nodes { get; set; }

        protected MultiNodeItem() : base() { }

        internal MultiNodeItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Base method for adding a new MultiNodeItem to the map.
        /// </summary>
        internal static T Add<T>(IItemContainer map, IList<Vector3> positions) where T : MultiNodeItem, new()
        {
            var item = new T();
            item.CreateNodes(map, positions);
            map.AddItem(item);
            return item;
        }

        /// <summary>
        /// Creates map nodes for this item. 
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="nodePositions">The positions of the nodes.</param>
        protected virtual void CreateNodes(IItemContainer map, IList<Vector3> nodePositions)
        {
            if (Nodes.Count != 0)
            {
                throw new InvalidOperationException("Map item already has nodes.");
            }

            // all nodes have the item as ForwardItem; 
            // how the nodes connect is determined by their position in the list only
            for (int i = 0; i < nodePositions.Count; i++)
            {
                var node = map.AddNode(nodePositions[i]);
                if (i == 0)
                {
                    // one node has to have the red node flag.
                    // without it, the item can't be deleted.
                    node.IsRed = true;
                }
                node.ForwardItem = this;
                Nodes.Add(node);
            }
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes() => new List<INode>(Nodes);

        /// <inheritdoc/>
        internal override Vector3 GetCenter()
        {
            var acc = Vector3.Zero;
            foreach (var node in Nodes)
            {
                acc += node.Position;
            }
            return acc / Nodes.Count;
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }
    }
}