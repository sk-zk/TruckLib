using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    /// <summary>
    /// Functions for interpolating Catmull-Rom splines.
    /// </summary>
    public static class CatmullRomSpline
    {
        public static Vector3 Interpolate(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            double t_2 = t * t;
            double t_3 = t * t * t;

            double h1 = (-1 * t_3) + (2 * t_2) + (-1 * t);
            double h2 = (3 * t_3) + (-5 * t_2) + 2;
            double h3 = (-3 * t_3) + (4 * t_2) + t;
            double h4 = t_3 - t_2;

            double x = 0.5 * ((p0.X * h1) + (p1.X * h2) + (p2.X * h3) + (p3.X * h4));
            double y = 0.5 * ((p0.Y * h1) + (p1.Y * h2) + (p2.Y * h3) + (p3.Y * h4));
            double z = 0.5 * ((p0.Z * h1) + (p1.Z * h2) + (p2.Z * h3) + (p3.Z * h4));

            return new Vector3((float)x, (float)y, (float)z);
        }

        public static Vector3 Derivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            double t_2 = t * t;

            double h1d = (-3 * t_2) + (4 * t) - 1;
            double h2d = (9 * t_2) - (10 * t);
            double h3d = (-9 * t_2) + (8 * t) + 1;
            double h4d = (3 * t_2) - (2 * t);

            double x = 0.5 * ((p0.X * h1d) + (p1.X * h2d) + (p2.X * h3d) + (p3.X * h4d));
            double y = 0.5 * ((p0.Y * h1d) + (p1.Y * h2d) + (p2.Y * h3d) + (p3.Y * h4d));
            double z = 0.5 * ((p0.Z * h1d) + (p1.Z * h2d) + (p2.Z * h3d) + (p3.Z * h4d));

            return new Vector3((float)x, (float)y, (float)z);
        }

        public static float ApproximateLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Gaussian quadrature
            // see https://medium.com/@all2one/how-to-compute-the-length-of-a-spline-e44f5f04c40

            float sum = 0;
            int n = 64;
            for (int i = 0; i < n - 1; i++)
            {
                float t = (float)i / n + (1f / (2 * n));
                Vector3 deriv = Derivative(p0, p1, p2, p3, t);
                sum += Vector3.Multiply(1f / n, deriv).Length();
            }

            return (float)sum;
        }
    }
}
