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

        public BusStop() : base() { }

        internal BusStop(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
        }

        public static BusStop Add(IItemContainer map, Prefab parent, Vector3 position)
        {
            return PrefabSlaveItem.Add<BusStop>(map, parent, position);
        }
    }
}
