using System;
using System.Collections.Generic;
using System.Text;

namespace ScsReader.Model
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
