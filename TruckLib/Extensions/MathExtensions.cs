using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    internal static class MathExtensions
    {
        /// <summary>
        /// Converts a quaternion to Euler angles.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <returns>Euler angles in radians.</returns>
        public static Vector3 ToEuler(this Quaternion q)
        {
            // via https://stackoverflow.com/a/56055813

            double x, y, z;

            // if the input quaternion is normalized, this is exactly one. 
            // Otherwise, this acts as a correction factor for the quaternion's not-normalizedness
            float unit = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W);

            // this will have a magnitude of 0.5 or greater if and only if this is a singularity case
            float test = q.X * q.W - q.Y * q.Z;

            if (test > 0.4995f * unit) // singularity at north pole
            {
                x = Math.PI / 2;
                y = 2f * Math.Atan2(q.Y, q.X);
                z = 0;
            }
            else if (test < -0.4995f * unit) // singularity at south pole
            {
                x = -Math.PI / 2;
                y = -2f * Math.Atan2(q.Y, q.X);
                z = 0;
            }
            else // no singularity - this is the majority of cases
            {
                x = Math.Asin(2f * (q.W * q.X - q.Y * q.Z));
                y = Math.Atan2(2f * q.W * q.Y + 2f * q.Z * q.X, 1 - 2f * (q.X * q.X + q.Y * q.Y));
                z = Math.Atan2(2f * q.W * q.Z + 2f * q.X * q.Y, 1 - 2f * (q.Z * q.Z + q.X * q.X));
            }

            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Converts a quaternion to Euler angles in degrees.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <returns>Euler angles in degrees.</returns>
        public static Vector3 ToEulerDeg(this Quaternion q)
        {
            var euler = q.ToEuler();
            euler.X = MathEx.Deg(euler.X);
            euler.Y = MathEx.Deg(euler.Y);
            euler.Z = MathEx.Deg(euler.Z);
            return euler;
        }
    }
}
