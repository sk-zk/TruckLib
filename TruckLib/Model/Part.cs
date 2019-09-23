using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Model
{
    public class Part
    {
        public Token Name { get; set; }

        public uint PieceCount { get; set; }

        public uint PiecesIndex { get; set; }

        public uint LocatorCount { get; set; }

        public uint LocatorsIndex { get; set; }

        public override string ToString()
        {
            return Name.String;
        }
    }
}
