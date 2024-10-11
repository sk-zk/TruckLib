using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Models
{
    public class Part
    {
        public Token Name { get; set; }

        public List<Piece> Pieces { get; set; } = [];

        public List<Locator> Locators { get; set; } = [];

        public Part()
        {
            Name = "untitled";
        }

        public Part(Token name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.String;
        }
    }
}
