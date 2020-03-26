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
    /// A rectangular area which marks a city.
    /// </summary>
    public class CityArea : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.CityArea;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceFar;

        /// <summary>
        /// The unit name of the city.
        /// </summary>
        public Token Name { get; set; }

        public float Width { get; set; } = 100f;

        public float Height { get; set; } = 100f;

        public bool TriggerVisit
        {
            get => !Kdop.Flags[1];
            set => Kdop.Flags[1] = !value;
        }

        /// <summary>
        /// Determines if the city name is displayed on UI maps.
        /// </summary>
        public bool ShowInUi
        {
            get => !Kdop.Flags[0];
            set => Kdop.Flags[0] = !value;
        }
     
        public CityArea() : base()
        {
            ShowInUi = true;
            TriggerVisit = true;
        }

        /// <summary>
        /// Adds a city area to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="position">The position of the new city.</param>
        /// <param name="name">The unit name of the city.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The city area.</returns>
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
