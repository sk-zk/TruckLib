using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Sii
{
    /// <summary>
    /// Represents a texture unit of a .mat file.
    /// </summary>
    public class Texture
    {
        public string Name { get; set; }

        public Dictionary<string, dynamic> Attributes { get; set; } = new();
    }
}
