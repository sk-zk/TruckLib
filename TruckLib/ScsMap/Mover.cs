using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A model with a static loop animation.
    /// </summary>
    public class Mover : PathItem
    {
        public override ItemType ItemType => ItemType.Mover;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        public Token Model { get; set; }

        public Token Look { get; set; }

        public float Speed { get; set; }

        /// <summary>
        /// Sets for how long objects pause at the end of the path, in seconds.
        /// </summary>
        public float DelayAtEnd { get; set; }

        /// <summary>
        /// Width of the path.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The amount of models that will appear on the path.
        /// </summary>
        public uint Count { get; set; }

        public List<float> Lengths { get; set; } 

        public List<Token> Tags { get; set; }

        /// <summary>
        /// Sets the mover to be active when street lamps are off.
        /// </summary>
        public bool ActiveDuringDay
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        /// <summary>
        /// Sets the mover to be active when street lamps are on.
        /// </summary>
        public bool ActiveDuringNight
        {
            get => Kdop.Flags[2];
            set => Kdop.Flags[2] = value;
        }

        public bool BounceAtEnd
        {
            get => Kdop.Flags[3];
            set => Kdop.Flags[3] = value;
        }

        public bool FollowDir
        {
            get => Kdop.Flags[4];
            set => Kdop.Flags[4] = value;
        }

        /// <summary>
        /// Determines if the item uses a curved path 
        /// rather than a linear path.
        /// </summary>
        public bool UseCurvedPath
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Determines if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[5];
            set => Kdop.Flags[5] = value;
        }

        /* This property has apparently been removed
         * as of 1.35 (map ver. 868)
        public bool Pedestrian
        {
            get => Flags[8];
            set => Flags[8] = value;
        }
        */
        public bool UseSound
        {
            get => !Kdop.Flags[9];
            set => Kdop.Flags[9] = !value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[6];
            set => Kdop.Flags[6] = value;
        }

        [Obsolete]
        public bool IgnoreBoundingVolume
        {
            get => Kdop.Flags[7];
            set => Kdop.Flags[7] = value;
        }

        public bool ActiveDuringBadWeather
        {
            get => Kdop.Flags[11];
            set => Kdop.Flags[11] = value;
        }

        public bool ActiveDuringNiceWeather
        {
            get => Kdop.Flags[10];
            set => Kdop.Flags[10] = value;
        }

        /// <summary>
        /// Enables static animation for moving model if available.
        /// </summary>
        public bool PreferNonMovableAnimation
        {
            get => Kdop.Flags[12];
            set => Kdop.Flags[12] = value;
        }

        public bool UniformItemPlacement
        {
            get => Kdop.Flags[13];
            set => Kdop.Flags[13] = value;
        }

        public bool KeepOrientationOnBounce
        {
            get => Kdop.Flags[14];
            set => Kdop.Flags[14] = value;
        }

        public Mover() : base() { }

        internal Mover(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            ActiveDuringDay = true;
            ActiveDuringNight = true;
            ActiveDuringNiceWeather = true;
            ActiveDuringBadWeather = true;
            FollowDir = true;
            UseCurvedPath = true;
            Lengths = new List<float>();
            Tags = new List<Token>();
            Speed = 1;
            Count = 1;
        }
    }
}
