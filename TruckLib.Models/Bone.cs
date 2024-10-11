using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using TruckLib;

namespace TruckLib.Models
{
    public class Bone : IBinarySerializable
    {
        public Token Name { get; set; }

        public Matrix4x4 Transformation { get; set; }

        public Matrix4x4 TransformationReserved { get; set; }

        public Quaternion Stretch { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Translation { get; set; }

        public Vector3 Scale { get; set; }

        public float SignOfDeterminantOfMatrix { get; set; } = 1f;

        public int Parent { get; set; } = -1;

        public override string ToString()
        {
            return Name.String;
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Name = r.ReadToken();

            Transformation = r.ReadMatrix4x4();
            TransformationReserved = r.ReadMatrix4x4();

            Stretch = r.ReadQuaternion();
            Rotation = r.ReadQuaternion();
            Translation = r.ReadVector3();
            Scale = r.ReadVector3();
            SignOfDeterminantOfMatrix = r.ReadSingle();

            Parent = r.ReadInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Name);

            w.Write(Transformation);
            w.Write(TransformationReserved);

            w.Write(Stretch);
            w.Write(Rotation);
            w.Write(Translation);
            w.Write(Scale);
            w.Write(SignOfDeterminantOfMatrix);

            w.Write(Parent);
        }

    }
}
