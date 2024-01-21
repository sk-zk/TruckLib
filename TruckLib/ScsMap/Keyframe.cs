using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the properties of one keyframe of a <see cref="CameraPath">camera path</see>.
    /// </summary>
    public class Keyframe : IBinarySerializable
    {
        /// <summary>
        /// Easing function to use for speed changes between keyframes.
        /// </summary>
        public EasingFunction SpeedChange { get; set; } = EasingFunction.EaseInOutSine;

        /// <summary>
        /// Easing function to use for rotation changes between keyframes.
        /// </summary>
        public EasingFunction RotationChange { get; set; } = EasingFunction.EaseInOutSine;

        /// <summary>
        /// Multiplier for the main camera speed.
        /// </summary>
        public float SpeedCoefficient { get; set; } = 1;

        /// <summary>
        /// FOV of the camera for this keyframe.
        /// </summary>
        public float Fov { get; set; } = 60;

        /// <summary>
        /// Position of the forward control point of the node this keyframe belongs to.
        /// </summary>
        public Vector3 ControlPoint1Position { get; set; }

        /// <summary>
        /// Position of the backward control point of the subsequent node. 
        /// </summary>
        public Vector3 ControlPoint2Position { get; set; }

        /// <inheritdoc/>
        public void Deserialize(BinaryReader r)
        {
            SpeedChange = (EasingFunction)r.ReadInt32();
            RotationChange = (EasingFunction)r.ReadInt32();
            SpeedCoefficient = r.ReadSingle();
            Fov = r.ReadSingle();
            ControlPoint2Position = r.ReadVector3();
            ControlPoint1Position = r.ReadVector3();
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write((int)SpeedChange);
            w.Write((int)RotationChange);
            w.Write(SpeedCoefficient);
            w.Write(Fov);
            w.Write(ControlPoint2Position);
            w.Write(ControlPoint1Position);
        }
    }
}
