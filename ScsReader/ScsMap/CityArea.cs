using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Represents a rectangular city area.
    /// </summary>
    public class CityArea : SingleNodeItem
    {
        public override ItemType ItemType => ItemType.City;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceFar;

        /// <summary>
        /// The unit name of the city.
        /// </summary>
        public Token CityName;

        public float Width = 100f;

        public float Height = 100f;

        public bool TriggerVisit
        {
            get => !Flags[1];
            set => Flags[1] = !value;
        }

        /// <summary>
        /// Determines if the city name is displayed on UI maps.
        /// </summary>
        public bool ShowInUi
        {
            get => !Flags[0];
            set => Flags[0] = !value;
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

            city.CityName = name;
            city.Width = width;
            city.Height = height;

            return city;
        }

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            CityName = r.ReadToken();
            Width = r.ReadSingle();
            Height = r.ReadSingle();
            Node = new UnresolvedNode(r.ReadUInt64());
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            w.Write(CityName);
            w.Write(Width);
            w.Write(Height);
            w.Write(Node.Uid);
        }

    }
}
