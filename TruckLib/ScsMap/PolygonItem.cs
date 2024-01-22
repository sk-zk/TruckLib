using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for map items which define a polygonal area.
    /// </summary>
    public abstract class PolygonItem : MultiNodeItem
    {
        protected PolygonItem() : base() { }

        internal PolygonItem(bool initFields) : base(initFields)
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
        /// Appends a node to the item.
        /// </summary>
        /// <param name="position">The position of the new node.</param>
        public void Append(Vector3 position)
        {
            var node = Nodes.First().Sectors[0].Map.AddNode(position);
            node.ForwardItem = this;
            Nodes.Add(node);
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

        /// <summary>
        /// Translates the item to a different location.
        /// </summary>
        /// <param name="translation"></param>
        public override void Translate(Vector3 translation)
        {
            foreach (var node in Nodes)
                node.Move(node.Position + translation);
        }
    }
}
