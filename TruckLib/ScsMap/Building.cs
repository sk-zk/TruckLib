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
    /// A building segment.
    /// </summary>
    public class Building : PolylineItem
    {
        public override ItemType ItemType => ItemType.Building;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

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

        public byte ColorVariant
        {
            get => Kdop.Flags.GetByte(2);
            set => Kdop.Flags.SetByte(2, value);
        }

        /// <summary>
        /// The seed for this building.
        /// </summary>
        public uint RandomSeed { get; set; }

        /// <summary>
        /// Stretch coefficient.
        /// </summary>
        public float Stretch { get; set; }

        /// <summary>
        /// Height offsets for individual elements of the building.
        /// </summary>
        public List<float> HeightOffsets { get; set; }

        public bool Collision
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
        /// Enables or disables accessories.
        /// </summary>
        public bool Acc
        {
            get => !Kdop.Flags[0];
            set => Kdop.Flags[0] = !value;
        }

        /// <summary>
        /// Determines if the item casts shadows.
        /// </summary>
        public bool Shadows
        {
            get => !Kdop.Flags[4];
            set => Kdop.Flags[4] = !value;
        }

        /// <summary>
        /// Determines if this item is visible in mirrors.
        /// </summary>
        public bool MirrorReflection
        {
            get => !Kdop.Flags[5];
            set => Kdop.Flags[5] = !value;
        }

        public Building() : base() { }

        internal Building(bool initFields) : base(initFields)
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
        /// Adds a single building segment to the map.
        /// </summary>
        /// <param name="name">Unit name of the building.</param>
        /// <param name="look">The look.</param>
        /// <param name="backwardPos">The position of the backward (red) node.</param>
        /// <param name="forwardPos">The position of the forward (green) node.</param>
        /// <returns>The newly created building.</returns>
        public static Building Add(IItemContainer map, Vector3 backwardPos, Vector3 forwardPos, Token name, Token look)
        {
            var building = Add<Building>(map, backwardPos, forwardPos);
            building.InitFromAddOrAppend(name, look, backwardPos, forwardPos);
            return building;
        }

        /// <summary>
        /// Initializes a building which has been created via Add or Append.
        /// </summary>
        private void InitFromAddOrAppend(Token name, Token look, Vector3 backwardPos, Vector3 forwardPos)
        {
            Name = name;
            Look = look;
            Length = Vector3.Distance(backwardPos, forwardPos);
        }

        /// <summary>
        /// Appends a building segment to this building. 
        /// </summary>
        /// <param name="position">The position of the ForwardNode of the new building.</param>
        /// <returns>The newly created building.</returns>
        public Building Append(Vector3 position, bool cloneSettings = true)
        {
            if (!cloneSettings)
            {
                return Append(position, Name, Look);
            }

            var b = Append(position, Name, Look);
            CopySettingsTo(b);
            return b;
        }

        private void CopySettingsTo(Building b)
        {
            b.Kdop.Flags = Kdop.Flags;
            b.ViewDistance = ViewDistance;
            b.RandomSeed = RandomSeed;
            b.Stretch = Stretch;
            b.HeightOffsets = new List<float>(HeightOffsets);
        }

        /// <summary>
        /// Appends a building segment to this building. 
        /// </summary>
        /// <param name="position">The position of the ForwardNode of the new building.</param>
        /// <param name="name">The unit name.</param>
        /// <param name="look">The look.</param>
        /// <returns>The newly created building.</returns>
        public Building Append(Vector3 position, Token name, Token look)
        {
            var building = Append<Building>(position);
            building.InitFromAddOrAppend(name, look, ForwardNode.Position, position);
            return building;
        }
    }
}
