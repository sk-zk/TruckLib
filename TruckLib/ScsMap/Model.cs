using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A static model.
    /// </summary>
    public class Model : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.Model;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the model.
        /// </summary>
        public Token Name { get; set; }

        /// <summary>
        /// The model look.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// The model variant.
        /// </summary>
        public Token Variant { get; set; }

        private int colorVariantPos = 8;
        private int colorVariantLength = 4;
        /// <summary>
        /// The color variant.
        /// </summary>
        public Nibble ColorVariant
        {
            get => (Nibble)Kdop.Flags.GetBitString(
                colorVariantPos, colorVariantLength);
            set => Kdop.Flags.SetBitString(
                colorVariantPos, colorVariantLength, (uint)value);           
        }

        /// <summary>
        /// The relative scale of the model.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Unit names of additional parts used by the model.
        /// </summary>
        public List<Token> AdditionalParts { get; set; }

        public Token TerrainMaterial { get; set; }

        public Color TerrainColor { get; set; }

        private int staticLodPos = 4;
        private int staticLodLength = 2;
        /// <summary>
        /// Forces the game to display a lower LOD of the model, 
        /// no matter where the camera is.
        /// </summary>
        public StaticLod StaticLod
        {
            get => (StaticLod)Kdop.Flags.GetBitString(staticLodPos, staticLodLength);
            set => Kdop.Flags.SetBitString(staticLodPos, staticLodLength, (uint)value);
        }

        public bool ModelHookups
        {
            get => !Kdop.Flags[3];
            set => Kdop.Flags[3] = !value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        /// <summary>
        /// Determines if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        /// <summary>
        /// Enables the LHT variant of the model (?).
        /// </summary>
        public bool LeftHandTraffic
        { 
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation
        {
            get => Kdop.Flags[12];
            set => Kdop.Flags[12] = value;
        }

        public bool Collision
        {
            get => !Kdop.Flags[13];
            set => Kdop.Flags[13] = !value;
        }

        /// <summary>
        /// Determines if the item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Kdop.Flags[14];
            set => Kdop.Flags[14] = !value;
        }

        /// <summary>
        /// Determines if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Kdop.Flags[15];
            set => Kdop.Flags[15] = !value;
        }

        public Model() : base() { }

        internal Model(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Scale = new Vector3(1f, 1f, 1f);
            AdditionalParts = new List<Token>();
            TerrainColor = Color.FromArgb(0x00ffffff);
        }

        /// <summary>
        /// Adds a new model to the map. This method will also create a node for the model.
        /// </summary>
        /// <param name="map">The map the model will be added to.</param>
        /// <param name="name">The name of the model.</param>
        /// <param name="variant">The variant of the model.</param>
        /// <param name="look">The look of the model.</param>
        /// <param name="position">The position of the model.</param>
        /// <returns>The newly created model.</returns>
        public static Model Add(IItemContainer map, Vector3 position, Quaternion rotation,
            Token name, Token variant, Token look)
        {
            var model = Add<Model>(map, position);

            model.Node.Rotation = rotation;
            model.Name = name;
            model.Look = look;
            model.Variant = variant;

            return model;
        }

        public static Model Add(IItemContainer map, Vector3 position, float yRotation,
            Token name, Token variant, Token look)
        {
            var rotation = Quaternion.CreateFromYawPitchRoll(yRotation, 0, 0);
            return Add(map, position, rotation, name, variant, look);
        }
    }
}
