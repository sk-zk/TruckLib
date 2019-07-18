using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.Sii
{
    // TODO: Check that each segment (seperated by .) 
    // is also a valid token

    /// <summary>
    /// A unit name, which is a string containing tokens seperated by a period.
    /// </summary>
    public struct UnitName
    {
        public string Value;

        public UnitName(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;
    }
}
