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
    /// Represents the kdop_item struct in the SCS map format.
    /// This class is used to simplify deserialization and should not be
    /// directly exposed in item classes.
    /// </summary>
    internal class KdopItem
    {
        /// <summary>
        /// The UID of the item.
        /// </summary>
        public ulong Uid { get; set; } = 0;

        /// <summary>
        /// Minimums of the kDOP bounding box which is used for rendering and collision detection.
        /// </summary>
        public float[] Minimums { get; set; } = new float[5];

        /// <summary>
        /// Maximums of the kDOP bounding box which is used for rendering and collision detection.
        /// </summary>
        public float[] Maximums { get; set; } = new float[5];

        /// <summary>
        /// A flag field which is part of the kdop_item but is actually used for item flags 
        /// rather than flags relating to the bounding box.
        /// </summary>
        internal FlagField Flags;

        private static ushort MinDistance = 10;
        private static ushort MaxDistance = 1500;
        private ushort viewDistance = ViewDistanceClose;
        /// <summary>
        /// View distance of an item in meters.
        /// </summary>
        public ushort ViewDistance
        {
            get => viewDistance;
            set => viewDistance = Utils.SetIfInRange(value, MinDistance, MaxDistance);
        }

        // preset vals from the editor
        public const ushort ViewDistanceShort = 120;
        public const ushort ViewDistanceClose = 400;
        public const ushort ViewDistanceMiddle = 950;
        public const ushort ViewDistanceFar = 1400;

        public KdopItem() 
        {
            Minimums[0] = 1;
            Minimums[1] = 1;
            Minimums[2] = 1;
            Maximums[0] = 2;
            Maximums[1] = 2;
            Maximums[2] = 2;
        }

        public KdopItem(ulong uid) : this()
        {
            Uid = uid;
        }

    }
}
