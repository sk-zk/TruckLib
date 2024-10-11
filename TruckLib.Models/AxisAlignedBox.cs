using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using TruckLib;

namespace TruckLib.Models
{
    /// <summary>
    /// Axis-aligned bounding box (AABB).
    /// </summary>
    public class AxisAlignedBox : IBinarySerializable
    {
        public Vector3 Start { get; set; }

        public Vector3 End { get; set; }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Start = r.ReadVector3();
            End = r.ReadVector3();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Start);
            w.Write(End);
        }
    }
}
