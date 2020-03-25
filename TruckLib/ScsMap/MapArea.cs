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
            get => Flags[3] ? MapAreaType.Navigation : MapAreaType.Visual;
            set => Flags[3] = (Type == MapAreaType.Navigation);
        }

        /// <summary>
        /// Color of the map area.
        /// </summary>
        public MapAreaColor Color { get; set; } = MapAreaColor.Road;

        public byte DlcGuard
        {
            get => (Flags.GetByte(1));
            set => Flags.SetByte(1, value);
        }

        public bool DrawOutline
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        public bool DrawOver
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        public static MapArea Add(IItemContainer map, Vector3[] nodePositions, MapAreaType type)
        {
            var ma = Add<MapArea>(map, nodePositions);
            ma.Type = type;
            return ma;
        }
    }
}
