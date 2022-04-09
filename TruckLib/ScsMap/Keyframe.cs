using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    public class Keyframe : IBinarySerializable
    {
        public EasingFunction SpeedChange { get; set; } = EasingFunction.EaseInOutSine;

        public EasingFunction RotationChange { get; set; } = EasingFunction.EaseInOutSine;

        public float SpeedCoefficient { get; set; } = 1;

        public float Fov { get; set; } = 60;

        public Vector3 ForwardTangentPosition { get; set; }

        public Vector3 BackwardTangentPosition { get; set; }

        public void Deserialize(BinaryReader r)
        {
            SpeedChange = (EasingFunction)r.ReadInt32();
            RotationChange = (EasingFunction)r.ReadInt32();
            SpeedCoefficient = r.ReadSingle();
            Fov = r.ReadSingle();
            BackwardTangentPosition = r.ReadVector3();
            ForwardTangentPosition = r.ReadVector3();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write((int)SpeedChange);
            w.Write((int)RotationChange);
            w.Write(SpeedCoefficient);
            w.Write(Fov);
            w.Write(BackwardTangentPosition);
            w.Write(ForwardTangentPosition);
        }
    }
}
