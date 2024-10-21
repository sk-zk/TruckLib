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
    /// A curve segment, which repeats one or more models along a path.
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
        public float MinLength => 0f;

        /// <summary>
        /// Maximum length of a Curve item.
        /// </summary>
        public float MaxLength => 1000;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// Unit name of the curve model, as defined in <c>/def/world/curve_model.sii</c>.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// Unit name of the model look.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// Unit name of the first part, if applicable.
        /// </summary>
        public Token FirstPart { get; set; }

        /// <summary>
        /// Unit name of the center part variation.
        /// </summary>
        public Token CenterPartVariation { get; set; }

        /// <summary>
        /// Unit name of the last part, if applicable.
        /// </summary>
        public Token LastPart { get; set; }

        /// <summary>
        /// Unit name of the terrain material, if applicable.
        /// </summary>
        public Token TerrainMaterial { get; set; }

        /// <summary>
        /// Color tint of the terrain.
        /// </summary>
        public Color TerrainColor { get; set; }

        public uint RandomSeed { get; set; }

        /// <summary>
        /// Coefficient for stretching the scheme along the path. For some curves,
        /// this stretches the model; for others, it places its elements further apart.
        /// </summary>
        public float Stretch { get; set; }

        /// <summary>
        /// If not 0, the model will repeat every <i>n</i> meters, overriding the default.
        /// </summary>
        public float FixedStep { get; set; }

        /// <summary>
        /// Relative scale of the model.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// UV rotation of the terrain texture.
        /// </summary>
        public float TerrainRotation { get; set; }

        /// <summary>
        /// <para>Height offsets for individual elements of the curve.</para>
        /// <para>Offsets are applied to elements in order. For instance, if you want
        /// the third element to have an offset of 5, the content of the list should be
        /// [0, 0, 5].</para>
        /// </summary>
        public List<float> HeightOffsets { get; set; }

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
        /// Flips the model on the axis formed by the curve's two nodes.
        /// </summary>
        public bool InvertGeometry
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        }

        /// <summary>
        /// Gets or sets if elements of a sloped item are rendered like stair steps
        /// rather than being stretched along the path.
        /// </summary>
        public bool SteppedGeometry
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        /// <summary>
        /// Gets or sets if vertices of the model are covered in a random tint noise.<br />
        /// That's what the editor tooltip says, anyway. I don't really see any difference.
        /// </summary>
        public bool PerlinNoise
        {
            get => Kdop.Flags[5];
            set => Kdop.Flags[5] = value;
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
        /// 1-indexed color variant of the model. Set to 0 if there aren't any.
        /// </summary>
        public Nibble ColorVariant
        {
            get => (Nibble)Kdop.Flags.GetBitString(16, 4);
            set => Kdop.Flags.SetBitString(16, 4, (uint)value);
        }

        /// <summary>
        /// Gets or sets if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation
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

        public Curve() : base() { }

        internal Curve(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Look = "default";
            HeightOffsets = [];
            Stretch = 1f;
            Scale = 1f;
            TerrainColor = Color.White;
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

            curve.InitFromAddOrAppend(backwardPos, forwardPos, model);

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
                return Append(position, Model);
            }

            var b = Append(position, Model);
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
            curve.InitFromAddOrAppend(ForwardNode.Position, position, model);
            return curve;
        }

        private void InitFromAddOrAppend(Vector3 backwardPos, Vector3 forwardPos, Token model)
        {
            Model = model;
            Length = Vector3.Distance(backwardPos, forwardPos);
        }

        public override void Move(Vector3 newPos)
        {
            var offset = newPos - Node.Position;
            base.Move(newPos);
            foreach (var node in Locators)
            {
                node.Translate(offset);
            }
        }

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
            c.FirstPart = FirstPart;
            c.CenterPartVariation = CenterPartVariation;
            c.LastPart = LastPart;
            c.Look = Look;
            c.TerrainMaterial = TerrainMaterial;
            c.TerrainColor = TerrainColor;
            c.TerrainRotation = TerrainRotation;
            c.ViewDistance = ViewDistance;
            c.RandomSeed = RandomSeed;
            c.Stretch = Stretch;
            c.FixedStep = FixedStep;
            c.HeightOffsets = new List<float>(HeightOffsets);
        }

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
    }
}
