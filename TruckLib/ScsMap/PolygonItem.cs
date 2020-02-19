using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for items which define a polygonal area.
    /// </summary>
    public abstract class PolygonItem : MapItem
    {
        /// <summary>
        /// The nodes of the polygon.
        /// </summary>
        public List<Node> Nodes { get; set; } = new List<Node>();

        /// <summary>
        /// Base method for adding a new PolygonItem to the map.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        public static T Add<T>(IItemContainer map, Vector3[] positions) where T : PolygonItem, new()
        {
            var poly = new T();
            CreateNodes(map, positions, poly);
            map.AddItem(poly, poly.Nodes.First());
            return poly;
        }

        /// <summary>
        /// Creates map nodes for this item.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="nodePositions"></param>
        /// <param name="item"></param>
        protected static void CreateNodes(IItemContainer map, Vector3[] nodePositions, PolygonItem item)
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

        /// <summary>
        /// Appends a node to the item.
        /// </summary>
        /// <param name="position"></param>
        public void Append(Vector3 position)
        {
            var node = Nodes.First().Sectors[0].Map.AddNode(position);
            node.ForwardItem = this;
            Nodes.Add(node);
        }

        /// <summary>
        /// Moves the item, with the given parameter as the new position
        /// of the Nth node.
        /// </summary>
        /// <param name="newPos"></param>
        /// <param name="n"></param>
        public void Move(Vector3 newPos, int n = 0)
        {
            if (n < 0 || n >= Nodes.Count) 
                throw new ArgumentOutOfRangeException();

            var translation = newPos - Nodes[n].Position;
            MoveRel(translation);
        }

        public void MoveRel(Vector3 translation)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Node node = Nodes[i];
                node.Move(node.Position + translation);
            }
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new List<Node>(Nodes);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode && allNodes.ContainsKey(Nodes[i].Uid))
                {
                    Nodes[i] = allNodes[Nodes[i].Uid];
                }
            }
        }   
        
    }
}
