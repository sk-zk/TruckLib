using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    // TODO: Write xmldoc.
    public class Curve : PolylineItem
    {
        public override ItemType ItemType => ItemType.Curve;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public float MinLength => 0f;

        public float MaxLength => 1000;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Token Model { get; set; }

        public Token Look { get; set; }

        public Token FirstPart { get; set; }

        public Token CenterPartVariation { get; set; }

        public Token LastPart { get; set; }

        public Token TerrainMaterial { get; set; }

        public Color TerrainColor { get; set; }

        public uint RandomSeed { get; set; }

        public float Stretch { get; set; }

        /// <summary>
        /// If not 0, forces the model to repeat every n meters, overriding the default. 
        /// </summary>
        public float FixedStep { get; set; }

        public float Scale { get; set; }

        /// <summary>
        /// UV rotation of terrain textures.
        /// </summary>
        public float TerrainRotation { get; set; }

        /// <summary>
        /// Height offsets for individual elements of the curve.
        /// </summary>
        public List<float> HeightOffsets { get; set; }

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
        /// Inverts the model by 180 degrees.
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
        /// Makes the model follow a linear path rather than a spline.
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

        protected override void Init()
        {
            base.Init();
            Look = "default";
            HeightOffsets = new List<float>();
            Stretch = 1f;
            Scale = 1f;
            TerrainColor = Color.White;
        }

        public static Curve Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token model)
        {
            var curve = Add<Curve>(map, backwardPos, forwardPos);

            curve.InitFromAddOrAppend(backwardPos, forwardPos, model);

            return curve;
        }

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

        private void CopySettingsTo(Curve c)
        {
            c.Kdop.Flags = Kdop.Flags;
            c.FirstPart = FirstPart;
            c.CenterPartVariation = CenterPartVariation;
            c.LastPart = LastPart;
            c.Look = Look;
            c.TerrainMaterial = TerrainMaterial;
            c.TerrainColor = TerrainColor;
            c.ViewDistance = ViewDistance;
            c.RandomSeed = RandomSeed;
            c.Stretch = Stretch;
            c.FixedStep = FixedStep;
            c.HeightOffsets = new List<float>(HeightOffsets);
        }
    }
}
