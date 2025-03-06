using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    /// <summary>
    /// Represents a point which has a position and a rotation.
    /// </summary>
    public record struct OrientedPoint(Vector3 Position, Quaternion Rotation);
}
