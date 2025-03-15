using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            newItem.Length = (backwardPos - forwardPos).Length();

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
            newItem.RecalculateLength();

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

            newItem.Recalculate();

            p1.Parent.MapItems.Add(newItem.Uid, newItem);

            return newItem;
        }

        private static void SetMiddleRotation(INode left, INode middle, INode right)
        {
            middle.Rotation = Quaternion.Slerp(
                MathEx.GetNodeRotation(left.Position, middle.Position), 
                right.Rotation, 
                0.5f);
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

        /// <summary>
        /// Creates map items at equidistant intervals along the path of a chain of polyline items. 
        /// </summary>
        /// <param name="start">The polyline item (inclusive) where items will begin to be placed.</param>
        /// <param name="end">The polyline item (exclusive) where items will stop being placed.
        /// If null, the method will stop at the last polyline item connected to <paramref name="start"/>.</param>
        /// <param name="interval">The interval between items, in meters.</param>
        /// <param name="createItemFunction">The function which creates the map item 
        /// given an <see cref="OrientedPoint"/>. The return value is a list or array 
        /// of the created items.</param>
        /// <returns>A list of all map items which were created.</returns>
        public static List<MapItem> CreateItemsAlongPath(PolylineItem start, PolylineItem end,
            float interval, Func<IItemContainer, OrientedPoint, IList<MapItem>> createItemFunction)
        {
            return CreateItemsAlongPath(start, end, interval, 0, 0, createItemFunction);
        }

        /// <summary>
        /// Creates map items at equidistant intervals along the path of a chain of polyline items. 
        /// </summary>
        /// <param name="start">The polyline item (inclusive) where items will begin to be placed.</param>
        /// <param name="end">The polyline item (exclusive) where items will stop being placed.
        /// If null, the method will stop at the last polyline item connected to <paramref name="start"/>.</param>
        /// <param name="interval">The interval between items, in meters.</param>
        /// <param name="startOffset">The distance from the starting point where
        /// points will begin to be generated.</param>
        /// <param name="endOffset">The distance from the ending point where
        /// points will cease to be generated.</param>
        /// <param name="createItemFunction">The function which creates the map item 
        /// given an <see cref="OrientedPoint"/>. The return value is a list or array 
        /// of the created items.</param>
        /// <returns>A list of all map items which were created.</returns>
        public static List<MapItem> CreateItemsAlongPath(PolylineItem start, PolylineItem end,
            float interval, float startOffset, float endOffset,
            Func<IItemContainer, OrientedPoint, IList<MapItem>> createItemFunction)
        {
            if (start is null)
                throw new ArgumentException("The starting item must not be null.", nameof(start));

            if (interval <= 0)
                throw new ArgumentOutOfRangeException(nameof(interval),
                    "The interval must be greater than 0.");

            if (startOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(startOffset),
                    "The start offset must not be below 0.");

            if (endOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(startOffset),
                    "The end offset must not be below 0.");

            var container = start.Node.Parent;
            var items = new List<MapItem>();

            PolylineItem currentItem = start;
            float dist = -(startOffset - interval);
            while (currentItem is not null && currentItem != end)
            {
                if (dist + currentItem.Length < interval)
                {
                    dist += currentItem.Length;
                }
                else
                {
                    bool isFinalItem = currentItem.ForwardItem is not PolylineItem
                        || currentItem.ForwardItem == end;
                    float currentEndOffset = isFinalItem ? endOffset : 0;

                    float currentOffset = Math.Max(0, interval - dist);

                    var points = HermiteSpline.GetEquidistantPoints(currentItem.Node,
                        currentItem.ForwardNode, interval, currentOffset, currentEndOffset);

                    if (points.Count == 0)
                    {
                        dist += currentItem.Length;
                    }
                    else
                    {
                        foreach (var point in points)
                        {
                            items.AddRange(createItemFunction(container, point));
                        }

                        dist = (currentItem.ForwardNode.Position - points[^1].Position).Length();
                    }
                }

                currentItem = currentItem.ForwardItem is PolylineItem pli ? pli : null;
            }

            return items;
        }

        /// <summary>
        /// Interpolates the spline between the two nodes at <em>t</em>.
        /// </summary>
        /// <param name="t">The interpolation parameter between 0 and 1.</param>
        /// <returns>The position and rotation of the point at <em>t</em>.</returns>
        public OrientedPoint InterpolateCurve(float t)
        {
            var position = HermiteSpline.InterpolatePolyline(Node, ForwardNode, t);
            var rotation = MathEx.GetNodeRotation(HermiteSpline.DerivativePolyline(Node, ForwardNode, t));
            return new OrientedPoint(position, rotation);
        }

        /// <summary>
        /// Calculates the point on the spline which is <em>n</em> meters
        /// away from the <see cref="Node">backward node</see>.
        /// </summary>
        /// <param name="n">The distance in meters.</param>
        /// <returns>The position and rotation of the point, 
        /// or null if the curve is shorter than <em>n</em>.</returns>
        public OrientedPoint? InterpolateCurveDist(float n)
        {
            var list = HermiteSpline.GetSpacedPoints(Node, ForwardNode, [n], n, repeat: false);
            return list.Count == 0 ? null : list[0];
        }

        internal override IEnumerable<INode> GetItemNodes() => 
            [Node, ForwardNode];

        internal override Vector3 GetCenter() => 
            (Node.Position + ForwardNode.Position) / 2;

        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            Node = ResolveNodeReference(Node, allNodes);
            ForwardNode = ResolveNodeReference(ForwardNode, allNodes);
        }
    }
}
