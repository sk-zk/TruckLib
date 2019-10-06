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
    /// Defines an area without weather effects.
    /// </summary>
    public class NoWeatherArea : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.NoWeatherArea;

        public override ItemFile DefaultItemFile => ItemFile.Aux;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public float Width { get; set; }

        public float Height { get; set; }

        /// <summary>
        /// Adds a NoWeatherArea to the map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static NoWeatherArea Add(IItemContainer map, Vector3 position, float width, float height)
        {
            var nwa = Add<NoWeatherArea>(map, position);

            nwa.Width = width;
            nwa.Height = height;

            return nwa;
        }

        private const float sizeFactor = 2f;

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            Width = r.ReadSingle() * sizeFactor;
            Height = r.ReadSingle() * sizeFactor;
            Node = new UnresolvedNode(r.ReadUInt64());
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(Width / sizeFactor);
            w.Write(Height / sizeFactor);
            w.Write(Node.Uid);
        }
    }
}
