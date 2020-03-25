using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Vegetation on the terrain on the side of a road.
    /// </summary>
    public class RoadVegetation : Vegetation, IBinarySerializable
    {
        private float from;
        /// <summary>
        /// Where vegetation will start appearing, in meters from the center.
        /// </summary>
        public float From
        {
            get => from;
            set => from = Utils.SetIfInRange(value, 0, 6500);
        }

        private float to;
        /// <summary>
        /// Where vegetation will stop appearing, in meters from the center.
        /// </summary>
        public float To
        {
            get => to;
            set => to = Utils.SetIfInRange(value, 0, 6500);
        }

        /// <summary>
        /// Cutoff point for high poly models, in meters from the center.
        /// </summary>
        public byte HighPolyDistance { get; set; } = 50;

        private const float densityFactor = 10f;
        private const float fromToFactor = 10f;

        public void Deserialize(BinaryReader r)
        {
            Name = r.ReadToken();
            Density = r.ReadUInt16() / densityFactor;
            HighPolyDistance = r.ReadByte();
            Scale = (VegetationScale)r.ReadByte();
            From = r.ReadUInt16() / fromToFactor;
            To = r.ReadUInt16() / fromToFactor;
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Name);
            w.Write((ushort)(Density * densityFactor));
            w.Write(HighPolyDistance);
            w.Write((byte)Scale);
            w.Write((ushort)(From * fromToFactor));
            w.Write((ushort)(To * fromToFactor));
        }
    }

}
