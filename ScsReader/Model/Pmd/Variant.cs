using System;
using System.Collections.Generic;
using System.Text;

namespace ScsReader.Model.Pmd
{
    public class Variant
    {
        public Token Name { get; set; }

        public List<Part> Parts { get; set; } = new List<Part>();
    }
}
