using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal static (Vector3 tanStart, Vector3 tanEnd) CalculateTangents(INode start, INode end)
        {
            var length = (end.Position - start.Position).Length();
            var initialVector = new Vector3(0, 0, -length);
            var tanStart = Vector3.Transform(initialVector, start.Rotation);
            var tanEnd = Vector3.Transform(initialVector, end.Rotation);
            return (tanStart, tanEnd);
        }

        /// <summary>
        /// Calculates equidistant points along the curve.
        /// </summary>
        /// <param name="n1">The starting point.</param>
        /// <param name="n2">The ending point.</param>
        /// <param name="intervals">The distances between points.</param>
        /// <param name="startOffset">The distance from the starting point where
        /// points will begin to be generated.</param>
        /// <param name="endOffset">The distance from the ending point where
        /// points will cease to be generated.</param>
        /// <param name="repeat">Whether the given intervals should repeat. If false, the method
        /// will cease to create points after each interval has been used once.</param>
        /// <returns>An ordered list of oriented points.</returns>
        public static List<OrientedPoint> GetSpacedPoints(INode n1, INode n2, float[] intervals, 
            float startOffset = 0, float endOffset = 0, bool repeat = true)
        {
            // via http://www.planetclegg.com/projects/WarpingTextToSplines.html

            if (intervals.Any(x => x < 0))
                throw new ArgumentOutOfRangeException(nameof(intervals), "Intervals must be greater than 0.");

            if (startOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(startOffset), "The start offset must not be below 0.");

            if (endOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(startOffset), "The end offset must not be below 0.");

            var (tanStart, tanEnd) = CalculateTangents(n1, n2);

            float[] arcLengths = ApproximateLengths(n1.Position, n2.Position, tanStart, tanEnd);
            float splineLength = arcLengths[^1];

            for (int i = 0; i < intervals.Length; i++)
            {
                intervals[i] /= splineLength;
            }
            startOffset /= splineLength;
            endOffset = (splineLength - endOffset) / splineLength;

            var equiPoints = new List<OrientedPoint>();
            int intervalIdx = 0;
            for (float u = startOffset; u < endOffset; u += intervals[intervalIdx])
            {
                float targetArcLength = u * splineLength;
                int index = IndexOfLargestValueSmallerThan(arcLengths, targetArcLength);

                float segmentLength = index >= arcLengths.Length - 1 
                    ? arcLengths[index] - arcLengths[index - 1] 
                    : arcLengths[index + 1] - arcLengths[index];
                float segmentFraction = (targetArcLength - arcLengths[index]) / segmentLength;

                if (segmentFraction < 0)
                    segmentFraction += 1; // why does this even work?

                float t = (index + segmentFraction) / (arcLengths.Length - 1);
                if (t > 1)
                    break;
                var position = Interpolate(n1.Position, n2.Position, tanStart, tanEnd, t);
                var rotation = MathEx.GetNodeRotation(Derivative(n1.Position, n2.Position, tanStart, tanEnd, t));
                equiPoints.Add(new(position, rotation));

                if (++intervalIdx > intervals.Length - 1)
                {
                    if (repeat)
                        intervalIdx = 0;
                    else
                        break;
                }
            }

            return equiPoints;
        }

        /// <summary>
        /// Calculates equidistant points along the curve.
        /// </summary>
        /// <param name="n1">The starting point.</param>
        /// <param name="n2">The ending point.</param>
        /// <param name="interval">The distance between points.</param>
        /// <param name="startOffset">The distance from the starting point where
        /// points will begin to be generated.</param>
        /// <param name="endOffset">The distance from the ending point where
        /// points will cease to be generated.</param>
        /// <returns>An ordered list of equidistant oriented points.</returns>
        public static List<OrientedPoint> GetEquidistantPoints(INode n1, INode n2, float interval,
            float startOffset = 0, float endOffset = 0)
        {
            return GetSpacedPoints(n1, n2, [interval], startOffset, endOffset);
        }

        private static float[] ApproximateLengths(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1)
        {
            int n = 64;
            float acc = 0;
            var arcLengths = new float[n];
            Vector3 prevPoint = Interpolate(p0, p1, m0, m1, 0);
            for (int i = 1; i < n + 1; i++)
            {
                float t = (float)i / n;
                Vector3 point = Interpolate(p0, p1, m0, m1, t);
                acc += (point - prevPoint).Length();
                arcLengths[i - 1] = acc;
                prevPoint = point;
            }

            return arcLengths;
        }

        private static int IndexOfLargestValueSmallerThan(IList<float> list, float value)
        {
            var left = 0;
            var right = list.Count - 1;
            while (left < right)
            {
                var mid = (left + right) / 2;
                if (list[mid] < value)
                    left = mid + 1;
                else
                    right = mid;
            }
            return left;
        }
    }
}
