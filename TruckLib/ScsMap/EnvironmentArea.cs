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
    /// Defines a rectangular area in which various environmental graphical effects are different.
    /// </summary>
    public class EnvironmentArea : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.EnvironmentArea;

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

        /// <summary>
        /// Unit name of the climate profile, as defined in <c>/def/climate.sii</c>. 
        /// </summary>
        public Token Climate { get; set; }

        /// <summary>
        /// Unit name of the reflection cube, as defined in <c>/def/relightable_cubemaps.sii</c>(?). 
        /// </summary>
        public Token ReflectionCube { get; set; }

        public bool Precipitation
        {
            get => !Kdop.Flags[0];
            set => Kdop.Flags[0] = !value;
        }

        public EnvironmentArea() : base() { }

        internal EnvironmentArea(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            Width = 200;
            Height = 200;
        }

        /// <summary>
        /// Adds an environment area to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the center of the area.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <returns>The newly created environment area.</returns>
        public static EnvironmentArea Add(IItemContainer map, Vector3 position, float width, float height)
        {
            var ea = Add<EnvironmentArea>(map, position);

            ea.Width = width;
            ea.Height = height;

            return ea;
        }
    }
}
