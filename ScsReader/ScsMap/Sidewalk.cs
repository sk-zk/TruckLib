using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Sidewalk data for legacy roads.
    /// </summary>
    public struct Sidewalk
    {
        /// <summary>
        /// The name of the sidewalk material.
        /// </summary>
        public Token Material;

        /// <summary>
        /// The size of the sidewalk.
        /// </summary>
        public SidewalkSize Size;
    }
}
