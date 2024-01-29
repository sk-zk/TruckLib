using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Sii
{
    /// <summary>
    /// Represents an SII unit.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// Class name of this unit.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Name of this unit.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Attributes of this unit.
        /// </summary>
        public Dictionary<string, dynamic> Attributes { get; set; } 
            = new Dictionary<string, dynamic>();

        public override string ToString() => $"{Class} : {Name}";

        /// <summary>
        /// Instantiates an empty unit.
        /// </summary>
        public Unit() { }

        /// <summary>
        /// Instantiates a unit with the given class name and unit name.
        /// </summary>
        /// <param name="className">The class name.</param>
        /// <param name="name">The unit name.</param>
        public Unit(string className, string name)
        {
            Class = className;
            Name = name;
        }
    }
}
