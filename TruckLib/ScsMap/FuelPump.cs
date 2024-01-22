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
    /// A prefab slave item which is placed for the <c>GasStation</c> spawn point type of a prefab.
    /// </summary>
    public class FuelPump : Service
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.FuelPump;

        /// <summary>
        /// Adds a fuel pump to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="parent">The prefab this item is linked to.</param>
        /// <param name="position">The position of the node.</param>
        /// <returns>The newly created fuel pump.</returns>
        public new static FuelPump Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<FuelPump>(map, parent, position);
        }
    }
}
