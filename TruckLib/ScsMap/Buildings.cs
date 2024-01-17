using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A Buildings segment.
    /// </summary>
    public class Buildings : PolylineItem
    {
        public override ItemType ItemType => ItemType.Buildings;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

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
        /// The unit name of the building.
        /// </summary>
        public Token Name { get; set; }

        /// <summary>
        /// The building look.
        /// </summary>
        public Token Look { get; set; }

        public byte DlcGuard
        {
            get => Kdop.Flags.GetByte(1);
            set => Kdop.Flags.SetByte(1, value);
        }

        /// <summary>
        /// 1-indexed color variant of the model. Set to 0 if there aren't any.
        /// </summary>
        public byte ColorVariant
        {
            get => Kdop.Flags.GetByte(2);
            set => Kdop.Flags.SetByte(2, value);
        }

        /// <summary>
        /// Gets or sets the seed for the RNG which determines which vegetation models
        /// to place. The position of the models does not appear to be
        /// affected by this.
        /// </summary>
        public uint RandomSeed { get; set; }

        /// <summary>
        /// Coefficient for stretching the scheme along the path. For some buildings,
        /// this stretches the model; for others, it places the elements further apart.
        /// </summary>
        public float Stretch { get; set; }

        /// <summary>
        /// <para>Height offsets for individual elements of the building.</para>
        /// <para>Offsets are applied to elements in order. For instance, if you want
        /// the third element to have an offset of 5, the content of the list should be
        /// [0, 0, 5].</para>
        /// </summary>
        public List<float> HeightOffsets { get; set; }

        /// <summary>
        /// Gets or sets if collision is enabled.
        /// </summary>
        public bool Collision
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
        /// Gets or sets if accessories are enabled.
        /// </summary>
        public bool Accessories
        {
            get => !Kdop.Flags[0];
            set => Kdop.Flags[0] = !value;
        }

        /// <summary>
        /// Gets or sets if the item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Kdop.Flags[4];
            set => Kdop.Flags[4] = !value;
        }

        /// <summary>
        /// Gets or sets if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        public Buildings() : base() { }

        internal Buildings(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            RandomSeed = 1;
            Stretch = 1;
            HeightOffsets = new List<float>();
        }

        /// <summary>
        /// Adds a building segment to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="name">Unit name of the building.</param>
        /// <param name="backwardPos">The position of the backward (red) node.</param>
        /// <param name="forwardPos">The position of the forward (green) node.</param>
        /// <returns>The newly created building segment.</returns>
        public static Buildings Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token name)
        {
            var building = Add<Buildings>(map, backwardPos, forwardPos);
            building.InitFromAddOrAppend(name, backwardPos, forwardPos);
            return building;
        }

        /// <summary>
        /// Initializes a building segment which has been created via Add or Append.
        /// </summary>
        private void InitFromAddOrAppend(Token name, Vector3 backwardPos, Vector3 forwardPos)
        {
            Name = name;
            Length = Vector3.Distance(backwardPos, forwardPos);
        }

        /// <summary>
        /// Appends a building segment to the end of this one, using the same model and look.
        /// </summary>
        /// <param name="position">The position of the <see cref="PolylineItem.ForwardNode">ForwardNode</see>
        /// of the new building segment.</param>
        /// <param name="cloneSettings">Whether the new segment should have the
        /// same settings as this one. If false, the defaults will be used.</param>
        /// <returns>The newly created building segment.</returns>
        public Buildings Append(Vector3 position, bool cloneSettings = true)
        {
            if (!cloneSettings)
            {
                return Append(position, Name, Look);
            }

            var b = Append(position, Name, Look);
            CopySettingsTo(b);
            return b;
        }

        private void CopySettingsTo(Buildings b)
        {
            b.Kdop.Flags = Kdop.Flags;
            b.ViewDistance = ViewDistance;
            b.RandomSeed = RandomSeed;
            b.Stretch = Stretch;
            b.HeightOffsets = new List<float>(HeightOffsets);
        }

        /// <summary>
        /// Appends a building segment to the end of this one with the given model and look.
        /// </summary>
        /// <param name="position">The position of the ForwardNode of the
        /// new building segment.</param>
        /// <param name="name">The unit name of the building.</param>
        /// <param name="look">The look.</param>
        /// <returns>The newly created building segment.</returns>
        public Buildings Append(Vector3 position, Token name, Token look)
        {
            var building = Append<Buildings>(position);
            building.InitFromAddOrAppend(name, ForwardNode.Position, position);
            building.Look = look;
            return building;
        }
    }
}
