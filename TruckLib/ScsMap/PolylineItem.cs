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
    public abstract class PolylineItem : MapItem
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
             * p2 = the new node that is being created
             * p1 = the current forward node
             * p0 = the current backward node (= Node)
             *
             *  x--------------x------------x
             *  p0             p1           p2  <- we're adding this node
             *  Node        ForwardNode     new node
             *
             */
            var p1 = ForwardNode;
            var p0 = Node;

            if (ForwardItem != null) // there's already an item attached
            {
                throw new ArgumentOutOfRangeException("Can't append item: ForwardItem is not null");
            }

            var p2 = p1.Sectors[0].Map.AddNode(position, false);
            var newItem = new T
            {
                Node = p1,
                ForwardNode = p2
            };
            p1.ForwardItem = newItem;
            p2.BackwardItem = newItem;

            // Rotation:
            // =========
            //
            // for every item that is added to the chain,
            // 1) the rotation of p2 is calculated the same way as in Add.
            //    the rotation of the nodes is the angle formed by
            //    p2-p1 and the Z axis.
            // 2) the rotation of the node p1
            //      a) is set to (rot0 + rot2)/2 if there are only 
            //         3 nodes in the chain;
            //      b) is set to god knows what otherwise.
            //         it must be the derivative of the spline at that point, but 
            //         I don't even know which spline function the game uses.
            //
            // basically, this code is just wrong but it'll have to do for now.
            //
            // TL;DR: O MAN i don't know how to math pls to halp

            SetRotation(p0, p1, p2);

            p1.IsRed = true;
            p1.Sectors[0].MapItems.Add(newItem.Uid, newItem);          

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

            SetRotation(p2, p0, p1);

            p0.Sectors[0].MapItems.Add(newItem.Uid, newItem);

            return newItem;
        }

        internal static void SetRotation(Node p0, Node p1, Node p2)
        {
            var p2_direction = Vector3.Normalize((p2.Position - p1.Position) / 2f);
            p2.Rotation = MathEx.GetNodeRotation(p1.Position, p2.Position);

            var p0_direction = Vector3.Transform(-Vector3.UnitZ, p0.Rotation);
            var old_p1_direction = Vector3.Transform(-Vector3.UnitZ, p1.Rotation);
            Vector3 new_p1_direction;
            double new_p1_angle;

            if (p0.BackwardItem is null) // exactly three items in the chain
            {
                new_p1_direction = (p2_direction + p0_direction) / 2f;
            }
            else // more than 3 items in the chain
            {
                // ?????????
                new_p1_direction = Vector3.Normalize(
                    MathEx.CatmullRomDerivative(0, p0.Position, p1.Position, p2.Position, p2.Position + p2_direction)
                );
            }

            new_p1_angle = MathEx.AngleOffAroundAxis(new_p1_direction, -Vector3.UnitZ, Vector3.UnitY, false);
            p1.Rotation = Quaternion.CreateFromYawPitchRoll((float)new_p1_angle, 0, 0);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            if (allNodes.ContainsKey(ForwardNode.Uid))
            {
                ForwardNode = allNodes[ForwardNode.Uid];
            }
            if (allNodes.ContainsKey(Node.Uid))
            {
                Node = allNodes[Node.Uid];
            }
        }
    }
}
