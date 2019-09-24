using System;
using System.Collections.Generic;
using System.Text;

namespace TruckLib.Sii
{
    public class MatFile
    {
        /// <summary>
        /// Name of the shader, e.g. "eut2.dif.spec.rfx".
        /// </summary>
        public string Effect;

        /// <summary>
        /// Attributes of the shader.
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }
            = new Dictionary<string, object>();

        public static MatFile FromString(string mat)
        {
            return MatParser.DeserializeFromString(mat);
        }

        public static MatFile FromFile(string path)
        {
            return MatParser.DeserializeFromFile(path);
        }

    }
}
