using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    public abstract class PathItem : MultiNodeItem
    {
        protected PathItem() : base() { }

        internal PathItem(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Nodes = new List<INode>(2);
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            var translation = newPos - Nodes[0].Position;
            Translate(translation);
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            foreach (var node in Nodes)
                node.Move(node.Position + translation);
        }

    }
}
