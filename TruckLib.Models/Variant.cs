using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Models
{
    public class Variant
    {
        public Token Name { get; set; }

        public List<PartAttribute> Attributes { get; set; } = [];

        public Variant(Token name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.String;
        }
    }
}
