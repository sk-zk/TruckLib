using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Model
{
    public class Variant
    {
        public Token Name { get; set; }

        public List<PartAttribute> Attributes { get; set; } = new List<PartAttribute>();

        public Variant(Token name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
