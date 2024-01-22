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
    /// Defines a rectangular area in which weather effects are disabled.
    /// </summary>
    public class NoWeatherArea : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.NoWeatherArea;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Aux;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Width of the area.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height of the area.
        /// </summary>
        public float Height { get; set; }

        public FogMask FogBehavior { get; set; }

        public NoWeatherArea() : base() { }

        internal NoWeatherArea(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Adds a NoWeatherArea to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the center of the area.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <returns>The newly created NoWeatherArea.</returns>
        public static NoWeatherArea Add(IItemContainer map, Vector3 position, float width, float height)
        {
            var nwa = Add<NoWeatherArea>(map, position);

            nwa.Width = width;
            nwa.Height = height;

            return nwa;
        }
    }
}
