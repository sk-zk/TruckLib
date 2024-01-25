using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    internal static class MapItemUtils
    {
        /// <summary>
        /// Calculates the lengths of the sections of a path defined by a <see cref="Mover"/> or <see cref="Walker"/>.
        /// </summary>
        /// <param name="nodes">The nodes of the item.</param>
        /// <param name="useCurvedPath">Whether the path is a Catmull-Rom spline rather than linear.</param>
        /// <returns>The calculated lengths.</returns>
        internal static List<float> CalculatePathLengths(IList<INode> nodes, bool useCurvedPath)
        {
            var lengths = new List<float>(nodes.Count - 1);
            if (nodes.Count < 2) return lengths;

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                float length;
                if (useCurvedPath)
                {
                    var p0 = nodes[Math.Max(0, i - 1)].Position;
                    var p1 = nodes[i].Position;
                    var p2 = nodes[Math.Min(nodes.Count - 1, i + 1)].Position;
                    var p3 = nodes[Math.Min(nodes.Count - 1, i + 2)].Position;
                    length = CatmullRomSpline.ApproximateLength(p0, p1, p2, p3);
                }
                else
                {
                    length = (nodes[i + 1].Position - nodes[i].Position).Length();
                }
                lengths.Add(length);
            }
            return lengths;
        }
    }
}
