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
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Model;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the model, as defined in <c>/def/world/model.sii</c>.
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

        private const int colorVariantPos = 8;
        private const int colorVariantLength = 4;
        /// <summary>
        /// 1-indexed color variant of the model. Set to 0 if there aren't any.
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

        /// <summary>
        /// Color tint of the terrain.
        /// </summary>
        public Color TerrainColor { get; set; }

        /// <summary>
        /// UV rotation of the terrain texture.
        /// </summary>
        public float TerrainRotation { get; set; }

        private const int lodPos = 4;
        private const int lodLength = 2;
        /// <summary>
        /// LOD setting, if applicable. Must be between 0 and 3. If 0, the LOD of the model
        /// will change dynamically. The other indices, if supported by the model, will keep
        /// the model at one specific LOD.
        /// </summary>
        public byte Lod
        {
            get => (byte)Kdop.Flags.GetBitString(lodPos, lodLength);
            set => Kdop.Flags.SetBitString(lodPos, lodLength,
                Utils.SetIfInRange(value, (byte)0, (byte)3));
        }

        public bool ModelHookups
        {
            get => !Kdop.Flags[3];
            set => Kdop.Flags[3] = !value;
        }

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
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
        /// Gets or sets if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation
        {
            get => Kdop.Flags[12];
            set => Kdop.Flags[12] = value;
        }

        /// <summary>
        /// Gets or sets if collision is enabled.
        /// </summary>
        public bool Collision
        {
            get => !Kdop.Flags[13];
            set => Kdop.Flags[13] = !value;
        }

        /// <summary>
        /// Gets or sets if the item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Kdop.Flags[14];
            set => Kdop.Flags[14] = !value;
        }

        /// <summary>
        /// Gets or sets if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Kdop.Flags[15];
            set => Kdop.Flags[15] = !value;
        }

        /// <summary>
        /// Gets or sets if the terrain part of the model is rendered.
        /// </summary>
        public bool ShowTerrain
        {
            get => !Kdop.Flags[17];
            set => Kdop.Flags[17] = !value;
        }

        public Model() : base() { }

        internal Model(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Scale = new Vector3(1f, 1f, 1f);
            AdditionalParts = [];
            TerrainColor = Color.White;
        }

        /// <summary>
        /// Adds a model to the map.
        /// </summary>
        /// <param name="map">The map the model will be added to.</param>
        /// <param name="name">The name of the model.</param>
        /// <param name="variant">The variant of the model.</param>
        /// <param name="look">The look of the model.</param>
        /// <param name="position">The position of the model.</param>
        /// <returns>The newly created model.</returns>
        public static Model Add(IItemContainer map, Vector3 position, Token name, Token variant, Token look)
        {
            var model = Add<Model>(map, position);

            model.Name = name;
            model.Look = look;
            model.Variant = variant;

            return model;
        }
    }
}
