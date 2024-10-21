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
    /// Defines the tracking point for various cutscenes
    /// and is also used to create random events.
    /// </summary>
    public class CameraPoint : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.CameraPoint;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Tags of this item.
        /// </summary>
        public List<Token> Tags { get; set; }

        public CameraPoint() : base() { }

        internal CameraPoint(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Tags = [];
        }

        /// <summary>
        /// Adds a camera point to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the item.</param>
        /// <param name="tags">The tags of the item.</param>
        /// <returns>The newly created camera point.</returns>
        public static CameraPoint Add(IItemContainer map, Vector3 position, List<Token> tags)
        {
            var point = Add<CameraPoint>(map, position);
            point.Tags = tags;
            return point;
        }
    }
}
