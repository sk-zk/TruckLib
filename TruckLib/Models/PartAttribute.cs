using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Models
{
    public class PartAttribute
    {
        public int Type { get; set; }

        public Token Tag { get; set; }

        public uint Value { get; set; }

        public override string ToString()
        {
            return $"{Tag.String}: {Value}";
        }
    }
}
