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
    /// A prefab slave item which is placed for the <c>BusStop</c> spawn point type of a prefab.
    /// </summary>
    public class BusStop : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.BusStop;

        /// <summary>
        /// Unit name of the city this is a bus stop for, as defined in <c>/def/city.sii</c>.
        /// </summary>
        public Token CityName { get; set; }

        public BusStop() : base() { }

        internal BusStop(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Adds a bus stop item to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="parent">The prefab this item is linked to.</param>
        /// <param name="position">The (global) position of the bus stop node.</param>
        /// <returns>The newly created bus stop.</returns>
        public static BusStop Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<BusStop>(map, parent, position);
        }
    }
}
