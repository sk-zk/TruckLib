using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Sidewalk properties for legacy, pre-template roads.
    /// </summary>
    public class Sidewalk
    {
        /// <summary>
        /// Unit name of the sidewalk material.
        /// </summary>
        public Token Material { get; set; }

        /// <summary>
        /// The size of the sidewalk.
        /// </summary>
        public SidewalkSize Size { get; set; }

        /// <summary>
        /// Makes a deep copy of this object.
        /// </summary>
        /// <returns>A deep copy of this object.</returns>
        public Sidewalk Clone() => (Sidewalk)MemberwiseClone();
    }
}
