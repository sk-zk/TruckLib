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
        /// The kDOP bounding box which is used for rendering and collision detection.
        /// <para>Note that recomputing these values is out of scope for this library, so if you
        /// create or modify an object, don't forget to recompute the map in the editor.</para>
        /// </summary>
        public KdopBounds BoundingBox { get; set; } = new KdopBounds();

        /// <summary>
        /// A flag field which is part of the kdop_item but is actually used for item flags 
        /// rather than flags relating to the bounding box.
        /// </summary>
        public BitArray Flags { get; set; } = new BitArray(32);

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
        }

        public KdopItem(ulong uid)
        {
            Uid = uid;
        }

    }
}
