using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Sii
{
    /// <summary>
    /// Represents a SII unit.
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

        /// <summary>
        /// Includes of this unit.
        /// </summary>
        public List<string> Includes { get; set; } = new List<string>();

        public override string ToString() => $"{Class} : {Name}";

        public Unit() { }

        public Unit(string className, string name)
        {
            Class = className;
            Name = name;
        }

    }
}
