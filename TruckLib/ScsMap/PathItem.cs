using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for map items which define a polyline or spline that is fully contained
    /// in one item rather than being one piece of it.
    /// </summary>
    public abstract class PathItem : MapItem, IRecalculatable
    {
        /// <summary>
        /// The nodes of the item.
        /// </summary>
        public PathNodeList Nodes { get; set; }

        internal IItemContainer Parent { get; set; }

        protected PathItem() : base() { }

        internal PathItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Nodes = new PathNodeList(this);
        }

        /// <summary>
        /// Base method for adding a new path item to the map.
        /// </summary>
        internal static T Add<T>(IItemContainer map, IList<Vector3> positions) where T : PathItem, new()
        {
            var item = new T();
            item.Parent = map;
            item.CreateNodes(positions);
            map.AddItem(item);
            return item;
        }

        /// <summary>
        /// Moves the item, where the given position will be the new position
        /// of the node at index 0, and all other nodes will be moved relative to it.
        /// </summary>
        /// <param name="newPos">The new position of the node.</param>
        public override void Move(Vector3 newPos)
        {
            var translation = newPos - Nodes[0].Position;
            Translate(translation);
        }

        /// <summary>
        /// Moves the item, where the given position will be the new position
        /// of the node at index <paramref name="n">n/</paramref>, and all other nodes
        /// will be moved relative to it.
        /// </summary>
        /// <param name="newPos">The new position of the node.</param>
        /// <param name="n">The index of the node which will be moved to this position.</param>
        public void Move(Vector3 newPos, int n)
        {
            if (n < 0 || n >= Nodes.Count)
                throw new ArgumentOutOfRangeException(nameof(n));

            var translation = newPos - Nodes[n].Position;
            Translate(translation);
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            foreach (var node in Nodes)
                node.Move(node.Position + translation);
        }

        /// <summary>
        /// Creates map nodes for this item. 
        /// </summary>
        /// <param name="positions">The positions of the nodes.</param>
        protected void CreateNodes(IList<Vector3> positions)
        {
            if (Nodes.Count != 0)
                throw new InvalidOperationException("Map item already has nodes.");

            for (int i = 0; i < positions.Count; i++)
                Nodes.Add(positions[i]);

            Recalculate();
        }

        /// <inheritdoc/>
        public virtual void Recalculate()
        {
            RecalculateRotation();
        }

        /// <summary>
        /// Sets the node rotations when creating a path item.
        /// Called by <see cref="CreateNodes">CreateNodes</see>.
        /// </summary>
        private void RecalculateRotation()
        {
            for (int i = 0; i < Nodes.Count; i++)
                RecalculateRotation(i);
        }

        /// <summary>
        /// Recalculates the rotation of one node.
        /// </summary>
        /// <param name="i">The index of the node.</param>
        protected virtual void RecalculateRotation(int i)
        {
            var p0 = Nodes[Math.Max(0, i - 1)].Position;
            var p1 = Nodes[i].Position;
            var p2 = Nodes[Math.Min(Nodes.Count - 1, i + 1)].Position;
            var p3 = Nodes[Math.Min(Nodes.Count - 1, i + 2)].Position;
            var vec = CatmullRomSpline.Derivative(p0, p1, p2, p3, 0);
            Nodes[i].Rotation = MathEx.GetNodeRotation(vec);
        }

        /// <summary>
        /// Recalculates node i as well as i-1 and i+1 if they exist.
        /// </summary>
        /// <param name="i">The index of the central node.</param>
        internal virtual void RecalculateAdjacent(int i)
        {
            if (i > 0)
                RecalculateRotation(i - 1);

            RecalculateRotation(i);

            if (i < Nodes.Count - 1)
                RecalculateRotation(i + 1);
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes() => new List<INode>(Nodes);

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
                Nodes.SetWithoutRecalculating(i, ResolveNodeReference(Nodes[i], allNodes));
        }
    }
}
