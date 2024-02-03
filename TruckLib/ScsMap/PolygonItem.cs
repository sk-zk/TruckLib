using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for map items which define a polygonal area.
    /// </summary>
    public abstract class PolygonItem : MapItem
    {
        /// <summary>
        /// The nodes of the item.
        /// </summary>
        public PolygonNodeList Nodes { get; set; }

        internal IItemContainer Parent { get; set; }

        protected PolygonItem() : base() { }

        internal PolygonItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Nodes = new PolygonNodeList(this);
        }

        /// <summary>
        /// Base method for adding a new PolygonItem to the map.
        /// </summary>
        internal static T Add<T>(IItemContainer map, IList<Vector3> positions) where T : PolygonItem, new()
        {
            var item = new T();
            item.Parent = map;
            foreach (var position in positions)
            {
                item.Nodes.Add(position);
            }
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
            Move(newPos, 0);
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

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes()
        {
            return Nodes;
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }
    }
}
