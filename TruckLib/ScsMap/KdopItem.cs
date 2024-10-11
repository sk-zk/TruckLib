using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the kdop_item struct.
    /// This class is used to simplify deserialization and should not be
    /// directly exposed in item classes.
    /// </summary>
    internal class KdopItem : KdopBounds
    {
        /// <summary>
        /// The UID of the item.
        /// </summary>
        public ulong Uid { get; set; }

        /// <summary>
        /// A flag field which is part of the kdop_item but is actually used for item flags 
        /// rather than flags relating to the bounding box.
        /// </summary>
        internal FlagField Flags;

        private const ushort MinDistance = 10;
        // The editor only lets you go up to 1500, but there are items in europe.mbd that exceed it.
        // Not sure if this is intentional and the game actually renders it like that, or if someone somehow
        // set those values by accident and the game caps them to 1500 again when loaded
        private const ushort MaxDistance = 2120;
        private ushort viewDistance = ViewDistanceClose;
        /// <summary>
        /// Gets or sets the view distance of an item in meters.
        /// </summary>
        public ushort ViewDistance
        {
            get => viewDistance;
            set => viewDistance = Utils.SetIfInRange(value, MinDistance, MaxDistance);
        }

        // preset vals from the editor
        public const ushort ViewDistanceExtremelyShort = 80;
        public const ushort ViewDistanceVeryShort = 120;
        public const ushort ViewDistanceShort = 200;
        public const ushort ViewDistanceClose = 400;
        public const ushort ViewDistanceMiddle = 600;
        public const ushort ViewDistanceFar = 900;
        public const ushort ViewDistanceExtremelyFar = 1400;

        public KdopItem() : base() { }

        public KdopItem(ulong uid) : this()
        {
            Uid = uid;
        }

        internal KdopItem(bool initFields) : base(initFields) { }
    }
}
