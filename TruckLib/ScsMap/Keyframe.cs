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

    public enum EasingFunction
    {
        Custom = 0,

        EaseInCubic = 9,
        EaseInExpo = 11,
        EaseInQuad = 8,
        EaseInQuart = 10,
        EaseInSine = 7,

        EaseInOutCubic = 4,
        EaseInOutExpo = 6,
        EaseInOutQuad = 3,
        EaseInOutQuart = 5,
        EaseInOutSine = 2,

        EaseOutCubic = 14,
        EaseOutExpo = 16,
        EaseOutQuad = 13,
        EaseOutQuart = 15,
        EaseOutSine = 12,

        Linear = 1
    }
}
