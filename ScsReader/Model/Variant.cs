using System;
using System.Collections.Generic;
using System.Text;

namespace ScsReader.Model
{
    public class Variant
    {
        public Token Name { get; set; }

        public List<PartAttribute> Attributes { get; set; } = new List<PartAttribute>();

        public override string ToString()
        {
            return Name.String;
        }
    }
}
