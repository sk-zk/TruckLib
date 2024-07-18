﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for map items which form a polyline with other 
    /// items of the same type, such as <see cref="Road">roads</see> and
    /// <see cref="Buildings">buildings</see>.
    /// </summary>
    public abstract class PolylineItem : MapItem, IRecalculatable
    {
        /// <summary>
        /// The item attached to this item in backward direction.
        /// </summary>
        public IMapItem BackwardItem => (IMapItem)Node.BackwardItem;

        /// <summary>
        /// The item attached to this item in forward direction.
        /// </summary>
        public IMapItem ForwardItem => (IMapItem)ForwardNode.ForwardItem;

        /// <summary>
        /// The backward node / the node which holds this item as ForwardItem
        /// (= the node which highlights the item if you mouse over it).
        /// </summary>
        public INode Node { get; set; }

        /// <summary>
        /// The forward node (= the one which is green on a single segment of this item).
        /// </summary>
        public INode ForwardNode { get; set; }

        /// <summary>
        /// Cached length of the item's path.
        /// </summary>
        public float Length { get; set; }

        public PolylineItem() : base() { }

        internal PolylineItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Returns the first item of the polyline chain this item is a part of.
        /// </summary>
        /// <returns>The first item of the polyline chain this item is a part of.</returns>
        public PolylineItem FindFirstItem()
        {
            var first = this;
            while (first.BackwardItem is PolylineItem pli)
                first = pli;
            return first;
        }

        /// <summary>
        /// Returns the last item of the polyline chain this item is a part of.
        /// </summary>
        /// <returns>The last item of the polyline chain this item is a part of.</returns>
        public PolylineItem FindLastItem()
        {
            var last = this;
            while (last.ForwardItem is PolylineItem pli)
                last = pli;
            return last;
        }

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

            map.AddItem(newItem);
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
             *   x--------------x------------x
             *   p1             p2           p3   <- we're adding this node
             *  Node       ForwardNode    new node
             *
             */
            var p1 = Node;
            var p2 = ForwardNode;

            if (ForwardItem != null) // there's already an item attached
                throw new InvalidOperationException("Can't append item: ForwardItem is not null");

            var p3 = p2.Parent.AddNode(position, false);
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
            p3.Parent.MapItems.Add(newItem.Uid, newItem);

            RecalculateLength();

            return newItem;
        }

        internal T Prepend<T>(Vector3 position) where T : PolylineItem, new()
        {
            /* for readability:
             * p0 = the new node that is being created
             * p1 = the current backward node (= Node)
             * p2 = the current forward node
             *
             *   x--------------x------------x
             *   p0             p1           p2 
             *  new node       Node       ForwardNode
             *  ^ we're adding this one
             */
            var p1 = Node;
            var p2 = ForwardNode;

            if (BackwardItem != null) // there's already an item attached
                throw new InvalidOperationException("Can't prepend item: BackwardItem is not null");

            var p0 = p1.Parent.AddNode(position, true);
            var newItem = new T
            {
                Node = p0,
                ForwardNode = p1
            };
            p0.ForwardItem = newItem;
            p1.BackwardItem = newItem;

            newItem.RecalculateRotation();

            p1.Parent.MapItems.Add(newItem.Uid, newItem);

            RecalculateLength();

            return newItem;
        }

        private static void SetMiddleRotation(INode pBackw, INode pMiddle, INode pNew)
        {
            var pNewRot = pNew.Rotation.ToEuler();

            var backMiddleDirRot = MathEx.GetNodeRotation(pBackw.Position, pMiddle.Position).ToEuler();
            var middleYaw = (pNewRot.Y + backMiddleDirRot.Y) / 2;
            var middlePitch = (pNewRot.X + backMiddleDirRot.X) / 2;
            pMiddle.Rotation = Quaternion.CreateFromYawPitchRoll(middleYaw, middlePitch, 0);
        }

        /// <summary>
        /// Recalculates this item's length and adjusts properties based on it.
        /// </summary>
        public virtual void Recalculate()
        {
            if (Node is null) 
                return;

            RecalculateRotation();
            RecalculateLength();
        }

        internal void RecalculateLength()
        {
            if (Node is null)
                return;

            Length = PolylineLength.Calculate(Node, ForwardNode);
        }

        internal void RecalculateRotation()
        {
            if (Node is null)
                return;

            // just redo everything from the start,
            // as if you'd just added all the segments from start to end.
            // it's dumb, but it works

            var first = FindFirstItem();
            var initialRot = MathEx.GetNodeRotation(first.Node.Position, first.ForwardNode.Position);

            if (first.BackwardItem is not Prefab)
                first.Node.Rotation = initialRot;
            if (first.ForwardItem is not Prefab)
                first.ForwardNode.Rotation = initialRot;

            var next = first;
            while (next.ForwardItem is PolylineItem fw)
            {
                var p1 = next.Node;
                var p2 = next.ForwardNode;
                var p3 = fw.ForwardNode;

                if (p3.ForwardItem is not Prefab)
                    p3.Rotation = MathEx.GetNodeRotation(p2.Position, p3.Position);
                SetMiddleRotation(p1, p2, p3);
                next = fw;
            }
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            var fwNodeOffset = ForwardNode.Position - Node.Position;
            Node.Move(newPos);
            ForwardNode.Move(newPos + fwNodeOffset);
            Recalculate();
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            Node.Move(Node.Position + translation);
            ForwardNode.Move(ForwardNode.Position + translation);
            Recalculate();
        }

        internal override IEnumerable<INode> GetItemNodes() =>
            new[] { Node, ForwardNode };

        internal override Vector3 GetCenter() => 
            (Node.Position + ForwardNode.Position) / 2;

        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            Node = ResolveNodeReference(Node, allNodes);
            ForwardNode = ResolveNodeReference(ForwardNode, allNodes);
        }
    }
}
