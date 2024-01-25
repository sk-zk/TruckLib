using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Base class for map items which define a path that is fully contained in one item rather than
    /// forming a polyline.
    /// </summary>
    public abstract class PathItem : MultiNodeItem
    {
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

        /// <inheritdoc/>
        protected override void CreateNodes(IItemContainer map, IList<Vector3> nodePositions)
        {
            base.CreateNodes(map, nodePositions);
            for (int i = 0; i < Nodes.Count; i++)
            {
                var p0 = nodePositions[Math.Max(0, i - 1)];
                var p1 = nodePositions[i];
                var p2 = nodePositions[Math.Min(nodePositions.Count - 1, i + 1)];
                var p3 = nodePositions[Math.Min(nodePositions.Count - 1, i + 2)];
                var vec = CatmullRomSpline.Derivative(p0, p1, p2, p3, 0);
                var angle = MathEx.GetNodeAngle(vec);
                Nodes[i].Rotation = Quaternion.CreateFromYawPitchRoll((float)angle, 0, 0); ;
            }
        }
    }
}