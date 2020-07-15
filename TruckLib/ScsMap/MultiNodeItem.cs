using System.Collections.Generic;
using System.Numerics;

namespace TruckLib.ScsMap
{
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
            CreateNodes(map, positions, item);
            map.AddItem(item);
            return item;
        }

        /// <summary>
        /// Creates map nodes for this item.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="nodePositions"></param>
        /// <param name="item"></param>
        protected static void CreateNodes(IItemContainer map, IList<Vector3> nodePositions, MultiNodeItem item)
        {
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
                node.ForwardItem = item;
                item.Nodes.Add(node);
            }
        }

        internal override IEnumerable<INode> GetItemNodes() => new List<INode>(Nodes);

        internal override INode GetMainNode() => Nodes[0];

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }
    }
}