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

        internal FlagField NavigationFlags;

        public bool ForceThisWay
        {
            get => NavigationFlags[0];
            set => NavigationFlags[0] = value;
        }

        public bool LowProbability
        {
            get => NavigationFlags[1];
            set => NavigationFlags[1] = value;
        }

        public bool LimitDisplacement
        {
            get => NavigationFlags[3];
            set => NavigationFlags[3] = value;
        }

        public bool Public
        {
            get => NavigationFlags[12];
            set => NavigationFlags[12] = value;
        }

        public bool ParkingSpot
        {
            get => NavigationFlags[13];
            set => NavigationFlags[13] = value;
        }

        public bool OneWayBonus
        {
            get => NavigationFlags[14];
            set => NavigationFlags[14] = value;
        }

        public Nibble PriorityModifier
        {
            get => (Nibble)NavigationFlags.GetBitString(8, 4);
            set => NavigationFlags.SetBitString(8, 4, (uint)value);
        }

        public BlinkerType BlinkerType
        {
            get => (BlinkerType)NavigationFlags.GetBitString(4, 3);
            set => NavigationFlags.SetBitString(4, 3, (uint)value);
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
            NavigationFlags = new FlagField();
        }
    }
}
