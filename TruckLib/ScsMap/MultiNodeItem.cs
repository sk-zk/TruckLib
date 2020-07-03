using System.Collections.Generic;
using System.Numerics;

namespace TruckLib.ScsMap
{
    public abstract class MultiNodeItem : MapItem
    {
        /// <summary>
        /// The nodes of the item.
        /// </summary>
        public List<Node> Nodes { get; set; }

        protected MultiNodeItem() : base() { }

        internal MultiNodeItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Base method for adding a new MultiNodeItem to the map.
        /// </summary>
        internal static T Add<T>(IItemContainer map, Vector3[] positions) where T : MultiNodeItem, new()
        {
            var item = new T();
            CreateNodes(map, positions, item);
            map.AddItem(item, item.Nodes[0]);
            return item;
        }

        /// <summary>
        /// Creates map nodes for this item.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="nodePositions"></param>
        /// <param name="item"></param>
        protected static void CreateNodes(IItemContainer map, Vector3[] nodePositions, MultiNodeItem item)
        {
            // all nodes have the item as ForwardItem; 
            // how the nodes connect is determined by their position in the list only
            for (int i = 0; i < nodePositions.Length; i++)
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

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new List<Node>(Nodes);
        }

        internal override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }
    }
}