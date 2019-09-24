using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Sii
{
    public class SiiParserException : Exception
    {
        public SiiParserException(string message) : base(message)
        {
        }
    }
}
