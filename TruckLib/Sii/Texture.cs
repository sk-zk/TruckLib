using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Sii
{
    public class Texture
    {
        public string Name { get; set; }

        public Dictionary<string, dynamic> Attributes { get; set; } = new();
    }
}
