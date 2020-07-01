using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    public abstract class PathItem : MapItem
    {
        public List<Node> Nodes { get; set; }

        protected PathItem() : base() { }

        internal PathItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Nodes = new List<Node>(2);
        }

        /// <summary>
        /// Moves the item to a different location.
        /// </summary>
        /// <param name="newPos"></param>
        public void Move(Vector3 newPos)
        {
            var translation = newPos - Nodes[0].Position;
            MoveRel(translation);
        }

        /// <summary>
        /// Translates the item to a different location.
        /// </summary>
        /// <param name="translation"></param>
        public void MoveRel(Vector3 translation)
        {
            foreach (var node in Nodes)
                node.Move(node.Position + translation);
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new List<Node>(Nodes);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode
                    && allNodes.TryGetValue(Nodes[i].Uid, out var resolvedNode))
                {
                    Nodes[i] = resolvedNode;
                }
            }
        }
    }
}
