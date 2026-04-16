using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap.Collections;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Repeats models along a path.
    /// </summary>
    public class Curve : PolylineItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Curve;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Minimum length of a Curve item.
        /// </summary>
        public const float MinLength = 0f;

        /// <summary>
        /// Maximum length of a Curve item.
        /// </summary>
        public const float MaxLength = 1000;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The helper nodes placed for the corresponding locators
        /// of the curve model.
        /// </summary>
        public CurveLocatorList Locators { get; set; }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        /// <summary>
        /// Gets or sets if collision is enabled.
        /// </summary>
        public bool Collision
        {
            get => !Kdop.Flags[2];
            set => Kdop.Flags[2] = !value;
        }

        /// <summary>
        /// Gets or sets if the item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        /// <summary>
        /// Gets or sets if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Kdop.Flags[7];
            set => Kdop.Flags[7] = !value;
        }

        /// <summary>
        /// 1-indexed color variant of the models, shared across all subcurves.
        /// Set to 0 if there aren't any.
        /// </summary>
        public Nibble ColorVariant
        {
            get => (Nibble)Kdop.Flags.GetBitString(16, 4);
            set => Kdop.Flags.SetBitString(16, 4, (uint)value);
        }

        /// <summary>
        /// Forces the use of low LOD versions of models.
        /// </summary>
        public bool LowDetail
        {
            get => Kdop.Flags[20];
            set => Kdop.Flags[20] = value;
        }

        /// <summary>
        /// Makes the model follow a linear path from backward to forward node
        /// rather than the curved path created by the spline.
        /// </summary>
        public bool UseLinearPath
        {
            get => Kdop.Flags[21];
            set => Kdop.Flags[21] = value;
        }

        /// <summary>
        /// Gets or sets if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation
        {
            get => Kdop.Flags[22];
            set => Kdop.Flags[22] = value;
        }

        internal const int SubcurveCount = 4;

        /// <summary>
        /// The subcurves of this item.
        /// </summary>
        public Subcurve[] Subcurves { get; internal set; }

        public Curve() : base() { }

        internal Curve(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Subcurves = new Subcurve[SubcurveCount];
            Subcurves[0] = new();
            Locators = new CurveLocatorList(this);
        }

        /// <summary>
        /// Adds a curve to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="backwardPos">Position of the backward (red) node.</param>
        /// <param name="forwardPos">Position of the forward (green) node.</param>
        /// <param name="model">Unit name of the curve model.</param>
        /// <returns>The newly created curve.</returns>
        public static Curve Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token model)
        {
            var curve = Add<Curve>(map, backwardPos, forwardPos);
            curve.Subcurves[0].Model = model;
            return curve;
        }

        /// <summary>
        /// Appends a curve segment to the end of this one.
        /// </summary>
        /// <param name="position">The position of the <see cref="PolylineItem.ForwardNode">ForwardNode</see>
        /// of the new curve segment.</param>
        /// <param name="cloneSettings">Whether the new segment should have the
        /// same settings as this one. If false, the defaults will be used.</param>
        /// <returns>The newly created curve segment.</returns>
        public Curve Append(Vector3 position, bool cloneSettings = true)
        {
            if (!cloneSettings)
            {
                return Append(position, Subcurves[0].Model);
            }

            var b = Append(position, Subcurves[0].Model);
            CopySettingsTo(b);
            return b;
        }

        /// <summary>
        /// Appends a curve segment to the end of this one.
        /// </summary>
        /// <param name="position">The position of the <see cref="PolylineItem.ForwardNode">ForwardNode</see>
        /// of the new curve segment.</param>
        /// <param name="model">Unit name of the curve model.</param>
        /// <returns>The newly created curve.</returns>
        public Curve Append(Vector3 position, Token model)
        {
            var curve = Append<Curve>(position);
            curve.Subcurves[0].Model = model;
            return curve;
        }

        /// <inheritdoc/>
        public override void Move(Vector3 newPos)
        {
            var offset = newPos - Node.Position;
            base.Move(newPos);
            foreach (var node in Locators)
            {
                node.Translate(offset);
            }
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);
            foreach (var node in Locators)
            {
                node.Translate(translation);
            }
        }

        private void CopySettingsTo(Curve c)
        {
            c.Kdop.Flags = Kdop.Flags;
            c.ViewDistance = ViewDistance;

            for (int i = 0; i < SubcurveCount; i++)
            {
                c.Subcurves[i] = Subcurves[i]?.Clone();
            }
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes()
        {
            var list = new List<INode>(Locators.Count + 2)
            {
                Node,
                ForwardNode,
            };
            list.AddRange(Locators);
            return list;
        }

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(IDictionary<ulong, INode> allNodes)
        {
            base.UpdateNodeReferences(allNodes);
            ResolveNodeReferences(Locators, allNodes);
        }
    }
}
