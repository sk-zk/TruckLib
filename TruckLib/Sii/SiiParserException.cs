using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Sii
{
    /// <summary>
    /// Thrown if the SII parser encounters a problem.
    /// </summary>
    public class SiiParserException : Exception
    {
        public SiiParserException(string message) : base(message) { }
    }
}
