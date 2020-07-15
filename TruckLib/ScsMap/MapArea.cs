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
    /// Draws a polygon onto the UI map.
    /// </summary>
    public class MapArea : PolygonItem
    {
        public override ItemType ItemType => ItemType.MapArea;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public MapAreaType Type
        {
            get => Kdop.Flags[3] ? MapAreaType.Navigation : MapAreaType.Visual;
            set => Kdop.Flags[3] = (Type == MapAreaType.Navigation);
        }

        /// <summary>
        /// Color of the map area.
        /// </summary>
        public MapAreaColor Color { get; set; }

        public byte DlcGuard
        {
            get => Kdop.Flags.GetByte(1);
            set => Kdop.Flags.SetByte(1, value);
        }

        public bool DrawOutline
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        public bool DrawOver
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        public MapArea() : base() { }

        internal MapArea(bool initFields) : base(initFields)
        {
            if (initFields) Init();
        }

        protected override void Init()
        {
            base.Init();
            Color = MapAreaColor.Road;
        }

        public static MapArea Add(IItemContainer map, IList<Vector3> nodePositions, MapAreaType type)
        {
            var ma = Add<MapArea>(map, nodePositions);
            ma.Type = type;
            return ma;
        }
    }
}
