using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Model
{
    public class Part
    {
        public Token Name { get; set; }

        public List<Piece> Pieces { get; set; } = new List<Piece>();

        public List<Locator> Locators { get; set; } = new List<Locator>();

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
            return Name;
        }
    }
}
