using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TruckLib
{
    /// <summary>
    /// Functions for interpolating cardinal splines.
    /// </summary>
    public static class CardinalSpline
    {
        public static Vector3 Interpolate(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, float tension)
        {
            // tangents
            float tensionFactor = (1f - tension) / 2f;
            Vector3 m1 = tensionFactor * (p2 - p0);
            Vector3 m2 = tensionFactor * (p3 - p1);

            double t_2 = t * t;
            double t_3 = t * t * t;

            // base functions
            double h1 = (2 * t_3) - (3 * t_2) + 1;
            double h2 = (-2 * t_3) + (3 * t_2);
            double h3 = t_3 - (2 * t_2) + t;
            double h4 = t_3 - t_2;

            // interpolation polynomial
            double x = h1 * p1.X + h3 * m1.X + h2 * p2.X + h4 * m2.X;
            double y = h1 * p1.Y + h3 * m1.Y + h2 * p2.Y + h4 * m2.Y;
            double z = h1 * p1.Z + h3 * m1.Z + h2 * p2.Z + h4 * m2.Z;

            return new Vector3((float)x, (float)y, (float)z);
        }

        public static Vector3 Derivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, float tension)
        {
            // tangents
            float tensionFactor = (1f - tension) / 2f;
            Vector3 m1 = tensionFactor * (p2 - p0);
            Vector3 m2 = tensionFactor * (p3 - p1);

            double t_2 = t * t;

            // base functions
            double h1d = (6 * t_2) - (6 * t);
            double h2d = (-6 * t_2) + (6 * t);
            double h3d = (3 * t_2) - (4 * t) + 1;
            double h4d = (3 * t_2) - (2 * t);

            // interpolation polynomial
            double x = h1d * p1.X + h3d * m1.X + h2d * p2.X + h4d * m2.X;
            double y = h1d * p1.Y + h3d * m1.Y + h2d * p2.Y + h4d * m2.Y;
            double z = h1d * p1.Z + h3d * m1.Z + h2d * p2.Z + h4d * m2.Z;

            return new Vector3((float)x, (float)y, (float)z);
        }

        public static float ApproximateLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float tension)
        {
            // Gaussian quadrature
            // see https://medium.com/@all2one/how-to-compute-the-length-of-a-spline-e44f5f04c40

            float sum = 0;
            int n = 64;
            for (int i = 0; i < n - 1; i++)
            {
                float t = (float)i / n + (1f / (2 * n));
                Vector3 deriv = Derivative(p0, p1, p2, p3, t, tension);
                sum += Vector3.Multiply(1f / n, deriv).Length();
            }

            return (float)sum;
        }
    }
}
