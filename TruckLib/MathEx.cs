using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib
{
    internal static class MathEx
    {
        public static double Deg(double rad) => rad * (180.0 / Math.PI);
        public static float Deg(float rad) => (float)(rad * (180.0 / Math.PI));

        public static double Rad(double deg) => deg * (Math.PI / 180.0);
        public static float Rad(float deg) => (float)(deg * (Math.PI / 180.0));

        public static Quaternion GetNodeRotation(Vector3 a, Vector3 b)
        {
            var direction = Vector3.Normalize(b - a);
            return GetNodeRotation(direction);
        }

        public static Quaternion GetNodeRotation(Vector3 direction)
        {
            var yaw = Math.Atan2(-direction.Z, direction.X) - (Math.PI / 2);
            var pitch = Math.Atan2(direction.Y, Math.Sqrt(direction.X * direction.X + direction.Z * direction.Z));
            return Quaternion.CreateFromYawPitchRoll((float)yaw, (float)pitch, 0);
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rot)
        {
            return Vector3.Transform(point - pivot, rot) + pivot;
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

        public static double Mod(double a, double b)
        {
            return a - b * Math.Floor(a / b);
        }

        public static int Mod(int a, int b)
        {
            return a - b * (int)Math.Floor((double)a / b);
        }

        /// <summary>
        /// Returns the center point of the given nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The center point of the nodes.</returns>
        /// <exception cref="ArgumentException">Thrown if the collection is empty.</exception>
        public static Vector3 GetCenter(IEnumerable<INode> nodes)
        {
            return GetCenter(nodes.Select(x => x.Position));
        }

        /// <summary>
        /// Returns the center point of the given points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The center point of the nodes.</returns>
        /// <exception cref="ArgumentException">Thrown if the collection is empty.</exception>
        public static Vector3 GetCenter(IEnumerable<Vector3> points)
        {
            if (!points.Any())
                throw new ArgumentException("Collection must not be empty.");

            var sum = points.Aggregate((acc, point) => acc + point);
            return sum / points.Count();
        }
    }
}
