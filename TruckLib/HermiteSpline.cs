using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib
{
    /// <summary>
    /// Functions for interpolating Hermite splines.
    /// </summary>
    public static class HermiteSpline
    {
        public static Vector3 InterpolatePolyline(INode start, INode end, float t)
        {
            var (tanStart, tanEnd) = CalculateTangents(start, end);
            return Interpolate(start.Position, end.Position, tanStart, tanEnd, t);
        }

        public static Vector3 Interpolate(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1, float t)
        {
            var t2 = t * t;
            var t3 = t * t * t;
            return (2 * t3 - 3 * t2 + 1) * p0 +
                (t3 - 2 * t2 + t) * m0 +
                (-2 * t3 + 3 * t2) * p1 +
                (t3 - t2) * m1;
        }

        public static Vector3 DerivativePolyline(INode start, INode end, float t)
        {
            var (tanStart, tanEnd) = CalculateTangents(start, end);
            return Derivative(start.Position, end.Position, tanStart, tanEnd, t);
        }

        public static Vector3 Derivative(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1, float t)
        {
            var t2 = t * t;
            return (6 * t2 - 6 * t) * p0 +
                (3 * t2 - 4 * t + 1) * m0 +
                (-6 * t2 + 6 * t) * p1 +
                (3 * t2 - 2 * t) * m1;
        }

        private static (Vector3 tanStart, Vector3 tanEnd) CalculateTangents(INode start, INode end)
        {
            var length = (end.Position - start.Position).Length();
            var initialVector = new Vector3(0, 0, -length);
            var tanStart = Vector3.Transform(initialVector, start.Rotation);
            var tanEnd = Vector3.Transform(initialVector, end.Rotation);
            return (tanStart, tanEnd);
        }
    }
}
