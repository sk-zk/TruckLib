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
    public abstract class PolygonItem : MultiNodeItem
    {
        protected PolygonItem() : base() { }

        internal PolygonItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Nodes = new List<Node>(2);
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
                throw new ArgumentOutOfRangeException(nameof(n));

            var translation = newPos - Nodes[n].Position;
            MoveRel(translation);
        }

        /// <summary>
        /// Translates the item to a different location.
        /// </summary>
        /// <param name="translation"></param>
        public void MoveRel(Vector3 translation)
        {
            foreach (Node node in Nodes)
                node.Move(node.Position + translation);
        }
    }
}
