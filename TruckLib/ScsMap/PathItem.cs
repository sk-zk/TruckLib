using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for map items which define a polyline or spline that is fully contained
    /// in one item rather than being one piece of it.
    /// </summary>
    public abstract class PathItem : MapItem
    {
        /// <summary>
        /// The nodes of the item.
        /// </summary>
        public List<INode> Nodes { get; set; }

        protected PathItem() : base() { }

        internal PathItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Nodes = new List<INode>(2);
        }

        /// <summary>
        /// Base method for adding a new path item to the map.
        /// </summary>
        internal static T Add<T>(IItemContainer map, IList<Vector3> positions) where T : PathItem, new()
        {
            var item = new T();
            item.CreateNodes(map, positions);
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
        /// <param name="map">The map.</param>
        /// <param name="positions">The positions of the nodes.</param>
        protected void CreateNodes(IItemContainer map, IList<Vector3> positions)
        {
            if (Nodes.Count != 0)
            {
                throw new InvalidOperationException("Map item already has nodes.");
            }

            // all nodes have the item as ForwardItem; 
            // how the nodes connect is determined by their position in the list only
            for (int i = 0; i < positions.Count; i++)
            {
                var node = map.AddNode(positions[i]);
                if (i == 0)
                {
                    // one node has to have the red node flag.
                    // without it, the item can't be deleted.
                    node.IsRed = true;
                }
                node.ForwardItem = this;
                Nodes.Add(node);
            }

            SetNodeRotations();
        }

        /// <summary>
        /// Sets the node rotations when creating a path item.
        /// Called by <see cref="CreateNodes">CreateNodes</see>.
        /// </summary>
        protected virtual void SetNodeRotations()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                var p0 = Nodes[Math.Max(0, i - 1)].Position;
                var p1 = Nodes[i].Position;
                var p2 = Nodes[Math.Min(Nodes.Count - 1, i + 1)].Position;
                var p3 = Nodes[Math.Min(Nodes.Count - 1, i + 2)].Position;
                var vec = CatmullRomSpline.Derivative(p0, p1, p2, p3, 0);
                var angle = MathEx.GetNodeAngle(vec);
                angle = MathEx.Mod(angle, Math.Tau);
                Nodes[i].Rotation = Quaternion.CreateFromYawPitchRoll((float)angle, 0, 0);
            }
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes() => new List<INode>(Nodes);

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }
    }
}
