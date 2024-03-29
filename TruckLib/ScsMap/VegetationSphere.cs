﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A vegetation sphere.
    /// </summary>
    public class VegetationSphere : IBinarySerializable
    {
        /// <summary>
        /// The position of the sphere.
        /// <para>X is the position along the road in percent (so 0.75 means 
        ///       3/4 of the way towards the forward node).</para>
        /// <para>Y describes the Y position of the sphere in meters relative to
        ///       the terrain height at the sphere's X/Z coordinates
        ///       (so 1 means 1 meter above ground).</para>
        /// <para>Z describes the position on the axis perpendicular to the road
        ///       in meters (so 20 means 20 meters into the terrain at X).</para>
        /// </summary>
        public Vector3 Position { get; set; } = new Vector3();

        /// <summary>
        /// The radius of the sphere.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// The modifier which is applied to the vegetation inside the sphere.
        /// </summary>
        public VegetationSphereType Type { get; set; } 
            = VegetationSphereType.NoVegetation;

        /// <inheritdoc/>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Position = r.ReadVector3();
            Radius = r.ReadSingle();
            Type = (VegetationSphereType)r.ReadUInt32();
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write(Position);
            w.Write(Radius);
            w.Write((int)Type);
        }
    }
}
