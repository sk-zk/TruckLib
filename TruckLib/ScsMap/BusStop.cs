using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class BusStop : PrefabSlaveItem
    {
        public override ItemType ItemType => ItemType.BusStop;

        public Token CityName { get; set; }

        public static BusStop Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<BusStop>(map, parent, position);
        }
    }
}
