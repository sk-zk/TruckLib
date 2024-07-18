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
    /// A City item, which declares a rectangular area around the item's node as belonging to a city.
    /// </summary>
    public class CityArea : SingleNodeItem
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.CityArea;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceFar;

        /// <summary>
        /// The unit name of the city, as defined in <c>/def/city.sii</c>.
        /// </summary>
        public Token Name { get; set; }

        /// <summary>
        /// The width of the area.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// The height of the area.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets if entering this area can trigger the "first visit" message.
        /// </summary>
        public bool TriggerVisit
        {
            get => !Kdop.Flags[1];
            set => Kdop.Flags[1] = !value;
        }

        /// <summary>
        /// Gets or sets if the city name is displayed on UI maps.
        /// </summary>
        public bool ShowInUi
        {
            get => !Kdop.Flags[0];
            set => Kdop.Flags[0] = !value;
        }  

        public CityArea() : base() { }

        internal CityArea(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            ShowInUi = true;
            TriggerVisit = true;
            Width = 100f;
            Height = 100f;
        }

        /// <summary>
        /// Adds a city area to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The center point of the area.</param>
        /// <param name="name">The unit name of the city.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <returns>The newly created city area.</returns>
        public static CityArea Add(IItemContainer map, Vector3 position, Token name, float width, float height)
        {
            var city = Add<CityArea>(map, position);

            city.Name = name;
            city.Width = width;
            city.Height = height;

            return city;
        }

    }
}
