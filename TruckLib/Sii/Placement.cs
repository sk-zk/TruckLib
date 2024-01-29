using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Sii
{
    /// <summary>
    /// Represents the placement type used in SII files.
    /// </summary>
    /// <remarks>I don't know what this does, beyond that it's apparently being used for
    ///  Winter Wonderland portals, so the property names are a complete guess.</remarks>
    public struct Placement
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Placement(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
