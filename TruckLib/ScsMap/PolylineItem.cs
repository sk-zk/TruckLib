using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// <para>Parent class for map items which form a polyline with other 
    /// items, like roads and buildings.</para>
    /// </summary>
    public abstract class PolylineItem : MapItem, IRecalculatable
    {
        /// <summary>
        /// The item attached to this item in backward direction.
        /// </summary>
        public MapItem BackwardItem => (MapItem)Node.BackwardItem;

        /// <summary>
        /// The item attached to this item in forward direction.
        /// </summary>
        public MapItem ForwardItem => (MapItem)ForwardNode.ForwardItem;

        /// <summary>
        /// The backward node / the node which holds this item as ForwardItem
        /// (= the node which highlights the item if you mouse over it).
        /// </summary>
        public Node Node { get; set; }

        /// <summary>
        /// The forward node (= the one which is green on a single segment of this item).
        /// </summary>
        public Node ForwardNode { get; set; }

        /// <summary>
        /// The length of the item's path.
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// Creates a single, unconnected item. 
        /// <para>This method will create an empty item, add two new nodes to 
        /// the map, update fwd/bwd references and then add the item to the map.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the item that will be created.</typeparam>
        /// <param name="map">The map the item will be added to.</param>
        /// <param name="backwardPos">The backward position of the item.</param>
        /// <param name="forwardPos">The forward position of the item.</param>
        /// <returns>A new, empty item which has been added to the map.</returns>
        internal static T Add<T>(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos)
            where T : PolylineItem, new()
        {
            var backwardNode = map.AddNode(backwardPos, true);
            var forwardNode = map.AddNode(forwardPos, false);

            var newItem = new T();
            backwardNode.ForwardItem = newItem;
            forwardNode.BackwardItem = newItem;
            newItem.Node = backwardNode;
            newItem.ForwardNode = forwardNode;

            // rotation:
            // for the first two nodes, the Rotation of the nodes is just
            // the angle formed by pos2-pos1 and the Z axis
            var rotation = MathEx.GetNodeRotation(backwardPos, forwardPos);
            newItem.Node.Rotation = rotation;
            newItem.ForwardNode.Rotation = rotation; 

            map.AddItem(newItem, forwardNode);
            return newItem;
        }

        /// <summary>
        /// Appends a new item to this one in forward direction. 
        /// <para>This method will create an empty item, add a new node to the
        /// map, update fwd/bwd references, and add the item to the map.
        /// All item-specific initialization must be done in the child 
        /// class afterwards.</para>
        /// </summary>
        /// <typeparam name="T">The type of the item that will be appended.</typeparam>
        /// <param name="position">The position of the new forward node. </param>
        /// <returns>An new, empty item which has been appended.</returns>
        internal T Append<T>(Vector3 position) where T : PolylineItem, new()
        {
            /* for readability:
             * p3 = the new node that is being created
             * p2 = the current forward node
             * p1 = the current backward node (= Node)
             *
             *  x--------------x------------x
             *  p1             p2           p3  <- we're adding this node
             *  Node        ForwardNode     new node
             *
             */
            var p2 = ForwardNode;
            var p1 = Node;

            if (ForwardItem != null) // there's already an item attached
            {
                throw new ArgumentOutOfRangeException("Can't append item: ForwardItem is not null");
            }

            var p3 = p2.Sectors[0].Map.AddNode(position, false);
            var newItem = new T
            {
                Node = p2,
                ForwardNode = p3
            };
            p2.ForwardItem = newItem;
            p3.BackwardItem = newItem;

            p3.Rotation = MathEx.GetNodeRotation(p2.Position, p3.Position);
            SetMiddleRotation(p1, p2, p3);

            p2.IsRed = true;
            p2.Sectors[0].MapItems.Add(newItem.Uid, newItem);

            return newItem;
        }

        internal T Prepend<T>(Vector3 position) where T : PolylineItem, new()
        {
            /* for readability:
             * p0 = the new node that is being created
             * p1 = the current backward node (= Node)
             * p2 = the current forward node
             *
             *  x--------------x------------x
             *  p0             p1           p2 
             *  new node      Node       ForwardNode
             *  ^ we're adding this one
             */
            var p1 = Node;
            var p2 = ForwardNode;

            if (BackwardItem != null) // there's already an item attached
            {
                throw new ArgumentOutOfRangeException("Can't prepend item: BackwardItem is not null");
            }

            var p0 = p1.Sectors[0].Map.AddNode(position, true);
            var newItem = new T
            {
                Node = p0,
                ForwardNode = p1
            };
            p0.ForwardItem = newItem;
            p1.BackwardItem = newItem;

            p0.Rotation = MathEx.GetNodeRotation(p0.Position, p1.Position);
            SetMiddleRotation(p2, p1, p0, true);

            p0.Sectors[0].MapItems.Add(newItem.Uid, newItem);

            return newItem;
        }

        private static void SetMiddleRotation(Node pBackw, Node pMiddle, Node pNew,
            bool isPrepend = false)
        {
            var pNew_yaw = pNew.Rotation.ToEuler().Y;

            var backMiddle_dir_yaw = MathEx.GetNodeRotation(
                pBackw.Position, pMiddle.Position).ToEuler().Y;

            var middle_yaw = (pNew_yaw + backMiddle_dir_yaw) / 2;
            pMiddle.Rotation = Quaternion.CreateFromYawPitchRoll(middle_yaw, 0, 0);
        }

        /// <summary>
        /// Recalculates this item's length and adjusts properties based on it.
        /// </summary>
        public virtual void Recalculate()
        {
            if (Node is null) return;

            HermiteSpline.ApproximateLength(
                Node.Position, Node.Rotation.ToEuler(), 
                ForwardNode.Position, ForwardNode.Rotation.ToEuler());
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new[] { Node, ForwardNode };
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            if (allNodes.TryGetValue(ForwardNode.Uid, out var resolvedFw))
            {
                ForwardNode = resolvedFw;
            }
            if (allNodes.TryGetValue(Node.Uid, out var resolvedBw))
            {
                Node = resolvedBw;
            }
        }
    }
}
