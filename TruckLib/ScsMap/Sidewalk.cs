using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Sidewalk data for legacy roads.
    /// </summary>
    public class Sidewalk
    {
        /// <summary>
        /// The name of the sidewalk material.
        /// </summary>
        public Token Material { get; set; }

        /// <summary>
        /// The size of the sidewalk.
        /// </summary>
        public SidewalkSize Size { get; set; }

        public Sidewalk Clone() => (Sidewalk)MemberwiseClone();
    }
}
