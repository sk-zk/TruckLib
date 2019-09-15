using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace ScsReader.Model.Pmg
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

        public void ReadFromStream(BinaryReader r)
        {
            Name = r.ReadToken();

            // TODO
            Transformation = new Matrix4x4(
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()
                );

            TransformationReserved = new Matrix4x4(
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()
                );

            Stretch = r.ReadQuaternion();
            Rotation = r.ReadQuaternion();
            Translation = r.ReadVector3();
            Scale = r.ReadVector3();
            SignOfDeterminantOfMatrix = r.ReadSingle();

            Parent = r.ReadInt32();
        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }

    }
}
