using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Defines a path creating one or more planes perpendicular to the ground plane.
    /// Items which are fully behind these planes from the point of view of the camera
    /// are not rendered.
    /// </summary>
    public class CutPlane : PathItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.CutPlane;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Gets or sets if the cut plane is only active when looking at it from the back.
        /// </summary>
        public bool OneSideOnly
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool RotatedLimits
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public CutPlaneNodeAngle StartNodeAngle
        {
            get => Kdop.Flags.GetByte(2);
            set => Kdop.Flags.SetByte(2, (byte)value.InternalValue);
        }

        public CutPlaneNodeAngle EndNodeAngle
        {
            get => Kdop.Flags.GetByte(3);
            set => Kdop.Flags.SetByte(3, (byte)value.InternalValue);
        }

        public CutPlane() : base() { }

        internal CutPlane(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Adds a cut plane to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="positions">Positions of the nodes of the cut plane.</param>
        /// <param name="oneSideOnly">Whether the cut plane is only active when looking at
        /// it from the back.</param>
        /// <returns>The newly created cut plane.</returns>
        public static CutPlane Add(IItemContainer map, IList<Vector3> positions,
            bool oneSideOnly = false)
        {
            var cutPlane = Add<CutPlane>(map, positions);

            cutPlane.OneSideOnly = oneSideOnly;

            return cutPlane;
        }

        /// <summary>
        /// Appends a new node to the cut plane.
        /// </summary>
        /// <param name="position">The position of the new node.</param>
        public void Append(Vector3 position)
        {
            var node = Nodes[0].Sectors[0].Map.AddNode(position);
            node.ForwardItem = this;
            Nodes.Add(node);
        }

        /// <inheritdoc/>
        internal override INode GetMainNode() => Nodes[^1];

        /// <inheritdoc/>
        protected override void SetNodeRotations()
        {
            return; // Do nothing - cut plane nodes are 0y, 0p, 0r by default
        }
    }
}
