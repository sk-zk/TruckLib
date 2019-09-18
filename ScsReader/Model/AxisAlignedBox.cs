using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace ScsReader.Model
{
    /// <summary>
    /// Axis-aligned bounding box (AABB).
    /// </summary>
    public class AxisAlignedBox : IBinarySerializable
    {
        public Vector3 Start { get; set; }

        public Vector3 End { get; set; }

        public void ReadFromStream(BinaryReader r)
        {
            Start = r.ReadVector3();
            End = r.ReadVector3();
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Start);
            w.Write(End);
        }
    }
}
