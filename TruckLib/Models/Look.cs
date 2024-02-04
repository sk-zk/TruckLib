using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Models
{
    public class Look
    {
        public Token Name { get; set; }

        public List<string> Materials { get; set; } = new List<string>();

        public Look()
        {
        }

        public Look(Token name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.String;
        }
    }
}
