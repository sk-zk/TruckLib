using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TruckLib
{
    public class HermiteSpline
    {
        /// <param name="p0">Point 0</param>
        /// <param name="m0">Tangent 0</param>
        /// <param name="p1">Point 1</param>
        /// <param name="m1">Tangent 1</param>
        /// <param name="t">Position</param>
        /// <returns></returns>
        public static Vector3 Interpolate(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
        {
            // uses doubles internally for extra precision

            double t_2 = t * t;
            double t_3 = t * t * t;

            // base functions
            double h1 = (2 * t_3) - (3 * t_2) + 1;
            double h2 = (-2 * t_3) + (3 * t_2);
            double h3 = t_3 - (2 * t_2) + t;
            double h4 = t_3 - t_2;

            // interpolation polynomial
            double x = h1 * p0.X + h3 * m0.X + h2 * p1.X + h4 * m1.X;
            double y = h1 * p0.Y + h3 * m0.Y + h2 * p1.Y + h4 * m1.Y;
            double z = h1 * p0.Z + h3 * m0.Z + h2 * p1.Z + h4 * m1.Z;

            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <param name="p0">Point 0</param>
        /// <param name="m0">Tangent 0</param>
        /// <param name="p1">Point 1</param>
        /// <param name="m1">Tangent 1</param>
        /// <param name="t">Position</param>
        /// <returns></returns>
        public static Vector3 Derivative(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
        {
            // uses doubles internally for extra precision

            double t_2 = t * t;

            // base functions
            double h1d = (6 * t_2) - (6 * t);
            double h2d = (-6 * t_2) + (6 * t);
            double h3d = (3 * t_2) - (4 * t) + 1;
            double h4d = (3 * t_2) - (2 * t);

            // interpolation polynomial
            double x = h1d * p0.X + h3d * m0.X + h2d * p1.X + h4d * m1.X;
            double y = h1d * p0.Y + h3d * m0.Y + h2d * p1.Y + h4d * m1.Y;
            double z = h1d * p0.Z + h3d * m0.Z + h2d * p1.Z + h4d * m1.Z;

            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <param name="p0">Point 0</param>
        /// <param name="m0">Tangent 0</param>
        /// <param name="p1">Point 1</param>
        /// <param name="m1">Tangent 1</param>
        /// <returns></returns>
        public static float ApproximateLength(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1)
        {
            Vector3 prev = Vector3.Zero;
            float dist = 0;
            const int step = 1;
            for (int i = 0; i < 100; i += step)
            {
                var point = Interpolate(p0, m0, p1, m1, i / 100f);
                if (i > 0)
                {
                    dist += (point - prev).Length();
                }
                prev = point;
            }
            return dist;
        }
    }
}
