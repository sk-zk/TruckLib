using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    public static class MathEx
    {
        // Don't look too closely at this file


        /// <summary>
        /// Factor for converting radians to degrees.
        /// </summary>
        public const double RadToDeg = 180.0 / Math.PI;

        /// <summary>
        /// Factor for converting degrees to radians.
        /// </summary>
        public const double DegToRad = Math.PI / 180.0;

        /// <summary>
        /// Returns the angle between (b - a) and the Z axis as a quaternion.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Quaternion GetNodeRotation(Vector3 a, Vector3 b)
        {
            var angle = GetNodeAngle(a, b);
            var rotation = Quaternion.CreateFromYawPitchRoll((float)angle, 0f, 0f);
            return rotation;
        }

        public static double GetNodeAngle(Vector3 a, Vector3 b)
        {
            var direction = Vector3.Normalize(b - a);
            var angle = AngleOffAroundAxis(direction, -Vector3.UnitZ, Vector3.UnitY, false);
            return angle;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rot)
        {
            return Vector3.Transform((point - pivot), rot) + pivot;
        }

        /// <summary>
        /// Calculates the angle between two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The angle in radians.</returns>
        public static double Angle(Vector3 a, Vector3 b)
        {
            return Math.Acos(Vector3.Dot(a, b) / (a.Length() * b.Length()));
        }

        public static double AngleOffAroundAxis(Vector3 v, Vector3 forward, Vector3 axis, bool clockwise = false)
        {
            // via https://forum.unity.com/threads/how-to-find-the-360-angle-between-2-vectors.511650/#post-3355049

            Vector3 right;
            if (clockwise)
            {
                right = Vector3.Cross(forward, axis);
                forward = Vector3.Cross(axis, right);
            }
            else
            {
                right = Vector3.Cross(axis, forward);
                forward = Vector3.Cross(right, axis);
            }
            return Math.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward));
        }

    }
}
