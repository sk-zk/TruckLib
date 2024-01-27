using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Defines a point where a random vehicle model is placed at runtime.
    /// </summary>
    public class Hookup : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Hookup;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Name of the model pool.
        /// </summary>
        public string Name { get; set; }

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
        /// Gets or sets if the item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Kdop.Flags[2];
            set => Kdop.Flags[2] = !value;
        }

        /// <summary>
        /// Gets or sets if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Kdop.Flags[3];
            set => Kdop.Flags[3] = !value;
        }

        /// <summary>
        /// Gets or sets if physics are enabled for this item.
        /// </summary>
        public bool Physics
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        /// <summary>
        /// Gets or sets if the seed for the RNG is based on the item's position, meaning that
        /// the chosen model therefore only changes if the item is moved or the model pool changes.
        /// </summary>
        public bool FixedSeed
        {
            get => !Kdop.Flags[6];
            set => Kdop.Flags[6] = !value;
        }

        private const int NodeAlignmentStart = 8;
        private const int NodeAlignmentLength = 2;
        /// <summary>
        /// Determines where the model is placed relative to the item's node.
        /// </summary>
        public HookupNodeAlignment NodeAlignment
        {
            get => (HookupNodeAlignment)Kdop.Flags.GetBitString(NodeAlignmentStart, NodeAlignmentLength);
            set => Kdop.Flags.SetBitString(NodeAlignmentStart, NodeAlignmentLength, (uint)value);
        }

        private const int SpawnProbabilityStart = 12;
        private const int SpawnProbabilityLength = 2;
        /// <summary>
        /// Tthe likelihood in percent that a model will be spawned.
        /// </summary>
        public HookupSpawnProbability SpawnProbability
        {
            get => (HookupSpawnProbability)Kdop.Flags.GetBitString(SpawnProbabilityStart, SpawnProbabilityLength);
            set => Kdop.Flags.SetBitString(SpawnProbabilityStart, SpawnProbabilityLength, (uint)value);
        }

        private const int ModelDetailStart = 16;
        private const int ModelDetailLength = 2;
        /// <summary>
        /// The LOD of the model.
        /// </summary>
        public HookupModelDetail ModelDetail
        {
            get => (HookupModelDetail)Kdop.Flags.GetBitString(ModelDetailStart, ModelDetailLength);
            set => Kdop.Flags.SetBitString(ModelDetailStart, ModelDetailLength, (uint)value);
        }

        public Hookup() : base() { }

        internal Hookup(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Adds a hookup to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the hookup.</param>
        /// <param name="name">The name of the model pool.</param>
        /// <returns>The newly created hookup.</returns>
        public static Hookup Add(IItemContainer map, Vector3 position, string name)
        {
            var hookup = Add<Hookup>(map, position);

            hookup.Name = name;

            return hookup;
        }
    }
}
