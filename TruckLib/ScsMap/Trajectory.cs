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
    /// Used for Special Transport DLC. TODO: Figure out how this works
    /// </summary>
    public class Trajectory : PathItem
    {
        public override ItemType ItemType => ItemType.Trajectory;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public Token AccessRule { get; set; } 

        public List<TrajectoryRule> Rules { get; set; } 

        public List<TrajectoryCheckpoint> Checkpoints { get; set; } 

        public List<Token> Tags { get; set; }

        public bool ForceThisWay
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public bool LowProbability
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public bool LimitDisplacement
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        }

        public bool Public
        {
            get => Kdop.Flags[12];
            set => Kdop.Flags[12] = value;
        }

        public bool ParkingSpot
        {
            get => Kdop.Flags[13];
            set => Kdop.Flags[13] = value;
        }

        public bool OneWayBonus
        {
            get => Kdop.Flags[14];
            set => Kdop.Flags[14] = value;
        }

        public bool Spawning
        {
            get => Kdop.Flags[15];
            set => Kdop.Flags[15] = value;
        }

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[16];
            set => Kdop.Flags[16] = value;
        }


        public Nibble PriorityModifier
        {
            get => (Nibble)Kdop.Flags.GetBitString(8, 4);
            set => Kdop.Flags.SetBitString(8, 4, (uint)value);
        }

        public BlinkerType BlinkerType
        {
            get => (BlinkerType)Kdop.Flags.GetBitString(4, 3);
            set => Kdop.Flags.SetBitString(4, 3, (uint)value);
        }

        public Trajectory() : base() { }

        internal Trajectory(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            AccessRule = "_tj_default";
            Rules = new List<TrajectoryRule>();
            Checkpoints = new List<TrajectoryCheckpoint>();
            Tags = new List<Token>();
        }

        /// <summary>
        /// Adds a trajectory to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="positions">The points of the path.</param>
        /// <returns>The newly created trajectory.</returns>
        public static Trajectory Add(IItemContainer map, IList<Vector3> positions)
        {
            var tj = Add<Trajectory>(map, positions);
            return tj;
        }
    }
}
