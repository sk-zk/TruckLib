using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    public class FuelPump : Service
    {
        public override ItemType ItemType => ItemType.FuelPump;

        public new static FuelPump Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<FuelPump>(map, parent, position);
        }
    }
}
