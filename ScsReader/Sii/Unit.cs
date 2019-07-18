using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.Sii
{
    /// <summary>
    /// Represents a SII unit.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// Name of the class type of this unit.
        /// </summary>
        public string Class;

        /// <summary>
        /// Name of this unit.
        /// </summary>
        public UnitName Name;

        /// <summary>
        /// Attributes of this unit.
        /// </summary>
        public Dictionary<string, object> Attributes = new Dictionary<string, object>();

        /// <summary>
        /// Includes in this unit.
        /// </summary>
        public List<string> Includes = new List<string>();

        public override string ToString() => $"{Class}: {Name}";

        public Unit()
        {
        }

        public Unit(string className, UnitName name)
        {
            Class = className;
            Name = name;
        }

    }
}
