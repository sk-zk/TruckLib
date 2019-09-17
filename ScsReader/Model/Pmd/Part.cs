using System;
using System.Collections.Generic;
using System.Text;

namespace ScsReader.Model.Pmd
{
    public class Part
    {
        public Token Name { get; set; }

        public List<PartAttribute> Attributes { get; set; } = new List<PartAttribute>();
    }
}
