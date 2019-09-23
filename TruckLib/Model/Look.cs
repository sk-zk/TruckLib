using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Model
{
    public class Look
    {
        public Token Name { get; set; }

        public List<string> Materials { get; set; }

        public Look()
        {
        }

        public Look(Token name)
        {
            Name = name;
        }
    }
}
